using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TooltipInformationFactory
{
    public static TooltipInformations GetUsableCompetenceTooltip(Competence competence)
    {
        TooltipInformations infos = new TooltipInformations();

        infos.tooltipName = competence.GetCompetenceName;
        infos.tooltipDescription = competence.GetCompetenceTooltip;
        infos.tooltipAdditionalInformationType = TooltipAdditionalInformationType.ActionPointsCost;
        infos.tooltipAdditionalInformationValue = competence.GetActionPointsCost;

        return infos;
    }

    public static TooltipInformations GetDiscTypeInformations(DiscInformations discInformations)
    {
        TooltipInformations infos = new TooltipInformations();

        infos.tooltipName = discInformations.tooltipName;
        infos.tooltipDescription = discInformations.tooltipDescription;
        infos.tooltipAdditionalInformationType = TooltipAdditionalInformationType.Damage;
        infos.tooltipAdditionalInformationValue = discInformations.damagesValue;

        return infos;
    }
}
