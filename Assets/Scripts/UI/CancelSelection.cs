using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CancelSelection : MonoBehaviour
{
    [SerializeField] Button cancelButton;

    private void OnEnable()
    {
        GameManager.Instance.OnMoveActionSelectionStateChanged += ChangeStateCancelButton;
        GameManager.Instance.OnThrowCompetenceSelectionStateChanged += ChangeStateCancelButton;
        GameManager.Instance.OnRecallCompetenceSelectionStateChanged += ChangeStateCancelButton;
        GameManager.Instance.OnSpecialCompetenceSelectionStateChanged += ChangeStateCancelButton;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnMoveActionSelectionStateChanged -= ChangeStateCancelButton;
        GameManager.Instance.OnThrowCompetenceSelectionStateChanged -= ChangeStateCancelButton;
        GameManager.Instance.OnRecallCompetenceSelectionStateChanged -= ChangeStateCancelButton;
        GameManager.Instance.OnSpecialCompetenceSelectionStateChanged -= ChangeStateCancelButton;
    }

    public void ChangeStateCancelButton(bool state)
    {
        cancelButton.gameObject.SetActive(state);
    }

    public void CancelCurrentAction()
    {
        GameManager.Instance.SelectAction(ActionType.None);
        Debug.Log("Cancel action");
    }
}
