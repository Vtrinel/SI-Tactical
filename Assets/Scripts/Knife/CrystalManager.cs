using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalManager : MonoBehaviour
{
    public List<GameObject> allCrystals = new List<GameObject>();
    List<GameObject> crystalsUse = new List<GameObject>();

    public GameObject prefabCrystal;


    private static CrystalManager _instance;
    public static CrystalManager Instance { get { return _instance; } }

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
}
