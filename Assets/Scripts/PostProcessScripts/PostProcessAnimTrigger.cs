using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessAnimTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Animator postProcessAnimator;
    [SerializeField] protected string animTriggerName = "PlayerDamaged";

    protected void PlayPostProcessAnim()
    {
        postProcessAnimator.SetTrigger(animTriggerName);
    }
}
