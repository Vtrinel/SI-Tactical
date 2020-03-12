using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFXOnEnable : MonoBehaviour
{
    [SerializeField] FxType fxToPlayType = FxType.Explosion;
    private void OnEnable()
    {
        PlayFxAtPosition(transform.position);
    }

    public void PlayFxAtPosition(Vector3 position)
    {

    }
}
