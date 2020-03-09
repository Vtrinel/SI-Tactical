using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxManager : MonoBehaviour
{
    [SerializeField] List<GameObject> allExplo = new List<GameObject>();
    [SerializeField] List<GameObject> exploUse = new List<GameObject>();

    [SerializeField] List<GameObject> allHit = new List<GameObject>();
    [SerializeField] List<GameObject> hitUse = new List<GameObject>();


    [SerializeField] GameObject prefabExplo = default;
    [SerializeField] GameObject prefabHit = default;

    private static FxManager _instance;
    public static FxManager Instance { get { return _instance; } }

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

    public GameObject DemandeFx(fxType myTypeFx)
    {
        List<GameObject> currentList = new List<GameObject>();
        List<GameObject> currentUseList = new List<GameObject>();
        GameObject currentPrefab = null;

        switch (myTypeFx)
        {
            case fxType.Explosion:
                currentList = allExplo;
                currentUseList = exploUse;
                currentPrefab = prefabExplo;
                break;

            case fxType.Hit:
                currentList = allHit;
                currentUseList = hitUse;
                currentPrefab = prefabHit;
                break;
        }

        foreach (GameObject element in currentList)
        {
            if (!element.gameObject.activeSelf && !currentUseList.Contains(element))
            {
                AddFxToList(element, myTypeFx);
                return element;
            }
        }

        return CreateFx(myTypeFx);
    }

    GameObject CreateFx(fxType hisType)
    {
        switch (hisType)
        {
            case fxType.Explosion:
                GameObject newExplo = Instantiate(prefabExplo, transform);
                allExplo.Add(newExplo);
                exploUse.Add(newExplo);
                return newExplo;

            case fxType.Hit:
                GameObject newHit = Instantiate(prefabHit, transform);
                allHit.Add(newHit);
                hitUse.Add(newHit);
                return newHit;
        }

        return null;
    }

    void AddFxToList(GameObject fx ,fxType hisType)
    {
        switch (hisType)
        {
            case fxType.Explosion:
                exploUse.Add(fx);
                break;

            case fxType.Hit:
                hitUse.Add(fx);
                break;
        }
    }

    public void RemoveThisFx(GameObject fx, fxType hisType)
    {
        fx.SetActive(false);
        
        switch (hisType)
        {
            case fxType.Explosion:
                exploUse.Remove(fx);
                break;

            case fxType.Hit:
                hitUse.Remove(fx);
                break;
        }
    }

    public enum fxType
    {
        Explosion,
        Hit
    }
}
