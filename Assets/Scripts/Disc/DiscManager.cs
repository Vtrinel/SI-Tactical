using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscManager : MonoBehaviour
{
    public DiscScript prefabDisc = default;

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
                    newDisc = Instantiate(discPoolParameters.discPrefab, poolsParent);
                    newDisc.gameObject.SetActive(false);
                    newDisc.transform.SetParent(newPoolParent);
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
    #endregion

    #region Possessed Discs
    [Header("Stock system")]
    [SerializeField] int maxNumberOfPossessedDiscs = 3;
    Stack<DiscType> possessedDiscs = new Stack<DiscType>();
    public int GetPossessedDiscsCount => possessedDiscs.Count;
    public void AddOneMaxNumberOfPossessedDiscs() 
    { 
        maxNumberOfPossessedDiscs++;

        /*DiscScript newDisc = GetDiscFromPool();
        newDisc.gameObject.SetActive(false);
        possessedDiscs.Push(newDisc);*/
    }

    public void FillPossessedDiscsWithBasicDiscs()
    {
        for(int i =0; i < maxNumberOfPossessedDiscs; i++)
            possessedDiscs.Push(DiscType.Basic);
    }

    public void PlayerRetreiveDisc(DiscScript retreivedDisc)
    {
        if (possessedDiscs.Count < maxNumberOfPossessedDiscs)
        {
            throwedDiscs.Remove(retreivedDisc);
            possessedDiscs.Push(retreivedDisc.GetDiscType);
            ReturnDiscInPool(retreivedDisc);
        }
        else
        {
            Debug.Log("TOO MUCH DISCS, NOT ADDED BUT SUPPOSED TO BE SOMETHING");
            ReturnDiscInPool(retreivedDisc);
        }
    }

    public DiscScript TakeFirstDiscFromPossessedDiscs()
    {
        if (possessedDiscs.Count == 0)
            return null;

        DiscScript newDisc = GetDiscFromPool(possessedDiscs.Pop());
        if (newDisc != null)
            throwedDiscs.Add(newDisc);

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
    Basic
}

[System.Serializable]
public struct DiscPoolParameters
{
    public DiscType discType;
    public DiscScript discPrefab;
    public int baseNumberOfElements;
}