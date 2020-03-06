using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMenuManager : MonoBehaviour
{
    public Animator AnimatorSelf;

    private void Start()
    {
        AnimatorSelf = GetComponent<Animator>();
    }

    public void MenuStep1()
    {
        AnimatorSelf.SetInteger("Step", 1);
    }

    public void MenuStep2()
    {
        AnimatorSelf.SetInteger("Step", 2);
    }

    public void MenuReturnStep0()
    {
        AnimatorSelf.SetInteger("Step", 0);
    }

}
