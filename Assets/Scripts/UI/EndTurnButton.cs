using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnButton : MonoBehaviour
{
    public void PastTurn()
    {
        GameManager.Instance.SelectAction(ActionType.None);
        TurnManager.Instance.EndPlayerTurn();
        CameraManager.instance.GetPlayerCamera.ResetPlayerCamera();
    }

}
