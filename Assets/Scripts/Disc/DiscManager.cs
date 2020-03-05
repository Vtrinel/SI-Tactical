using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscManager : MonoBehaviour
{
    public List<DiscScript> allDiscs = new List<DiscScript>();
    List<DiscScript> discsUse = new List<DiscScript>();

    public GameObject prefabDisc;

    public static float crystalHeight = 1f;

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

        /*recallZonePreview = Instantiate(recallZonePreviewPrefab);
        recallZonePreview.SetActive(false);*/
    }

    private void Update()
    {
        Vector3 playerPos = player.position;
        foreach (DiscScript disc in discsUse)
        {
            Vector3 discPosHorizontalyWithPlayer = disc.transform.position;
            discPosHorizontalyWithPlayer.y = playerPos.y;

            disc.isInRange = (Vector3.Distance(playerPos, discPosHorizontalyWithPlayer) <= rangeOfPlayer);
        }
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

    public GameObject GetCrystal()
    {
        foreach (DiscScript element in allDiscs)
        {
            if (!element.gameObject.activeSelf && !discsUse.Contains(element))
            {
                discsUse.Add(element.GetComponent<DiscScript>());
                return element.gameObject;
            }
        }

        DiscScript newEnnemy = Instantiate(prefabDisc, transform).GetComponent<DiscScript>();
        allDiscs.Add(newEnnemy);
        discsUse.Add(newEnnemy);
        return newEnnemy.gameObject;
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

    /*#region Recall Preview
    [Header("Preview")]
    [SerializeField] GameObject recallZonePreviewPrefab = default;
    GameObject recallZonePreview = default;

    public void StartRecallPreview(Vector3 recallPosition)
    {
        recallZonePreview.SetActive(true);
        recallZonePreview.transform.localScale = Vector3.one * rangeOfPlayer;

        UpdateRecallPreview(recallPosition);
    }

    public void UpdateRecallPreview(Vector3 recallPosition)
    {
        recallZonePreview.transform.position = recallPosition;
    }

    public void EndRecallPreview()
    {
        recallZonePreview.SetActive(false);
    }
    #endregion*/
}