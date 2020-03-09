﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscManager : MonoBehaviour
{
    [SerializeField] DiscType testDiscType = DiscType.Piercing;

    public static float discHeight = 1f;

    public float rangeOfPlayer = 5;
    public void AddOneMaxRangeOfPlayer() {
        rangeOfPlayer += 1f;
    }

    Transform player;

    [SerializeField] bool showDebugGizmo = false;

    private static DiscManager _instance;
    public static DiscManager Instance { get { return _instance; } }

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

        #region TEST
        DiscScript[] alreadyInGameDiscs = FindObjectsOfType<DiscScript>();
        foreach(DiscScript disc in alreadyInGameDiscs)
        {
            inGameDiscs.Add(disc);
        }
        #endregion
    }

    private void Start()
    {
        player = GameManager.Instance.GetPlayer.transform;
        FillPossessedDiscsWithBasicDiscs();
    }

    private void OnDrawGizmos()
    {
        if (!player) return;
        if (!showDebugGizmo) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.transform.position, rangeOfPlayer);
        Gizmos.color = new Color(Color.red.r, Color.red.g, Color.red.b, 0.35f);
        Gizmos.DrawSphere(player.transform.position, rangeOfPlayer);
    }

    #region Pooling
    [Header("Pooling")]
    [SerializeField] List<DiscPoolParameters> allDiscPoolParameters = new List<DiscPoolParameters>();
    [SerializeField] Transform poolsParent = default;
    Dictionary<DiscType, Queue<DiscScript>> allDiscPools = new Dictionary<DiscType, Queue<DiscScript>>();
    Dictionary<DiscType, DiscScript> discTypeToPrefab = new Dictionary<DiscType, DiscScript>();
    Dictionary<DiscType, Transform> discTypeToPoolParent = new Dictionary<DiscType, Transform>();

    List<DiscScript> inGameDiscs = new List<DiscScript>();
    public List<DiscScript> GetAllInGameDiscs => inGameDiscs;
    List<DiscScript> throwedDiscs = new List<DiscScript>();
    public List<DiscScript> GetAllThrowedDiscs => throwedDiscs;

    public void SetUpPools()
    {
        allDiscPools = new Dictionary<DiscType, Queue<DiscScript>>();
        discTypeToPrefab = new Dictionary<DiscType, DiscScript>();
        discTypeToPoolParent = new Dictionary<DiscType, Transform>();

        DiscScript newDisc = null;
        Queue<DiscScript> newDiscQueue = new Queue<DiscScript>();
        Transform newPoolParent = null;

        foreach (DiscPoolParameters discPoolParameters in allDiscPoolParameters)
        {
            if (!allDiscPools.ContainsKey(discPoolParameters.discType))
            {
                newPoolParent = new GameObject().transform;
                newPoolParent.SetParent(poolsParent);
                newPoolParent.name = discPoolParameters.discType + "DiscsPool";
                newPoolParent.transform.localPosition = new Vector3();

                discTypeToPrefab.Add(discPoolParameters.discType, discPoolParameters.discPrefab);
                discTypeToPoolParent.Add(discPoolParameters.discType, newPoolParent);

                newDiscQueue = new Queue<DiscScript>();
                for (int i = 0; i < discPoolParameters.baseNumberOfElements; i++)
                {
                    newDisc = Instantiate(discPoolParameters.discPrefab, newPoolParent);
                    newDisc.SetUpModifiers();
                    newDisc.gameObject.SetActive(false);
                    newDisc.SetDiscType(discPoolParameters.discType);
                    newDiscQueue.Enqueue(newDisc);
                }

                allDiscPools.Add(discPoolParameters.discType, newDiscQueue);
            }
        }
    }

    public DiscScript GetDiscFromPool(DiscType discType)
    {
        if (allDiscPools.ContainsKey(discType))
        {
            DiscScript newDisc = null;

            if (allDiscPools[discType].Count > 0)
            {
                newDisc = allDiscPools[discType].Dequeue();
                newDisc.gameObject.SetActive(true);
            }
            else
            {
                newDisc = Instantiate(discTypeToPrefab[discType], discTypeToPoolParent[discType]);
                newDisc.SetUpModifiers();
                newDisc.SetDiscType(discType);
                newDisc.gameObject.SetActive(true);
            }

            inGameDiscs.Add(newDisc);

            return newDisc;
        }

        return null;
    }

    public void ReturnDiscInPool(DiscScript disc)
    {
        DiscType discType = disc.GetDiscType;
        disc.gameObject.SetActive(false);

        inGameDiscs.Remove(disc);
        if (allDiscPools.ContainsKey(discType))
            allDiscPools[discType].Enqueue(disc);
        else
            Destroy(disc.gameObject);
    }

    public void DestroyDisc(DiscScript disc)
    {
        if (throwedDiscs.Contains(disc))
            throwedDiscs.Remove(disc);

        ReturnDiscInPool(disc);
    }
    #endregion

    #region Possessed Discs
    [Header("Stock system")]
    [SerializeField] int maxNumberOfPossessedDiscs = 3;
    Stack<DiscType> possessedDiscs = new Stack<DiscType>();
    public int GetPossessedDiscsCount => possessedDiscs.Count;

    public Action OnAddOneMaxDisc;
    public void AddOneMaxNumberOfPossessedDiscs() 
    { 
        maxNumberOfPossessedDiscs++;

        possessedDiscs.Push(DiscType.Piercing);
        OnAddOneMaxDisc?.Invoke();
    }

    public Action<int> OnDiscFilled;
    public void FillPossessedDiscsWithBasicDiscs()
    {
        for(int i =0; i < maxNumberOfPossessedDiscs; i++)
            possessedDiscs.Push(testDiscType);

        OnDiscFilled?.Invoke(maxNumberOfPossessedDiscs);
    }

    public Action<DiscScript> OnDiscAdded;
    public void PlayerRetreiveDisc(DiscScript retreivedDisc)
    {
        throwedDiscs.Remove(retreivedDisc);
        ReturnDiscInPool(retreivedDisc);
        if (possessedDiscs.Count < maxNumberOfPossessedDiscs)
        {
            possessedDiscs.Push(retreivedDisc.GetDiscType);
            OnDiscAdded?.Invoke(retreivedDisc);
        }
        else
        {
            DiscType retreivedDiscType = retreivedDisc.GetDiscType;
            DiscOverload(retreivedDiscType);
            //Debug.Log("TOO MUCH DISCS, NOT ADDED BUT SUPPOSED TO BE SOMETHING");
        }
    }

    [Header("Overlaod Effects")]
    [SerializeField] int discOverloadPiercingGainedExperience = 2;
    [SerializeField] int discOverloadGhostGainedActionPoints = 2;
    [SerializeField] EffectZoneType discOverloadExplosiveEffectZoneType = EffectZoneType.ExplosiveDiscOverload;
    [SerializeField] int discOverloadHeavyGainedHP = 2;
    [SerializeField] EffectZoneType discOverloadShockwaveEffectZoneType = EffectZoneType.ShockwaveDiscOverload;
    public void DiscOverload(DiscType overloadType)
    {
        switch (overloadType)
        {
            case DiscType.Piercing:
                PlayerExperienceManager.Instance.GainExperience(discOverloadPiercingGainedExperience);
                break;

            case DiscType.Ghost:
                GameManager.Instance.GainActionPoints(discOverloadGhostGainedActionPoints);
                break;

            case DiscType.Explosive:
                EffectZone explosiveEffectZone = EffectZonesManager.Instance.GetEffectZoneFromPool(discOverloadExplosiveEffectZoneType);
                if(explosiveEffectZone != null)
                {
                    explosiveEffectZone.StartZone(player.transform.position);
                }
                break;

            case DiscType.Heavy:
                GameManager.Instance.GetPlayer.damageReceiptionSystem.RegainLife(discOverloadHeavyGainedHP);
                break;

            case DiscType.Shockwave:
                EffectZone shockwaveEffectZone = EffectZonesManager.Instance.GetEffectZoneFromPool(discOverloadShockwaveEffectZoneType);
                if (shockwaveEffectZone != null)
                {
                    shockwaveEffectZone.StartZone(player.transform.position);
                }
                break;
        }
    }

    public Action OnDiscConsommed;

    public DiscScript TakeFirstDiscFromPossessedDiscs()
    {
        if (possessedDiscs.Count == 0)
            return null;

        DiscScript newDisc = GetDiscFromPool(possessedDiscs.Pop());
        if (newDisc != null)
        {
            throwedDiscs.Add(newDisc);
            Debug.Log("disc throwed");
            OnDiscConsommed?.Invoke();
        }
        return newDisc;
    }
    #endregion

    #region Proximity

    public void CheckAllDiscsProximity(Vector3 playerPosition)
    {
        foreach (DiscScript disc in inGameDiscs)
        {
            disc.SetIsInRange(DiscIsInRange(playerPosition, disc.transform.position));
        }
    }

    public bool DiscIsInRange(Vector3 playerPos, Vector3 discPos)
    {
        playerPos.y = discPos.y;
        return Vector3.Distance(playerPos, discPos) <= rangeOfPlayer;
    }

    public List<DiscScript> GetAllInRangeDiscsFromPosition(Vector3 position)
    {
        List<DiscScript> inRangeFromPos = new List<DiscScript>();

        foreach (DiscScript disc in inGameDiscs)
        {
            if (DiscIsInRange(position, disc.transform.position))
                inRangeFromPos.Add(disc);
        }

        return inRangeFromPos;
    }

    public int GetInRangeDiscsCount
    {
        get
        {
            int counter = 0;

            foreach (DiscScript disc in inGameDiscs)
            {
                if (disc.IsInRange)
                    counter++;
            }

            return counter;
        }
    }

    public List<DiscScript> GetInRangeDiscs
    {
        get
        {
            List<DiscScript> list = new List<DiscScript>();

            foreach (DiscScript disc in inGameDiscs)
            {
                if (disc.IsInRange)
                    list.Add(disc);
            }

            return list;
        }
    }
    #endregion    
}

public enum DiscType
{
    None, Piercing, Ghost, Explosive, Heavy, Shockwave
}

[System.Serializable]
public struct DiscPoolParameters
{
    public DiscType discType;
    public DiscScript discPrefab;
    public int baseNumberOfElements;
}