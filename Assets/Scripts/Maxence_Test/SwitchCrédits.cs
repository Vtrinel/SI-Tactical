using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCrédits : MonoBehaviour
{

    public Animator AnimatorSelf;

    public void SwitchCredit()
    {
        if(AnimatorSelf.GetBool("GetCredits") == true)
        {
            AnimatorSelf.SetBool("GetCredits", false);
        }
        else
        {
            AnimatorSelf.SetBool("GetCredits", true);
        }

     


    }

}
