using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ChangeScale2 : MonoBehaviour
{




    public void SetScaletoSelection()
    {
     
            transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    public void SetScaletoExitSelection()
    { 
    
            transform.localScale = new Vector3(1, 1, 1);
        
    }

}
