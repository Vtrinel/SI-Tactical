using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscBar : MonoBehaviour
{
    [Header("Disc types")]
    [SerializeField] Transform discIconHolder;

    [SerializeField] List<DiscElement> AllDiscElement = new List<DiscElement>();

    [SerializeField] Image imgFirst;

    private void OnEnable()
    {
        DiscManager.Instance.OnDiscFilled += InitDisc;
        DiscManager.Instance.OnDiscConsommed += RemoveDiscFromBar;
        DiscManager.Instance.OnDiscAdded += AddNewDisc;
        GameManager.Instance.OnThrowCompetenceSelectionStateChanged += SetColor;
    }

    private void OnDisable()
    {
        DiscManager.Instance.OnDiscFilled -= InitDisc;
        DiscManager.Instance.OnDiscAdded -= AddNewDisc;
        DiscManager.Instance.OnDiscConsommed -= RemoveDiscFromBar;
        GameManager.Instance.OnThrowCompetenceSelectionStateChanged -= SetColor;
    }

    void SetColor(bool value)
    {
        if (value)
        {
            //imgFirst.color = new Color(1, 0.8313726f, 0);
            AllDiscElement[0].Select(new Color(1, 0.8313726f, 0));
        }
        else
        {
            //imgFirst.color = Color.white;
            AllDiscElement[0].Unselect();
        }
    }

    void InitDisc(int i, int y, DiscType type)
    {
        Stack<DiscType> allDisc = DiscManager.Instance.GetPossessedDiscs;

        Refresh(allDisc);
    }

    void AddNewDisc(DiscScript newDisc)
    {
        Stack<DiscType> allDisc = DiscManager.Instance.GetPossessedDiscs;

        Refresh(allDisc);
    }

    void RemoveDiscFromBar()
    {
        Stack<DiscType> allDisc = DiscManager.Instance.GetPossessedDiscs;

        Refresh(allDisc);
    }

    void Refresh(Stack<DiscType> _allDisc)
    {
        if(_allDisc.Count == 0)
        {
            discIconHolder.gameObject.SetActive(false);
            return;
        }
        else
        {
            discIconHolder.gameObject.SetActive(true);
        }

        foreach(DiscElement discElement in AllDiscElement)
        {
            discElement.gameObject.SetActive(false);
        }

        int i = 0;
        foreach (DiscType discType in _allDisc)
        {
            DiscScript peekedDisc = DiscManager.Instance.PeekDiscFromPool(discType);

            if (i >= AllDiscElement.Count)
                break;

            DiscElement de = AllDiscElement[i];

            de.SetIcon((int)discType, peekedDisc.GetCurrentDamage);

            AllDiscElement[i].gameObject.SetActive(true);
            i++;
        }
    }
}
