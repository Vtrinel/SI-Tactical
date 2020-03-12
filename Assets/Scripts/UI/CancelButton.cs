using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelButton : MonoBehaviour
{
    public void CancelCurrentAction()
    {
        GameManager.Instance.SelectAction(ActionType.None);
    }

}
