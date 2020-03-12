using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColorButton1 : MonoBehaviour
{
    public Color32 ColorOutlineAndTest;
    public GameObject Outline;
    public GameObject Text;
    public GameObject Cross;

    public void SetColortoSelection()
    {
       
        Outline.GetComponent<Image>().color = ColorOutlineAndTest;
        Text.GetComponent<Text>().color = ColorOutlineAndTest;
        Cross.GetComponent<Image>().color = ColorOutlineAndTest;
    }

    public void SetColortoExitSelection()
    {
      
        Outline.GetComponent<Image>().color = Color.white;
        Text.GetComponent<Text>().color = Color.white;
        Cross.GetComponent<Image>().color = Color.white;
    }
}
