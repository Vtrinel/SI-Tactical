using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipColliderUI : MonoBehaviour, ITooltipable
{
    public void SetTooltipInformations(TooltipInformations infos)
    {
        informations = infos;
    }

    [SerializeField] TooltipInformations informations = default;
    public TooltipInformations GetTooltipInformations => informations;

    public Action OnStartTooltip { get; set; }
    public Action OnEndTooltip { get; set; }

    bool isTooltipable = true;
    public bool Tooltipable => isTooltipable;
    public void SetTooltipable(bool tooltipable)
    {
        isTooltipable = tooltipable;
    }

    public void SetValueInInfos(int value)
    {
        informations.tooltipAdditionalInformationValue = value;
    }

    public void SetName(string name)
    {
        informations.tooltipName = name; 
    }
}
