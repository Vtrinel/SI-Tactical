using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class TooltipsManagementSystem 
{
    [SerializeField] TooltipInterface tooltipInterface = default;
    ITooltipable currentTooltipable;

    public void UpdateSystem(ITooltipable newTooltipable)
    {
        ITooltipable previousTooltipable = currentTooltipable;
        currentTooltipable = newTooltipable;

        if (currentTooltipable == previousTooltipable)
        {
            if(currentTooltipable != null)
            {
                
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
                tooltipInterface.SetTooltipVisibility(true);
            }

            if (currentTooltipable != null)
            {
                currentTooltipable.OnStartTooltip?.Invoke();
                tooltipInterface.SetTooltipInfos(currentTooltipable.GetTooltipInformations);
            }
            else
            {
                tooltipInterface.SetTooltipVisibility(false);
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
