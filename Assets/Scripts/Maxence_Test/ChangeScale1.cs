using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ChangeScale1 : MonoBehaviour
{
  
    public Color32 ColorOutlineAndTest;
    public GameObject Outline;
    public GameObject Select;
    public bool Actived;

    public void ActivedButton()
    {
        Select.SetActive(true);
        GetComponent<Image>().color = ColorOutlineAndTest;
        transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
        Actived = true;
    }

    public void NoActivedButton()
    {
        Select.SetActive(false);
        GetComponent<Image>().color = Color.white;
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        Actived = false;
    }

    public void SetScaletoSelection()
    {
      
        if (Actived == false)
        {
            GetComponent<Image>().color = ColorOutlineAndTest;
            transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
        }
          
    }

    public void SetScaletoExitSelection()
    {
      
        if (Actived == false)
        {
            GetComponent<Image>().color = Color.white;
            transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
           
        
    }

}
