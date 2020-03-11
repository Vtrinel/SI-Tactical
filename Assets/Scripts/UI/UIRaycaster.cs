using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIRaycaster : MonoBehaviour
{
    [SerializeField] GraphicRaycaster m_Raycaster = default;
    [SerializeField] EventSystem m_EventSystem = default;
    PointerEventData m_PointerEventData;

    public ITooltipable CheckForUITooltipable()
    {
        ITooltipable foundTooltipable = null;

        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);

        foreach (RaycastResult result in results)
        {
            foundTooltipable = result.gameObject.GetComponent<ITooltipable>();
            if (foundTooltipable != null)
                break;
        }

        return foundTooltipable;
    }
}
