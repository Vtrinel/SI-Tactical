using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TooltipInformationFactory
{
    public static TooltipInformations GetUsableCompetenceTooltip(Competence competence)
    {
        TooltipInformations infos = new TooltipInformations();

        infos.tooltipName = competence.GetCompetenceName;
        infos.tooltipDescription = competence.GetCompetenceDescription;
        infos.tooltipAdditionalInformationType = TooltipAdditionalInformationType.ActionPointsCost;
        infos.tooltipAdditionalInformationValue = competence.GetActionPointsCost;

        return infos;
    }
}
