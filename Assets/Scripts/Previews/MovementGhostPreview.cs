using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementGhostPreview : MonoBehaviour
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
