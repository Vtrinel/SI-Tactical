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

        navMeshAgent.isStopped = true;
    }

    private void Update()
    {
        if (moving)
            CheckForDestinationReached();

        if (ableToAct)
            UpdateInputs();

        if (Input.GetKeyDown(KeyCode.K))
        {
            Vector3 randomDir = Random.onUnitSphere;
            randomDir.y = 0;
            randomDir.Normalize();

            Debug.DrawRay(transform.position + Vector3.up, randomDir * 3, Color.red);
        }
    }

    [Header("References")]
    [SerializeField] NavMeshAgent navMeshAgent = default;
    [SerializeField] DamageableEntity damageReceiptionSystem = default;
    [SerializeField] KnockbackableEntity knockbackReceiptionSystem = default;

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
    [SerializeField] KeyCode passTurnInput = KeyCode.Return;

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

        if (Input.GetKeyDown(passTurnInput))
        {
            GameManager.Instance.SelectAction(ActionType.None);
            TurnManager.Instance.EndPlayerTurn();
        }
    }
    #endregion

    #region Movement
    bool moving;
    public void MoveTo(Vector3 targetPosition)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(targetPosition);
        moving = true;
    }
    public void CheckForDestinationReached()
    {
        if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && navMeshAgent.remainingDistance == 0)
        {
            navMeshAgent.isStopped = true;
            moving = false;
            OnPlayerReachedMovementDestination?.Invoke();
        }
    }
    public System.Action OnPlayerReachedMovementDestination;
    #endregion    
}
