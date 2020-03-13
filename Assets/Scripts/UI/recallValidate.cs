using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class recallValidate : MonoBehaviour
{
    [SerializeField] GameObject parentObj;


    private void OnEnable()
    {
        GameManager.Instance.OnRecallCompetenceSelectionStateChanged += ShowOrHide;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnRecallCompetenceSelectionStateChanged -= ShowOrHide;
    }

    void ShowOrHide(bool value)
    {
        parentObj.SetActive(value);
    }

    public void Validate()
    {
        GameManager.Instance.OnPlayerClickAction();
    }
}
