using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscManager : MonoBehaviour
{
    [SerializeField] DiscType testDiscType = DiscType.Piercing;

    public static float discHeight = 1f;

    public float recallRange = 12;
    public float throwRange = 9;
    public void AddOneMaxRangeOfPlayer() 
    {
        recallRange++;
        throwRange++;
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
        Gizmos.DrawWireSphere(player.transform.position, recallRange);
        Gizmos.color = new Color(Color.red.r, Color.red.g, Color.red.b, 0.35f);
        Gizmos.DrawSphere(player.transform.position, recallRange);
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

    public DiscScript PeekNextThrowDisc()
    {
        if (possessedDiscs.Count > 0)
            return PeekDiscFromPool(possessedDiscs.Peek());
        else
            return null;
    }

    public DiscScript PeekDiscFromPool(DiscType discType)
    {
        if (allDiscPools.ContainsKey(discType))
        {
            DiscScript peekedDisc = null;

            if (allDiscPools[discType].Count > 0)
            {
                peekedDisc = allDiscPools[discType].Peek();
            }
            else
            {
                peekedDisc = Instantiate(discTypeToPrefab[discType], discTypeToPoolParent[discType]);
                peekedDisc.SetUpModifiers();
                peekedDisc.SetDiscType(discType);
                peekedDisc.gameObject.SetActive(false);
                allDiscPools[discType].Enqueue(peekedDisc);
            }

            return peekedDisc;
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
        {
            FxManager.Instance.CreateFx(FxType.discDestroyed, disc.transform.position);
            throwedDiscs.Remove(disc);
        }

        ReturnDiscInPool(disc);
    }
    #endregion

    #region Possessed Discs
    [Header("Stock system")]
    [SerializeField] int maxNumberOfPossessedDiscs = 3;
    [SerializeField] int currentPossessedDiscs = 3;
    Stack<DiscType> possessedDiscs = new Stack<DiscType>();
    public int GetPossessedDiscsCount => possessedDiscs.Count;
    public Stack<DiscType> GetPossessedDiscs => possessedDiscs;

    public Action OnAddOneMaxDisc;
    public void AddOneMaxNumberOfPossessedDiscs() 
    { 
        maxNumberOfPossessedDiscs++;

        possessedDiscs.Push(DiscType.Piercing);
        OnAddOneMaxDisc?.Invoke();
    }

    public Action<Stack<DiscType>> OnDiscUpdate;

    //int total, combien, type
    public Action<int, int, DiscType> OnDiscFilled;
    public void FillPossessedDiscsWithBasicDiscs()
    {
                        // maxNumberOfPossessedDiscs normalement
        for (int i =0; i < currentPossessedDiscs; i++) 
            possessedDiscs.Push(testDiscType);

        OnDiscFilled?.Invoke(maxNumberOfPossessedDiscs, currentPossessedDiscs, testDiscType);
        OnDiscUpdate?.Invoke(possessedDiscs);
    }

    public Action<DiscScript> OnDiscAdded;
    public void PlayerRetreiveDisc(DiscScript retreivedDisc)
    {
        SoundManager.Instance.PlaySound(Sound.RecallDisc, player.position);
        //FxManager.Instance.CreateFx(FxType.discRecall, retreivedDisc.transform.position);


        throwedDiscs.Remove(retreivedDisc);
        ReturnDiscInPool(retreivedDisc);
        if (possessedDiscs.Count < maxNumberOfPossessedDiscs)
        {
            possessedDiscs.Push(retreivedDisc.GetDiscType);
            OnDiscAdded?.Invoke(retreivedDisc);
            OnDiscUpdate?.Invoke(possessedDiscs);
        }
        else
        {
            DiscType retreivedDiscType = retreivedDisc.GetDiscType;
            DiscOverload(retreivedDiscType);
            //Debug.Log("TOO MUCH DISCS, NOT ADDED BUT SUPPOSED TO BE SOMETHING");
        }

        GameManager.Instance.CheckForCompetencesUsability();
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
                PlayerExperienceManager.Instance.GainGold(discOverloadPiercingGainedExperience);
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

        DiscType newDiscType = possessedDiscs.Pop();
        DiscScript newDisc = GetDiscFromPool(newDiscType);
        if (newDisc != null)
        {
            throwedDiscs.Add(newDisc);
            OnDiscConsommed?.Invoke();
            OnDiscUpdate?.Invoke(possessedDiscs);

            SoundManager.Instance.PlaySound(Sound.ThrowDisc, newDisc.transform.position);

            FxManager.Instance.CreateFx(FxType.discThrow, GameManager.Instance.GetPlayer.transform.position);
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
        return Vector3.Distance(playerPos, discPos) <= recallRange;
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

    [Header("Discs Informations")]
    [SerializeField] DiscsInformationsLibrary discsInformationsLibrary = default;

    public DiscInformations GetDiscInformations(DiscType discType)
    {
        return discsInformationsLibrary.GetDiscInformations(discType);
    }
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