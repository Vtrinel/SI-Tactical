using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipInterface : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CanvasGroup tooltipGroup = default;
    [SerializeField] Text tooltipName = default;
    [SerializeField] Text tooltipDescription = default;
    [SerializeField] RectTransform rectTr = default;
    [SerializeField] RectTransform backgroundRectTr = default;

    [Header("Tweaking")]
    [SerializeField] float transitionDuration = 0.5f;
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

    public void SetTooltipInfos(TooltipInformations infos)
    {
        tooltipName.text = infos.tooltipName;
        tooltipDescription.text = infos.tooltipDescription;
        switch (infos.tooltipAdditionalInformationType)
        {
            case TooltipAdditionalInformationType.None:
                break;

            case TooltipAdditionalInformationType.ActionPointsCost:
                tooltipName.text = tooltipName.text  + " - Cost : " + infos.tooltipAdditionalInformationValue + "AP";
                break;

            case TooltipAdditionalInformationType.LifePoints:
                tooltipName.text = tooltipName.text + " - State : " + infos.tooltipAdditionalInformationValue + "HP";
                break;

            case TooltipAdditionalInformationType.Damage:
                tooltipName.text = tooltipName.text + " - Damage : " + infos.tooltipAdditionalInformationValue + "HP";
                break;
        }
    }

    private void Update()
    {
        Vector2 newPos = Input.mousePosition;
        if (newPos.x < backgroundRectTr.sizeDelta.x)
            newPos.x += backgroundRectTr.sizeDelta.x;

        if (newPos.y < backgroundRectTr.sizeDelta.y)
            newPos.y += backgroundRectTr.sizeDelta.y;

        rectTr.localPosition = newPos;

        if ((_visible && currentTransitionCounter < transitionDuration) || (!_visible && currentTransitionCounter > 0))
        {
            currentTransitionCounter += Time.deltaTime * (_visible ? 1 : -1);
            currentTransitionCounter = Mathf.Clamp(currentTransitionCounter, 0, transitionDuration);
            tooltipGroup.alpha = transitionCurve.Evaluate(currentTransitionCounter/transitionDuration);
        }     
    }
}
