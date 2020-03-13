using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerActability : MonoBehaviour
{
    public void UpdateActability()
    {
        PlayerExperienceManager.Instance.CloseCompetenceInterface();
    }
}
