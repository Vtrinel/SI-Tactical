using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementGhostPreview : MonoBehaviour
{
    [SerializeField] Renderer ghostRenderer = default;
    public Renderer GetRenderer => ghostRenderer;
    public void Awake()
    {
        if(ghostRenderer == null)
        {
            ghostRenderer = GetComponentInChildren<Renderer>();
        }
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
