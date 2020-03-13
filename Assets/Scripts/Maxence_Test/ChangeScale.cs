using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ChangeScale : MonoBehaviour
{
  
    public Color32 ColorOutlineAndTest;
    public GameObject Text;
    public GameObject Outline;
    public GameObject Icon;
    public bool Actived;

    public void ActivedButton()
    {
     
   
        Icon.SetActive(true);
        Text.GetComponent<TextMeshProUGUI>().color = ColorOutlineAndTest;
        transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
        Actived = true;
    }

    public void NoActivedButton()
    {
      
   
        Icon.SetActive(false);
        Text.GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        transform.localScale = new Vector3(0.9f, 0.9f, 1f);
        Actived = false;
    }

    public void SetScaletoSelection()
    {
        Outline.GetComponent<Image>().color = ColorOutlineAndTest;
        if (Actived == false)
            transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void SetScaletoExitSelection()
    {
        Outline.GetComponent<Image>().color = Color.white;
        if (Actived == false)
            transform.localScale = new Vector3(0.9f, 0.9f, 1f);
        
    }

}
