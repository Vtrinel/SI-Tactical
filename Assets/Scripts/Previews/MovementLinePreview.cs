using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementLinePreview : MonoBehaviour
{
    public void ShowPreview()
    {
        gameObject.SetActive(true);
    }

    public void HidePreview()
    {
        gameObject.SetActive(false);
    }
}
