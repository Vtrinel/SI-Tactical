using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using TMPro;

public class PlayerExperienceManager : MonoBehaviour
{
    public void SetUp()
    {
        equipedThrowCompetence = startThrowCompetence;
        equipedRecallCompetence = startRecallCompetence;
        equipedSpecialCompetence = startSpecialCompetence;

        OnSetChanged?.Invoke(equipedThrowCompetence, equipedRecallCompetence, equipedSpecialCompetence);
    }

    // Public attributes
    public static PlayerExperienceManager _instance;

    [Header("Interface modifications")]
    public Canvas competenceCanvas;
    public TextMeshProUGUI competenceInfoText;

    [Header("Menus Competences")]
    public GameObject throwInterface;
    public GameObject recallInterface;
    public GameObject specialInterface;

    //Actions
    public Action<int> OnSelectTypeCompetence;
    public Action<Competence> OnSelectCompetence;
    public Action<Competence> OnTryUnlockCompetence;
    public Action<Competence> OnEquipCompetence;
    public Action<CompetenceThrow, CompetenceRecall, CompetenceSpecial> OnSetChanged;
    public Action<int> OnGainExperience;

    // Private Attributes
    
    private Competence selectedCompetence;

    CompetenceThrow equipedThrowCompetence = default;
    CompetenceRecall equipedRecallCompetence = default;
    CompetenceSpecial equipedSpecialCompetence = default;

    

    [Header("Start competences")]
    [SerializeField] CompetenceThrow startThrowCompetence = default;
    [SerializeField] CompetenceRecall startRecallCompetence = default;
    [SerializeField] CompetenceSpecial startSpecialCompetence = default;

    private int competenceTypeSelected = 0; // 0 = throw, 1 = recall, 2 = special
    private int competenceBar = 0; // 0 => 100. At 100, unlock a competence point
    private bool canUnlockComp = false;

    private bool isCanvasCompetenceShowed = false;
    public bool IsUsingCompetencesMenu => isCanvasCompetenceShowed;

    // For the singleton
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
    public static PlayerExperienceManager Instance { get { return _instance; } }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            IsCompetenceInterfaceShowing();
        }

        // Change the competence interface on click
        switch (competenceTypeSelected)
        {
            case 0:
                throwInterface.gameObject.SetActive(true);
                recallInterface.gameObject.SetActive(false);
                specialInterface.gameObject.SetActive(false);
                break;

            case 1:
                throwInterface.gameObject.SetActive(false);
                recallInterface.gameObject.SetActive(true);
                specialInterface.gameObject.SetActive(false);
                break;

            case 2:
                throwInterface.gameObject.SetActive(false);
                recallInterface.gameObject.SetActive(false);
                specialInterface.gameObject.SetActive(true);
                break;

            default:
                throwInterface.gameObject.SetActive(true);
                recallInterface.gameObject.SetActive(false);
                specialInterface.gameObject.SetActive(false);
                break;
        }

        // Compatence bar
        if (competenceBar >= 100)
        {
            competenceBar = 100;
            canUnlockComp = true;

            competenceInfoText.text = "Competence point available";
        }
        else
        {
            canUnlockComp = false;
            competenceInfoText.text = "No point available";
        }

    }

    // Show or not the competence interface

    public Action OnMenuOpenedOrClosed;
    public void IsCompetenceInterfaceShowing()
    {
        isCanvasCompetenceShowed = !isCanvasCompetenceShowed;
        competenceCanvas.gameObject.SetActive(isCanvasCompetenceShowed);
        OnMenuOpenedOrClosed?.Invoke();
    }

    // Change the type of competence selected
    public void SelectTypeCompetence(int type)
    {
        competenceTypeSelected = type;
        OnSelectTypeCompetence?.Invoke(type);
    }
    

    // Check if a competence can be unlocked and unlock it if possible
    public void CanUnlockCompetence(Competence competence)
    {

        if (canUnlockComp)
        {
            competence.SetUnlockedState(true);
            competenceBar = 0;
            Debug.Log("Competence unlocked");
        }
        else
        {
            Debug.Log("Not enough competence point");
        }

        OnTryUnlockCompetence?.Invoke(competence);
    }

    // Setter SelectedCompetence
    public void SelectCompetence(Competence competence)
    {
        selectedCompetence = competence;

        OnSelectCompetence?.Invoke(competence);
    }

    public void EquipCompetence(Competence competence)
    {
        //Debug.Log(listEquipedCompetences.Count);

        bool competenceAdd = false;

        CompetenceThrow newThrowComp = competence as CompetenceThrow;
        if (newThrowComp != null)
        {
            equipedThrowCompetence = newThrowComp;
        }
        else
        {
            CompetenceRecall newRecallComp = competence as CompetenceRecall;
            if (newRecallComp != null)
            {
                equipedRecallCompetence = newRecallComp;
            }
            else
            {
                CompetenceSpecial newSpecialComp = competence as CompetenceSpecial;
                if (newSpecialComp != null)
                {
                    equipedSpecialCompetence = newSpecialComp;
                }
            }
        }        

        OnEquipCompetence?.Invoke(competence);
        OnSetChanged?.Invoke(equipedThrowCompetence, equipedRecallCompetence, equipedSpecialCompetence);
    }

    // Getter SelectedCompetence
    public Competence GetSelectedCompetence()
    {
        return selectedCompetence;
    }

    // Show the debug stats
    public void ResumeStats()
    {
        ClearLog();

        Debug.Log("Show all competences stats");
        Debug.Log("<color=blue>Competences points : </color>" + canUnlockComp);
        Debug.Log("<color=yellow>Equiped competences</color>");
        Debug.Log("equipedThrowCompetence : " + equipedThrowCompetence.GetCompetenceName);
        Debug.Log("equipedRecallCompetence : " + equipedRecallCompetence.GetCompetenceName);
        Debug.Log("equipedSpecialCompetence : " + equipedSpecialCompetence.GetCompetenceName);
    }

    // Clear the console
    public void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }

    public void GainExperience(int experience)
    {
        competenceBar += experience;
        OnGainExperience?.Invoke(experience);
    }

    public void AddMaximumDisc()
    {
        if (canUnlockComp)
        {
            DiscManager.Instance.AddOneMaxNumberOfPossessedDiscs();

            canUnlockComp = false;
            competenceBar = 0;
        }
    }

    public void AddMaximumRangeDisc()
    {
        if (canUnlockComp)
        {
            DiscManager.Instance.AddOneMaxRangeOfPlayer();

            canUnlockComp = false;
            competenceBar = 0;
        }
    }
}