using System.Collections.Generic;
using UnityEngine;

public class DiscBar : MonoBehaviour
{
    private int maxDisc;

    [SerializeField] int currentDisc = 3;

    [Header("Disc types")]
    public GameObject discPiercing;
    public GameObject discGhost;
    public GameObject discExplosive;
    public GameObject discHeavy;
    public GameObject discShockwave;

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
        //currentDisc = maxDisc-1;

        for (int i = 0; i < currentDisc; i++)
        {
            GameObject newDiscBarElement = Instantiate(discPiercing, gameObject.transform);
            allDiscBarElement.Add(newDiscBarElement);
        }

        currentDisc = currentDisc - 1;
    }

    void RemoveDiscFromBar()
    {
        allDiscBarElement.RemoveAt(currentDisc);

        currentDisc--;
    }

    void AddNewDisc(DiscScript newDisc)
    {
        GameObject newDiscBarElement;

        switch (newDisc.GetDiscType)
        {
            case DiscType.Piercing:
                newDiscBarElement = Instantiate(discPiercing, gameObject.transform);
                break;

            case DiscType.Ghost:
                newDiscBarElement = Instantiate(discGhost, gameObject.transform);
                break;

            case DiscType.Explosive:
                newDiscBarElement = Instantiate(discExplosive, gameObject.transform);
                break;

            case DiscType.Heavy:
                newDiscBarElement = Instantiate(discHeavy, gameObject.transform);
                break;

            case DiscType.Shockwave:
                newDiscBarElement = Instantiate(discShockwave, gameObject.transform);
                break;

            default:
                newDiscBarElement = Instantiate(discPiercing, gameObject.transform);
                break;
        }

        allDiscBarElement.Add(newDiscBarElement);

        currentDisc++;
    }

    void UpdateMaxDiscBar()
    {
        //GameObject newDiscBarElement = Instantiate(discPiercing, gameObject.transform);
        //allDiscBarElement.Add(newDiscBarElement);

        maxDisc++;
        currentDisc++;

    }
}
