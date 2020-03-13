using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeColorButton2 : MonoBehaviour
{
    public Color32 ColorOutlineAndTest;
    public GameObject Outline;
    public GameObject Text;

    public void SetColortoSelection()
    {
       
        Outline.GetComponent<Image>().color = ColorOutlineAndTest;
        Text.GetComponent<TextMeshPro>().color = ColorOutlineAndTest;

    }

    public void SetColortoExitSelection()
    {
      
        Outline.GetComponent<Image>().color = Color.white;
        Text.GetComponent<TextMeshPro>().color = Color.white;

    }
}
