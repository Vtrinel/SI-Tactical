using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TooltipsManagementSystem 
{
    ITooltipable currentTooltipable;

    public void UpdateSystem(ITooltipable newTooltipable)
    {
        ITooltipable previousTooltipable = currentTooltipable;
        currentTooltipable = newTooltipable;

        if (currentTooltipable == previousTooltipable)
        {
            if(currentTooltipable != null)
            {
                //DebugLogTooltipInfos(currentTooltipable.GetTooltipInformations);
                //Here, update UI tooltip position
            }
        }
        else
        {
            if (previousTooltipable != null)
            {
                previousTooltipable.OnEndTooltip?.Invoke();
            }
            else
            {
                //Here, start UI tooltip apparition
            }

            if (currentTooltipable != null)
            {
                //Here, change the UI tooltip content
                currentTooltipable.OnStartTooltip?.Invoke();
                DebugLogTooltipInfos(currentTooltipable.GetTooltipInformations);
            }
            else
            {
                //Here, start the UI tooltip disparition
            }
        }
    }

    public void DebugLogTooltipInfos(TooltipInformations infos)
    {
        string debugText = infos.tooltipName + " : " +  infos.tooltipDescription;
        switch (infos.tooltipAdditionalInformationType)
        {
            case TooltipAdditionalInformationType.ActionPointsCost:
                debugText += " (Cost : " + infos.tooltipAdditionalInformationValue + "AP)";
                break;
            case TooltipAdditionalInformationType.LifePoints:
                debugText += " (State : " + infos.tooltipAdditionalInformationValue + "HP)";
                break;
            case TooltipAdditionalInformationType.Damage:
                debugText += " (Damages : " + infos.tooltipAdditionalInformationValue + "HP)";
                break;
        }
        Debug.Log(debugText);
    }
}
