using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAElement : MonoBehaviour
{
    [SerializeField] GameObject imageLife;
    public Animator PA_Animator;

    public bool statut = true;

    public void SetValue(bool value)
    {
        imageLife.SetActive(value);

        if (statut != value)
        {
            if (value)
            {
                PA_Animator.SetTrigger("Heal");
                PA_Animator.SetBool("InPreview", false);
            }
            else
            {
                PA_Animator.SetTrigger("Use");
            }
        }

        statut = value;
    }

    public void restartStepAnimator()
    {
        if(PA_Animator != null)
        {
            PA_Animator.SetInteger("PaBarStep", 0);

        }
    }
}
