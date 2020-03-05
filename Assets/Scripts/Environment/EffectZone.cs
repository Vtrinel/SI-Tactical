using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectZone : MonoBehaviour
{
    [Header("Dimensions")]
    [SerializeField] float startRadius = 0f;
    [SerializeField] float endRadius = 5f;
    TimerSystem growingDurationSystem = new TimerSystem();
    TimerSystem persistanceDurationSystem = new TimerSystem();

    [Header("Debug")]
    [SerializeField] MeshRenderer debugZoneRenderer = default;

    public void SetUp()
    {

    }

    public void StartZone()
    {
        growingDurationSystem.StartTimer();
    }

    private void Update()
    {
        if (!growingDurationSystem.TimerOver)
        {

        }
    }

    public void DestroyZone()
    {
        Destroy(gameObject);
    }
}
