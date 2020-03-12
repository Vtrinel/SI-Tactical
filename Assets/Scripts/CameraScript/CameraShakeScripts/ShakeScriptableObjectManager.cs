using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using NaughtyAttributes;

public class ShakeScriptableObjectManager : MonoBehaviour
{
    public CameraShake CameraShakeScript;
    public static ShakeScriptableObjectManager instance;
    [Header("ShakeScriptableObjectManager")]
    public List<ShakeSettings> currentShakeSettingsList = new List<ShakeSettings>();
    List<string> NamecurrentShakeSettingsList = new List<string>();

    private void Start()
    {
        instance = this;
        NamecurrentShakeSettingsList = new List<string>(new string[currentShakeSettingsList.Count]);

    }

    private void Update()
    {
        if(currentShakeSettingsList.Count > 0)
        {
            for (int i = 0; i < currentShakeSettingsList.Count; i++)
            {
                NamecurrentShakeSettingsList[i] = currentShakeSettingsList[i].name;
            }
        }
    }

    public void LoadShake (string _currentShakeSettingsName)
    {
        if (NamecurrentShakeSettingsList.Count > 0)
        {
            for (int i = 0; i < NamecurrentShakeSettingsList.Count; i++)
            {
                if (NamecurrentShakeSettingsList[i] == _currentShakeSettingsName)
                {
                    CameraShakeScript.StartShake(currentShakeSettingsList[i]);
                    return;
                }
              
            }

            Debug.Log("Il n'y a pas de scriptable Object équivalent au nom donné à joué");
        }
        else
        {
            Debug.Log("Il n'y a pas de scriptable Object dans la list");
        }


        //  StartShake(_currentShakeSettings);
    }


}
