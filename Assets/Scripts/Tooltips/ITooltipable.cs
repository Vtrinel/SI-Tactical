using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITooltipable
{
    TooltipInformations GetTooltipInformations { get; }

    Action OnStartTooltip { get; set; }
    Action OnEndTooltip { get; set; }

    bool Tooltipable { get; }
}

public enum TooltipAdditionalInformationType
{
    None, ActionPointsCost, LifePoints, Damage
}

[System.Serializable]
public struct TooltipInformations
{
    public string tooltipName;
    [TextArea] public string tooltipDescription;
    public int tooltipAdditionalInformationValue;
    public TooltipAdditionalInformationType tooltipAdditionalInformationType;
    public bool miniSizeTooltip;
    public RectTransform forcedTooltipLPosition;
    public TooltipForcedPositionType tooltipForcedPositionType;
}

public enum TooltipForcedPositionType
{
    None, UpLeft, UpRight, BottomLeft, BottomRight
}