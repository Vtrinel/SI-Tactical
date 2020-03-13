using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessAnimTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected PostProcessAnimator postProcessAnimator;

    public void PlayPostProcessAnim()
    {
        postProcessAnimator.StartAnim();
    }
}
