using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscBar : MonoBehaviour
{
    private int maxDisc;

    [SerializeField] int currentDisc;

    [Header("Disc types")]
    // Types : None , Piercing, Ghost, Explosive, Heavy, Shockwave
    public GameObject discBasic;
    //public GameObject discPiercing;
    //public GameObject discGhost;

    [SerializeField] List<GameObject> allDiscBarElement = new List<GameObject>();

    private void OnEnable()
    {
        DiscManager.Instance.OnDiscFilled += CreateDiscBar;
        DiscManager.Instance.OnDiscConsommed += RemoveDiscFromBar;
        DiscManager.Instance.OnDiscAdded += AddNewDisc;
        DiscManager.Instance.OnAddOneMaxDisc += UpdateMaxDiscBar;
    }

    private void OnDisable()
    {
        DiscManager.Instance.OnDiscConsommed -= RemoveDiscFromBar;
        DiscManager.Instance.OnDiscFilled -= CreateDiscBar;
        DiscManager.Instance.OnDiscAdded -= AddNewDisc;
        DiscManager.Instance.OnAddOneMaxDisc -= UpdateMaxDiscBar;
    }

    void CreateDiscBar(int maxNumberOfPossessedDiscs)
    {
        maxDisc = maxNumberOfPossessedDiscs;
        currentDisc = maxDisc-1;

        Debug.Log("test ?");

        for (int i = 0; i < maxDisc; i++)
        {
            GameObject newDiscBarElement = Instantiate(discBasic, gameObject.transform);
            allDiscBarElement.Add(newDiscBarElement);
        }
    }

    void RemoveDiscFromBar()
    {
        Destroy(allDiscBarElement[currentDisc]);

        currentDisc--;
    }

    void AddNewDisc(DiscScript newDisc)
    {
        GameObject newDiscBarElement = Instantiate(discBasic, gameObject.transform);
        allDiscBarElement.Add(newDiscBarElement);

        currentDisc++;
    }

    void UpdateMaxDiscBar()
    {
        GameObject newDiscBarElement = Instantiate(discBasic, gameObject.transform);
        allDiscBarElement.Add(newDiscBarElement);

        maxDisc++;
        currentDisc++;

    }
}
