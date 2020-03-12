using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessAnimPlayerDamaged : PostProcessAnimTrigger
{
    private void OnEnable()
    {
        GameManager.Instance.GetPlayer.OnPlayerReceivedDamages += PlayerDamaged;
    }
    private void OnDisable()
    {
        GameManager.Instance.GetPlayer.OnPlayerReceivedDamages -= PlayerDamaged;
    }

    public void PlayerDamaged(bool dead)
    {
        if (dead) return;
        PlayPostProcessAnim();
    }
}
