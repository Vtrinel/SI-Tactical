using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscManager : MonoBehaviour
{
    #region V1 SAM
    public List<DiscScript> allDiscs = new List<DiscScript>();
    List<DiscScript> discsUse = new List<DiscScript>();
    #endregion

    public DiscScript prefabDisc = default;

    public static float discHeight = 1f;

    public float rangeOfPlayer = 5;
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
    }

    private void Start()
    {
        player = GameManager.Instance.GetPlayer.transform;
        FillPossessedDiscsWithBasicDiscs();
    }

    private void Update()
    {
        // HERE : need to add and remove discs from proximity
        /*
        Vector3 playerPos = player.position;
        foreach (DiscScript disc in discsUse)
        {
            Vector3 discPosHorizontalyWithPlayer = disc.transform.position;
            discPosHorizontalyWithPlayer.y = playerPos.y;

            disc.isInRange = (Vector3.Distance(playerPos, discPosHorizontalyWithPlayer) <= rangeOfPlayer);
        }*/
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

    public DiscScript GetCrystal()
    {
        foreach (DiscScript element in allDiscs)
        {
            if (!element.gameObject.activeSelf && !discsUse.Contains(element))
            {
                discsUse.Add(element.GetComponent<DiscScript>());
                return element;
            }
        }

        DiscScript newElement = Instantiate(prefabDisc, transform).GetComponent<DiscScript>();
        allDiscs.Add(newElement);
        discsUse.Add(newElement);
        return newElement;
    }

    public void DeleteCrystal(GameObject element)
    {
        element.SetActive(false);
        discsUse.Remove(element.GetComponent<DiscScript>());
    }

    public List<DiscScript> GetAllCrystalUse()
    {
        return discsUse;
    }




    #region Pooling
    [Header("Pooling")]
    [SerializeField] Queue<DiscScript> unusedDiscs = new Queue<DiscScript>();

    public DiscScript GetDiscFromPool()
    {
        if (unusedDiscs.Count > 0)
            return unusedDiscs.Dequeue();
        else
        {
            DiscScript newElement = Instantiate(prefabDisc, transform);
            return newElement;
        }
    }

    public void PutBackDiscInPool(DiscScript disc)
    {
        unusedDiscs.Enqueue(disc);
    }
    #endregion

    #region Possessed Discs
    [Header("Stock system")]
    [SerializeField] int maxNumberOfPossessedDiscs = 3;
    Stack<DiscScript> possessedDiscs = new Stack<DiscScript>();
    public int GetPossessedDiscsCount => possessedDiscs.Count;
    public void FillPossessedDiscsWithBasicDiscs()
    {
        for(int i =0; i < maxNumberOfPossessedDiscs; i++)
        {
            DiscScript newDisc = GetDiscFromPool();
            newDisc.gameObject.SetActive(false);
            possessedDiscs.Push(newDisc);
        }
    }

    public void PlayerRetreiveDisc(DiscScript retreivedDisc)
    {
        if (possessedDiscs.Count < maxNumberOfPossessedDiscs)
        {
            possessedDiscs.Push(retreivedDisc);
            retreivedDisc.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("TOO MUCH DISCS, NOT ADDED BUT SUPPOSED TO BE SOMETHING");
            retreivedDisc.gameObject.SetActive(false);
        }
    }

    public DiscScript TakeFirstDiscFromPossessedDiscs()
    {
        if (possessedDiscs.Count == 0)
            return null;

        return possessedDiscs.Pop();
    }
    #endregion

    #region Thrown Discs
    List<DiscScript> thrownDiscs = new List<DiscScript>();

    public void AddDiscToThrown(DiscScript disc)
    {
        if (!thrownDiscs.Contains(disc))
            thrownDiscs.Add(disc);

        AddDiscToNearbyProximity(disc);
    }
    #endregion

    #region Nearby Discs
    public void CheckAllDiscsProximity(Vector3 playerPosition)
    {
        List<DiscScript> newOutRangeDiscs = new List<DiscScript>();
        foreach(DiscScript disc in nearbyDiscs)
        {
            if(!DiscIsInRange(playerPosition, disc.transform.position))
                newOutRangeDiscs.Add(disc);
        }

        List<DiscScript> newInRangeDiscs = new List<DiscScript>();
        foreach (DiscScript disc in outRangeDiscs)
        {
            if (DiscIsInRange(playerPosition, disc.transform.position))
                newInRangeDiscs.Add(disc);
        }

        foreach (DiscScript disc in newOutRangeDiscs)
            RemoveDiscFromNearbyProximity(disc);

        foreach (DiscScript disc in newInRangeDiscs)
            AddDiscToNearbyProximity(disc);
    }

    public bool DiscIsInRange(Vector3 playerPos, Vector3 discPos)
    {
        playerPos.y = discPos.y;
        return Vector3.Distance(playerPos, discPos) <= rangeOfPlayer;
    }

    List<DiscScript> nearbyDiscs = new List<DiscScript>();
    public List<DiscScript> GetNearbyDiscs => nearbyDiscs;
    List<DiscScript> nearbyUnthrowedDiscs = new List<DiscScript>();
    List<DiscScript> outRangeDiscs = new List<DiscScript>();

    public int GetNearbyDiscsCount => nearbyDiscs.Count;

    public void AddDiscToNearbyProximity(DiscScript disc)
    {
        disc.isInRange = true;

        if (!nearbyDiscs.Contains(disc))
        {
            nearbyDiscs.Add(disc);

            if (!thrownDiscs.Contains(disc))
            {
                nearbyUnthrowedDiscs.Add(disc);
            }
        }

        if (outRangeDiscs.Contains(disc))
        {
            outRangeDiscs.Remove(disc);
        }
    }

    public void RemoveDiscFromNearbyProximity(DiscScript disc)
    {
        disc.isInRange = false;

        if (nearbyDiscs.Contains(disc))
        {
            nearbyDiscs.Remove(disc);

            if (nearbyUnthrowedDiscs.Contains(disc))
            {
                nearbyUnthrowedDiscs.Remove(disc);
            }
        }

        if (!outRangeDiscs.Contains(disc))
        {
            outRangeDiscs.Add(disc);
        }
    }
    #endregion
}