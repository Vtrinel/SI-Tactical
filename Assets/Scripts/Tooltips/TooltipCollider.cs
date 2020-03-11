using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipCollider : MonoBehaviour, ITooltipable
{
    public void SetTooltipInformations(TooltipInformations infos)
    {
        informations = infos;
    }

    [SerializeField] TooltipInformations informations = default;
    public TooltipInformations GetTooltipInformations => informations;

    public Action OnStartTooltip { get; set; }
    public Action OnEndTooltip { get; set; }
}
