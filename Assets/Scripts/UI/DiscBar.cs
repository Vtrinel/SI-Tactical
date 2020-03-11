using System.Collections.Generic;
using UnityEngine;

public class DiscBar : MonoBehaviour
{
    [Header("Disc types")]
    [SerializeField] Transform discIconHolder;

    [SerializeField] List<DiscElement> AllDiscElement = new List<DiscElement>();

    [SerializeField] List<Vector2> AllPos = new List<Vector2>();

    private void OnEnable()
    {
        //DiscManager.Instance.OnDiscFilled += CreateDiscBar;
        DiscManager.Instance.OnDiscConsommed += RemoveDiscFromBar;
        DiscManager.Instance.OnDiscAdded += AddNewDisc;
        //DiscManager.Instance.OnAddOneMaxDisc += UpdateMaxDiscBar;
    }

    private void OnDisable()
    {
        //DiscManager.Instance.OnDiscFilled -= CreateDiscBar;
        DiscManager.Instance.OnDiscAdded -= AddNewDisc;
        //DiscManager.Instance.OnAddOneMaxDisc -= UpdateMaxDiscBar;
        DiscManager.Instance.OnDiscConsommed -= RemoveDiscFromBar;
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
        DiscType oldType = DiscType.None;
        int numBerOfSame = 0;
        int i = 0;

        foreach (DiscType discType in _allDisc)
        {
            if(discType == oldType)
            {
                numBerOfSame++;
                oldType = discType;

                DiscElement de = AllDiscElement[i];
                de.SetIcon((int)discType, numBerOfSame);
            }
            else{
                i++;
            }

            numBerOfSame = 0;
        }

        discIconHolder.position = AllPos[i];
    }
}
