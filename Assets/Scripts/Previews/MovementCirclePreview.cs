using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCirclePreview : MonoBehaviour
{
    [SerializeField] Transform zoneRenderer = default;
    public void ChangeRadius(float newRadius)
    {
        zoneRenderer.transform.localScale = Vector3.one * newRadius;
    }

    public void ShowPreview()
    {
        gameObject.SetActive(true);
    }

    public void HidePreview()
    {
        gameObject.SetActive(false);
    }
}
