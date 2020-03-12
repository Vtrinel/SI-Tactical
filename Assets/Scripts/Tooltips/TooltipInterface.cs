using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipInterface : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CanvasGroup tooltipGroup = default;
    [SerializeField] GameObject tooltipParent = default;
    [SerializeField] Text tooltipName = default;
    [SerializeField] Text tooltipDescription = default;
    [SerializeField] RectTransform rectTr = default;
    [SerializeField] RectTransform backgroundRectTr = default;

    [Header("References : Mini Tooltip")]
    [SerializeField] GameObject miniTooltipParent = default;
    [SerializeField] Text miniTooltipText = default;
    [SerializeField] RectTransform miniTooltipBackgroundRectTr = default;

    [Header("Tweaking")]
    [SerializeField] float transitionDuration = 0.5f;
    [SerializeField] float disparitionSpeed = 2;
    float currentTransitionCounter = 0;
    [SerializeField] AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private void Start()
    {
        SetTooltipVisibility(false);
        tooltipGroup.alpha = 0;
    }

    bool _visible;
    public void SetTooltipVisibility(bool visible)
    {
        _visible = visible;
    }

    bool isRegularSizeTooltip = true;
    RectTransform currentTooltipBackgroundTr = default;
    bool forceTooltipPosition = false;
    RectTransform forcedTooltipPosition = null;
    TooltipForcedPositionType tooltipForcedPositionType = TooltipForcedPositionType.None;
    public void SetTooltipInfos(TooltipInformations infos)
    {
        isRegularSizeTooltip = !infos.miniSizeTooltip;
        currentTooltipBackgroundTr = isRegularSizeTooltip ? backgroundRectTr : miniTooltipBackgroundRectTr;
        if (isRegularSizeTooltip)
        {
            tooltipParent.SetActive(true);
            miniTooltipParent.SetActive(false);
            tooltipName.text = infos.tooltipName;
            tooltipDescription.text = infos.tooltipDescription;
            switch (infos.tooltipAdditionalInformationType)
            {
                case TooltipAdditionalInformationType.None:
                    break;

                case TooltipAdditionalInformationType.ActionPointsCost:
                    tooltipName.text = tooltipName.text + " - Cost : " + infos.tooltipAdditionalInformationValue + " AP";
                    break;

                case TooltipAdditionalInformationType.LifePoints:
                    tooltipName.text = tooltipName.text + " - State : " + infos.tooltipAdditionalInformationValue + " HP";
                    break;

                case TooltipAdditionalInformationType.Damage:
                    tooltipName.text = tooltipName.text + " - Damage : " + infos.tooltipAdditionalInformationValue + " DMG";
                    break;
            }
        }
        else
        {
            tooltipParent.SetActive(false);
            miniTooltipParent.SetActive(true);
            miniTooltipText.text = infos.tooltipName;
        }

        forcedTooltipPosition = infos.forcedTooltipLPosition;
        forceTooltipPosition = (forcedTooltipPosition != null);
        tooltipForcedPositionType = infos.tooltipForcedPositionType;
    }

    private void Update()
    {
        if (currentTooltipBackgroundTr == null)
            return;

        if (!forceTooltipPosition)
        {
            Vector2 newPos = Input.mousePosition;
            if (newPos.x < currentTooltipBackgroundTr.sizeDelta.x)
                newPos.x += currentTooltipBackgroundTr.sizeDelta.x;

            if (newPos.y < currentTooltipBackgroundTr.sizeDelta.y)
                newPos.y += currentTooltipBackgroundTr.sizeDelta.y;

            rectTr.localPosition = newPos;
        }
        else
        {
            Vector2 newPos = forcedTooltipPosition.position;
            if (tooltipForcedPositionType == TooltipForcedPositionType.None)
            {
                if (newPos.x < currentTooltipBackgroundTr.sizeDelta.x)
                    newPos.x += currentTooltipBackgroundTr.sizeDelta.x;

                if (newPos.y < currentTooltipBackgroundTr.sizeDelta.y)
                    newPos.y += currentTooltipBackgroundTr.sizeDelta.y;
            }
            else
            {
                if (tooltipForcedPositionType == TooltipForcedPositionType.BottomRight || tooltipForcedPositionType == TooltipForcedPositionType.UpRight)
                    newPos.x += currentTooltipBackgroundTr.sizeDelta.x;

                if (tooltipForcedPositionType == TooltipForcedPositionType.UpLeft || tooltipForcedPositionType == TooltipForcedPositionType.UpRight)
                    newPos.y += currentTooltipBackgroundTr.sizeDelta.y;
            }

            rectTr.localPosition = newPos;
        }

        if ((_visible && currentTransitionCounter < transitionDuration) || (!_visible && currentTransitionCounter > 0))
        {
            currentTransitionCounter += Time.deltaTime * (_visible ? 1 : -disparitionSpeed);
            currentTransitionCounter = Mathf.Clamp(currentTransitionCounter, 0, transitionDuration);
            tooltipGroup.alpha = transitionCurve.Evaluate(currentTransitionCounter/transitionDuration);
        }     
    }
}
