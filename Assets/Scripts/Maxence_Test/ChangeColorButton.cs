using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColorButton : MonoBehaviour
{
    public Color32 ColorOutlineAndTest;
    public GameObject Outline;

    public void SetColortoSelection()
    {
        Debug.Log("test");
        Outline.GetComponent<Image>().color = ColorOutlineAndTest;
    }

    public void SetColortoExitSelection()
    {
        Debug.Log("test1");
        Outline.GetComponent<Image>().color = Color.white;
    }
}
