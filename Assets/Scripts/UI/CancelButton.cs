using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelButton : MonoBehaviour
{
    public void CancelCurrentAction()
    {
        GameManager.Instance.SelectAction(ActionType.None);
    }

    [SerializeField] Animator animController = default;
    public void SetHover(bool hover)
    {
        animController.SetBool("Hover", hover);
    }
}
