using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TooltipInformationFactory
{
    public static TooltipInformations GetUsableCompetenceTooltip(Competence competence, RectTransform forcedPosition, TooltipForcedPositionType tooltipForcedPositionType)
    {
        if (competence == null)
            return new TooltipInformations();

        TooltipInformations infos = new TooltipInformations();

        infos.tooltipName = competence.GetCompetenceName;
        infos.tooltipDescription = competence.GetCompetenceTooltip;
        infos.tooltipAdditionalInformationType = TooltipAdditionalInformationType.ActionPointsCost;
        infos.tooltipAdditionalInformationValue = competence.GetActionPointsCost;
        infos.forcedTooltipLPosition = forcedPosition;
        infos.tooltipForcedPositionType = tooltipForcedPositionType;

        return infos;
    }

    public static TooltipInformations GetDiscTypeInformations(DiscInformations discInformations, RectTransform forcedPosition, TooltipForcedPositionType tooltipForcedPositionType)
    {
        if (discInformations == null)
            return new TooltipInformations();

        TooltipInformations infos = new TooltipInformations();

        infos.tooltipName = discInformations.tooltipName;
        infos.tooltipDescription = discInformations.tooltipDescription;
        infos.tooltipAdditionalInformationType = TooltipAdditionalInformationType.Damage;
        infos.tooltipAdditionalInformationValue = discInformations.damagesValue;
        infos.forcedTooltipLPosition = forcedPosition;
        infos.tooltipForcedPositionType = tooltipForcedPositionType;

        return infos;
    }
}
