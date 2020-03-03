using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerCompetenceSystem
{
    [SerializeField] CompetenceThrow throwCompetence = default;
    public CompetenceThrow GetCompetenceThrow => throwCompetence;

    [SerializeField] CompetenceRecall recallCompetence = default;
    public CompetenceRecall GetRecallCompetence => recallCompetence;

    CompetenceUsabilityState currentUsabilityState = CompetenceUsabilityState.None;
    public CompetenceUsabilityState GetCurrentUsabilityState => currentUsabilityState;
    CompetenceType currentCompetenceType = CompetenceType.None;
    public CompetenceType GetCurrentCompetenceType => currentCompetenceType;

    public void ChangeUsabilityState(CompetenceUsabilityState usabilityState, CompetenceType compType)
    {
        currentUsabilityState = usabilityState;
        currentCompetenceType = compType;
    }

    public void ResetUsabilityState()
    {
        ChangeUsabilityState(CompetenceUsabilityState.None, CompetenceType.None);
    }

    public bool IsUsingCompetenceSystem => currentUsabilityState != CompetenceUsabilityState.None;

    public void InterruptPreparation()
    {
        ResetUsabilityState();
    }

    public void LaunchThrowCompetence(Vector3 targetPosition)
    {
        Debug.Log("Throw knife at position " + targetPosition);
        ResetUsabilityState();
    }

    public void LaunchRecallCompetence(Vector3 playerPosition)
    {
        Debug.Log("Recall knife at position " + playerPosition);
        ResetUsabilityState();
    }
}

public enum CompetenceUsabilityState
{
    None, Preparing, Using
}

public enum CompetenceType
{
    None, Throw, Recall
}