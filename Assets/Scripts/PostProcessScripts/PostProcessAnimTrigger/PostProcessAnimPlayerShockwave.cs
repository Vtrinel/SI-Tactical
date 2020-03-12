using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessAnimPlayerShockwave : PostProcessAnimTrigger
{
    [Header("Settings")]
    [SerializeField] float startDelay = 3;

    private void OnEnable()
    {
        GameManager.Instance.GetPlayer.OnPlayerReceivedDamages += StartShockwave;
    }

    private void OnDisable()
    {
        GameManager.Instance.GetPlayer.OnPlayerReceivedDamages -= StartShockwave;
    }

    public void StartShockwave(bool isDead)
    {
        if (isDead) return;
        StartCoroutine(DelayedAnimation());
    }

    IEnumerator DelayedAnimation()
    {
        yield return new WaitForSeconds(startDelay);
        PlayPostProcessAnim();
    }


}
