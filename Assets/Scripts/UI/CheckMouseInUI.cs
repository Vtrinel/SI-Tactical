using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMouseInUI : MonoBehaviour
{
    public void SetValueToGameManager(bool value)
    {
        GameManager.Instance.SetOnMouseInUI(value);
    }
}
