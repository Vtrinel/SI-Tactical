using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PlayerAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Animator playerAnimator;

    [Header("Parameter Name")]
    [SerializeField] string throwTriggerParameter = "Throw";
    [SerializeField] string callbackTriggerParameter = "Callback";
    [SerializeField] string specialTriggerParameter = "Special";
    [SerializeField] string isMovingBoolParameter = "IsMoving";
    [SerializeField] string damagedTriggerParameter = "Damaged";

    [Header("Debug")]
    [SerializeField] bool debugMode;
    [SerializeField] bool isMovingDebug;

    private void OnEnable()
    {
        GameManager.Instance.GetCompetencesUsabilityManager().OnDiscThrownAnimEvent += Throw;
        GameManager.Instance.GetCompetencesUsabilityManager().OnDiscRecallAnimEvent += Callback;
        GameManager.Instance.GetCompetencesUsabilityManager().OnSpecialLaunch += LaunchSpecial;
        GameManager.Instance.GetPlayer.OnMoveChange += SetMovement;

        if(playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
            Debug.LogWarning("ANIMATOR NOT SET IN PLAYER ANIMATION");
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.GetCompetencesUsabilityManager().OnDiscThrownAnimEvent -= Throw;
        GameManager.Instance.GetCompetencesUsabilityManager().OnDiscRecallAnimEvent -= Callback;
        GameManager.Instance.GetCompetencesUsabilityManager().OnSpecialLaunch -= LaunchSpecial;
        GameManager.Instance.GetPlayer.OnMoveChange -= SetMovement;

    }

    private void Update()
    {
        if (debugMode)
        {
            SetMovement(isMovingDebug);
        }
    }

    public void SetMovement(bool p_isMoving)
    {
        playerAnimator.SetBool(isMovingBoolParameter,p_isMoving);
    }

    [Button]
    public void Throw()
    {
        playerAnimator.SetTrigger(throwTriggerParameter);
    }

    [Button]
    public void Callback()
    {
        playerAnimator.SetTrigger(callbackTriggerParameter);
    }

    [Button]
    public void LaunchSpecial()
    {
        playerAnimator.SetTrigger(specialTriggerParameter);
    }

    [Button]
    public void Damaged()
    {
        playerAnimator.SetTrigger(damagedTriggerParameter);
    }

    public void LaunchCompetenceForReal()
    {
        GameManager.Instance.LaunchCompetenceForReal();
    }
}
