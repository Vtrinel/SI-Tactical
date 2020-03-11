using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnButton : MonoBehaviour
{
    public void PastTurn()
    {
        print("fin de tour");

        GameManager.Instance.SelectAction(ActionType.None);
        TurnManager.Instance.EndPlayerTurn();
        GameManager.Instance.SetOnMouseInUI(false);
        CameraManager.instance.GetPlayerCamera.ResetPlayerCamera();
    }

}
