using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColorButton2 : MonoBehaviour
{
    public Color32 ColorOutlineAndTest;
    public GameObject Cross;

    public void SetColortoSelection()
    {

        transform.localScale = new Vector3(1, 1, 1);
        Cross.GetComponent<Image>().color = ColorOutlineAndTest;
    }

    public void SetColortoExitSelection()
    {
        transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        Cross.GetComponent<Image>().color = Color.white;
    }
}
