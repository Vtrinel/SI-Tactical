using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscManager : MonoBehaviour
{
    public List<GameObject> allCrystals = new List<GameObject>();
    List<GameObject> crystalsUse = new List<GameObject>();

    public GameObject prefabCrystal;

    public static float crystalHeight = 1f;

    public float rangeOgPlayer = 5;
    Transform player;


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
    }

    private void Update()
    {
        foreach(GameObject disc in crystalsUse)
        {
            disc.GetComponent<DiscScript>().isInRange = (Vector3.Distance(player.position, disc.transform.position) < 7);
        }
    }

    public GameObject GetCrystal()
    {
        foreach (GameObject element in allCrystals)
        {
            if (!element.activeSelf && !crystalsUse.Contains(element.gameObject))
            {
                crystalsUse.Add(element);
                return element;
            }
        }

        GameObject newEnnemy = Instantiate(prefabCrystal, transform);
        allCrystals.Add(newEnnemy);
        crystalsUse.Add(newEnnemy);
        return newEnnemy;
    }

    public void DeleteCrystal(GameObject element)
    {
        element.SetActive(false);
        crystalsUse.Remove(element);
    }

    public List<GameObject> GetAllCrystalUse()
    {
        return crystalsUse;
    }
}
