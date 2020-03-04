using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] NavMeshAgent navMeshAgent = default;

    [Header("Inputs")]
    [SerializeField] KeyCode clickActionKey = KeyCode.Mouse0;
    [SerializeField] KeyCode selectMoveInput = KeyCode.Q;
    [SerializeField] KeyCode throwCompetenceInput = KeyCode.A;
    [SerializeField] KeyCode recallCompetenceInput = KeyCode.Z;

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
        #endregion

        if (Input.GetKeyDown(clickActionKey))
            GameManager.Instance.OnPlayerClickAction();
    }

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
   
    private void Update()
    {
        if (moving)
            CheckForDestinationReached();

        if (ableToAct)
            UpdateInputs();
    }
}
