using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    public GameObject PauseMenu;

    public void Pause()
    {
        print("Jeu en pause");

        GameManager.Instance.SelectAction(ActionType.None);
        TimeManager.Instance.Pause();
        CameraManager.instance.GetPlayerCamera.ResetPlayerCamera();
        PauseMenu.SetActive(true);
    }
}
