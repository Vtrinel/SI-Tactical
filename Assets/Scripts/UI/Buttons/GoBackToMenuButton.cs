using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoBackToMenuButton : MonoBehaviour
{
    public void GoToMenu()
    {
        GameManager.Instance.GoBackToMenu();
    }
}
