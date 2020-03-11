using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeElement : MonoBehaviour
{
    [SerializeField] GameObject imageLife;
    [SerializeField] Animator animator;

    public void SetValue(bool value)
    {
        //imageLife.SetActive(value);
        animator?.SetBool("IsAlive", value);
    }
}
