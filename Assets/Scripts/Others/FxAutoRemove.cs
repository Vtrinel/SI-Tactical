using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FxManager;

public class FxAutoRemove : MonoBehaviour
{
    [SerializeField] fxType myType;
    [SerializeField] float timeToDisable;

    private void OnEnable()
    {
        StartCoroutine(WaitToDisable());
    }

    IEnumerator WaitToDisable()
    {
        yield return new WaitForSeconds(timeToDisable);

        FxManager.Instance.RemoveThisFx(gameObject, myType);
    }
}
