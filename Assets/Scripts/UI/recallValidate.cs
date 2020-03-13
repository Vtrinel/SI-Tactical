using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class recallValidate : MonoBehaviour
{
    [SerializeField] GameObject parentObj;
    [SerializeField] TooltipColliderUI tooltipCollider = default;
    [SerializeField] Animator buttonAnimator = default;

    private void OnEnable()
    {
        GameManager.Instance.OnRecallCompetenceSelectionStateChanged += ShowOrHide;
        tooltipCollider.OnStartTooltip += StartTooltip;
        tooltipCollider.OnEndTooltip += EndTooltip;
        buttonAnimator.SetBool("Usable", true);
    }

    private void OnDisable()
    {
        GameManager.Instance.OnRecallCompetenceSelectionStateChanged -= ShowOrHide;
        tooltipCollider.OnStartTooltip -= StartTooltip;
        tooltipCollider.OnEndTooltip -= EndTooltip;
    }

    void ShowOrHide(bool value)
    {
        parentObj.SetActive(value);
    }

    public void Validate()
    {
        GameManager.Instance.OnPlayerClickAction();
    }

    public void StartTooltip()
    {
        buttonAnimator.SetBool("Tooltiped", true);
    }

    public void EndTooltip()
    {
        buttonAnimator.SetBool("Tooltiped", false);
    }
}
