using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectZonesManager : MonoBehaviour
{
    private static EffectZonesManager _instance;
    public static EffectZonesManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        SetUpPools();

    }


    [Header("Pooling")]
    [SerializeField] List<EffectZonePoolParameters> allEffectZonePoolParameters = new List<EffectZonePoolParameters>();
    [SerializeField] Transform poolsParent = default;
    Dictionary<EffectZoneType, Queue<EffectZone>> allEffectZonePools = new Dictionary<EffectZoneType, Queue<EffectZone>>();
    Dictionary<EffectZoneType, EffectZone> effectZoneTypeToPrefab = new Dictionary<EffectZoneType, EffectZone>();
    Dictionary<EffectZoneType, Transform> effectZoneTypeToPoolParent = new Dictionary<EffectZoneType, Transform>();

    public void SetUpPools()
    {
        allEffectZonePools = new Dictionary<EffectZoneType, Queue<EffectZone>>();
        effectZoneTypeToPrefab = new Dictionary<EffectZoneType, EffectZone>();
        effectZoneTypeToPoolParent = new Dictionary<EffectZoneType, Transform>();

        EffectZone newEffectZone = null;
        Queue<EffectZone> newEffectZoneQueue = new Queue<EffectZone>();
        Transform newPoolParent = null;

        foreach (EffectZonePoolParameters effectZonePoolParameters in allEffectZonePoolParameters)
        {
            if (!allEffectZonePools.ContainsKey(effectZonePoolParameters.effectZoneType))
            {
                newPoolParent = new GameObject().transform;
                newPoolParent.SetParent(poolsParent);
                newPoolParent.name = effectZonePoolParameters.effectZoneType + "Pool";
                newPoolParent.transform.localPosition = new Vector3();

                effectZoneTypeToPrefab.Add(effectZonePoolParameters.effectZoneType, effectZonePoolParameters.effectZonePrefab);
                effectZoneTypeToPoolParent.Add(effectZonePoolParameters.effectZoneType, newPoolParent);

                newEffectZoneQueue = new Queue<EffectZone>();
                for (int i = 0; i < effectZonePoolParameters.baseNumberOfElements; i++)
                {
                    newEffectZone = Instantiate(effectZonePoolParameters.effectZonePrefab, poolsParent);
                    newEffectZone.gameObject.SetActive(false);
                    newEffectZone.transform.SetParent(newPoolParent);
                    newEffectZone.SetEffectZoneType(effectZonePoolParameters.effectZoneType);
                    newEffectZoneQueue.Enqueue(newEffectZone);
                }

                allEffectZonePools.Add(effectZonePoolParameters.effectZoneType, newEffectZoneQueue);
            }
        }
    }

    public EffectZone GetEffectZoneFromPool(EffectZoneType effectZoneType)
    {
        if (allEffectZonePools.ContainsKey(effectZoneType))
        {
            EffectZone newEffectZone = null;

            if (allEffectZonePools[effectZoneType].Count > 0)
            {
                newEffectZone = allEffectZonePools[effectZoneType].Dequeue();
                newEffectZone.gameObject.SetActive(true);
            }
            else
            {
                newEffectZone = Instantiate(effectZoneTypeToPrefab[effectZoneType], effectZoneTypeToPoolParent[effectZoneType]);
                newEffectZone.gameObject.SetActive(true);
            }

            return newEffectZone;
        }

        return null;
    }

    public void ReturnEffectZoneInPool(EffectZone effectZone)
    {
        EffectZoneType effectZoneType = effectZone.GetEffectZoneType;
        effectZone.gameObject.SetActive(false);

        if (allEffectZonePools.ContainsKey(effectZoneType))
            allEffectZonePools[effectZoneType].Enqueue(effectZone);
        else
            Destroy(effectZone.gameObject);
    }
}

public enum EffectZoneType
{
    None, PlayerRage, ExplosiveDisc, ShockwaveDisc
}

[System.Serializable]
public struct EffectZonePoolParameters
{
    public EffectZoneType effectZoneType;
    public EffectZone effectZonePrefab;
    public int baseNumberOfElements;
}