using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwarmZone : MonoBehaviour
{
    private void OnEnable()
    {
        if (selfActivates)
            GameManager.Instance.OnPlayerPositionChanged += CheckIfPlayerIsInZone;
    }

    private void OnDisable()
    {
        if (selfActivates)
            GameManager.Instance.OnPlayerPositionChanged -= CheckIfPlayerIsInZone;
    }

    private void OnDrawGizmos()
    {
        if (!debugZone)
            return;

        Gizmos.color = new Color(1, 0, 1, 0.4f);
        Gizmos.DrawSphere(transform.position, radius);
    }

    [SerializeField] bool debugZone = true;

    [Header("Parameters")]
    [SerializeField] float radius = 5f;
    [SerializeField] bool selfActivates = false;

    SwarmZoneState currentState = SwarmZoneState.Inactive;

    public void CheckIfPlayerIsInZone(Vector3 playerPos)
    {
        switch (currentState)
        {
            case SwarmZoneState.Inactive:
                if (GetDistanceWithPlayer(playerPos) <= radius)
                    OnPlayerEntersZone();
                break;

            case SwarmZoneState.PlayerInZone:
                if (GetDistanceWithPlayer(playerPos) > radius)
                    OnPlayerExitsZone();
                break;

            case SwarmZoneState.PlayerOutZoneButEnteredOnce:
                if (GetDistanceWithPlayer(playerPos) <= radius)
                    OnPlayerEntersZone();
                break;
        }
    }

    public float GetDistanceWithPlayer(Vector3 playerPos)
    {
        Vector3 selfPos = transform.position;
        playerPos.y = transform.position.y;

        return Vector3.Distance(playerPos, selfPos);
    }

    public void OnPlayerEntersZone()
    {
        if(currentState == SwarmZoneState.Inactive)
        {
            OnPlayerEntersZoneFirstTime();
        }
        currentState = SwarmZoneState.PlayerInZone;
    }

    public void OnPlayerEntersZoneFirstTime()
    {

    }

    public void OnPlayerExitsZone()
    {
        currentState = SwarmZoneState.PlayerOutZoneButEnteredOnce;
    }
}

public enum SwarmZoneState
{
    Inactive, PlayerInZone, PlayerOutZoneButEnteredOnce
}