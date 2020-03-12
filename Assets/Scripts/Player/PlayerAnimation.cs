using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PlayerAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Animator playerAnimator;
    PlayerController _player;

    [Header("Parameter Name")]
    [SerializeField] string throwTriggerParameter = "Throw";
    [SerializeField] string callbackTriggerParameter = "Callback";
    [SerializeField] string specialTriggerParameter = "Special";
    [SerializeField] string isMovingBoolParameter = "IsMoving";
    [SerializeField] string damagedTriggerParameter = "Damaged";
    [SerializeField] string rageTriggerParameter = "Rage";
    [SerializeField] string deathTriggerParameter = "Dead";

    [Header("Debug")]
    [SerializeField] bool debugMode;
    [SerializeField] bool isMovingDebug;

    private void OnEnable()
    {
        GameManager.Instance.GetCompetencesUsabilityManager().OnDiscThrownAnimEvent += Throw;
        GameManager.Instance.GetCompetencesUsabilityManager().OnDiscRecallAnimEvent += Callback;
        GameManager.Instance.GetCompetencesUsabilityManager().OnSpecialLaunch += LaunchSpecial;
        GameManager.Instance.GetPlayer.OnPlayerReceivedDamages += Damaged;
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
        GameManager.Instance.GetPlayer.OnPlayerReceivedDamages -= Damaged;
        GameManager.Instance.GetPlayer.OnMoveChange -= SetMovement;

    }

    private void Start()
    {
        _player = GameManager.Instance.GetPlayer;
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

    public void Damaged(bool dead)
    {
        playerAnimator.SetTrigger(damagedTriggerParameter);
        playerAnimator.SetTrigger(dead ? deathTriggerParameter : rageTriggerParameter);
    }

    #region Anim Events
    public void LaunchCompetenceForReal()
    {
        GameManager.Instance.LaunchCompetenceForReal();
    }

    public void PlayRageFx()
    {
        Debug.Log("PLAY RAGE FX");
    }

    public void CreateRageZone()
    {
        _player.PlayRage();
    }

    public void EndRageAnimation()
    {
        TurnManager.Instance.EndPlayerRage();
    }
    #endregion
}
