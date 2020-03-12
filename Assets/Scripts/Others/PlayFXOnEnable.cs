using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFXOnEnable : MonoBehaviour
{
    [SerializeField] FxType fxToPlayType = FxType.playerShockwave;
    private void OnEnable()
    {
        PlayFxAtPosition(transform.position);
    }

    public void PlayFxAtPosition(Vector3 position)
    {

    }
}
