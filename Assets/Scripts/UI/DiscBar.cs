using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscBar : MonoBehaviour
{
    [Header("Disc types")]
    [SerializeField] Transform discIconHolder;

    [SerializeField] List<DiscElement> AllDiscElement = new List<DiscElement>();

    [SerializeField] List<Vector2> AllPos = new List<Vector2>();

    [SerializeField] Image imgFirst;

    private void OnEnable()
    {
        DiscManager.Instance.OnDiscFilled += InitDisc;
        DiscManager.Instance.OnDiscConsommed += RemoveDiscFromBar;
        DiscManager.Instance.OnDiscAdded += AddNewDisc;
        //DiscManager.Instance.OnAddOneMaxDisc += UpdateMaxDiscBar;
        GameManager.Instance.OnThrowCompetenceSelectionStateChanged += SetColor;
    }

    private void OnDisable()
    {
        DiscManager.Instance.OnDiscFilled -= InitDisc;
        DiscManager.Instance.OnDiscAdded -= AddNewDisc;
        //DiscManager.Instance.OnAddOneMaxDisc -= UpdateMaxDiscBar;
        DiscManager.Instance.OnDiscConsommed -= RemoveDiscFromBar;
    }

    void SetColor(bool value)
    {
        if (value)
        {
            imgFirst.color = new Color(255, 45, 0);
        }
        else
        {
            imgFirst.color = Color.white;
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
        foreach (DiscType discType in _allDisc)
        {
            print(discType);
        }

        if(_allDisc.Count == 0)
        {
            discIconHolder.gameObject.SetActive(false);
            return;
        }
        else
        {
            discIconHolder.gameObject.SetActive(true);
        }


        DiscType oldType = DiscType.None;
        int numBerOfSame = 0;
        int i = 0;

        foreach (DiscType discType in _allDisc)
        {
            if(oldType == DiscType.None)
            {
                oldType = discType;
            }

            if(discType != oldType)
            {
                i++;
                DiscElement de = AllDiscElement[i];
                de.SetIcon((int)discType, numBerOfSame);

                numBerOfSame = 0;
            }
            else
            {
                numBerOfSame++;
                DiscElement de = AllDiscElement[i];
                de.SetIcon((int)discType, numBerOfSame);
            }
            oldType = discType;
        }

        //discIconHolder.position = AllPos[i];
    }
}
