using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ChangeScale3 : MonoBehaviour
{

    public Vector3 scaleBase;
    public Vector3 GrosScale;

    public void SetScaletoSelection()
    {

        transform.localScale = GrosScale;
    }

    public void SetScaletoExitSelection()
    { 
    
            transform.localScale = scaleBase;
        
    }

}
