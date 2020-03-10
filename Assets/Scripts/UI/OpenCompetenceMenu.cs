using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCompetenceMenu : MonoBehaviour
{
    public void ShowCompetenceMenu()
    {
        PlayerExperienceManager.Instance.IsCompetenceInterfaceShowing();
    }
}
