using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class CollisionDebug : MonoBehaviour
{
    [Button]
    public void EnableRenderers()
    {
        SetRenderers(true);
    }

    [Button]
    public void DisableRederers()
    {
        SetRenderers(false);
    }
    
    void SetRenderers(bool p_enabled)
    {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].enabled = p_enabled;
        }
    }
}
