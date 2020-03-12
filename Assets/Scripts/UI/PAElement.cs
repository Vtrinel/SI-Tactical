using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAElement : MonoBehaviour
{
    [SerializeField] GameObject imageLife;
    [SerializeField] Animator PA_Animator;

    public void SetValue(bool value)
    {
        imageLife.SetActive(value);
    }

    public void restartStepAnimator()
    {
        if(PA_Animator != null)
        {
            PA_Animator.SetInteger("PaBarStep", 0);
        }
    }
}
