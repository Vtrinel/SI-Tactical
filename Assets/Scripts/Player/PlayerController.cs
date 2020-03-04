using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private void Start()
    {
        damageReceiptionSystem.SetUpSystem();
        damageReceiptionSystem.OnCurrentLifeAmountChanged += DebugLifeAmount;
        damageReceiptionSystem.OnLifeReachedZero += LifeReachedZero;
    }

    private void Update()
    {
        if (moving)
            CheckForDestinationReached();

        if (ableToAct)
            UpdateInputs();

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            damageReceiptionSystem.LoseLife(1);
        }
        else if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            damageReceiptionSystem.RegainLife(1);
        }
    }

    [Header("References")]
    [SerializeField] NavMeshAgent navMeshAgent = default;
    [SerializeField] DamageableEntity damageReceiptionSystem = default;
    #region Life
    public void LifeReachedZero()
    {
        Debug.Log("DEAD");
    }

    public void DebugLifeAmount(int amount, int delta)
    {
        Debug.Log("Current life : " + amount);
    }
    #endregion

    #region Inputs
    [Header("Inputs")]
    [SerializeField] KeyCode clickActionKey = KeyCode.Mouse0;
    [SerializeField] KeyCode selectMoveInput = KeyCode.A;
    [SerializeField] KeyCode throwCompetenceInput = KeyCode.Z;
    [SerializeField] KeyCode recallCompetenceInput = KeyCode.E;
    [SerializeField] KeyCode specialCompetenceInput = KeyCode.R;

    bool ableToAct = false;
    public void SetAbleToAct(bool able)
    {
        ableToAct = able;
    }

    public void UpdateInputs()
    {
        #region Action Selection
        if (Input.GetKeyDown(selectMoveInput))
            GameManager.Instance.SelectAction(ActionType.Move);
        else if (Input.GetKeyDown(throwCompetenceInput))
            GameManager.Instance.SelectAction(ActionType.Throw);
        else if (Input.GetKeyDown(recallCompetenceInput))
            GameManager.Instance.SelectAction(ActionType.Recall);
        else if (Input.GetKeyDown(specialCompetenceInput))
            GameManager.Instance.SelectAction(ActionType.Special);
        #endregion

        if (Input.GetKeyDown(clickActionKey))
            GameManager.Instance.OnPlayerClickAction();
    }
    #endregion

    #region Movement
    bool moving;
    public void MoveTo(Vector3 targetPosition)
    {
        navMeshAgent.SetDestination(targetPosition);
        moving = true;
    }
    public void CheckForDestinationReached()
    {
        if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && navMeshAgent.remainingDistance == 0)
        {
            moving = false;
            OnPlayerReachedMovementDestination?.Invoke();
        }
    }
    public System.Action OnPlayerReachedMovementDestination;
    #endregion    
}
