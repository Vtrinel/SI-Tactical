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
    private void Start()
    {
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
    public Action<bool> OnEnterTotemZone;
    public Action<int> OnSelectTypeCompetence;
    public Action<int> OnGainGold;
    public Action<int> OnLossGold;
    public Action<Competence> OnSelectCompetence;
    public Action<Competence> OnTryUnlockCompetence;
    public Action<Competence> OnEquipCompetence;
    public Action<CompetenceThrow, CompetenceRecall, CompetenceSpecial> OnSetChanged;


    // Private Attributes
    [SerializeField] PlayerController playerController;

    private bool canOpenCompetenceMenu = false;

    private Competence selectedCompetence;

    private CompetenceThrow equipedThrowCompetence = default;
    private CompetenceRecall equipedRecallCompetence = default;
    private CompetenceSpecial equipedSpecialCompetence = default;

    

    [Header("Start competences")]
    [SerializeField] CompetenceThrow startThrowCompetence = default;
    [SerializeField] CompetenceRecall startRecallCompetence = default;
    [SerializeField] CompetenceSpecial startSpecialCompetence = default;

    private int competenceTypeSelected = 0; // 0 = throw, 1 = recall, 2 = special
    [SerializeField] int goldBar = 0; // 0 => 100. At 100, unlock a competence point
    [SerializeField] int goldToUnlockCompetence = 100;
    private bool canUnlockComp = false;

    private bool isCanvasCompetenceShowed = false;
    public bool IsUsingCompetencesMenu => isCanvasCompetenceShowed;
    public int GetGoldQuantity => goldBar;

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
        if (Input.GetKeyDown(playerController.GetCompetenceMenuInput) && canOpenCompetenceMenu)
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

        // Competence experience bar
        if (goldBar >= goldToUnlockCompetence)
        {
            goldBar = goldToUnlockCompetence;
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
            if (competence.CanUnlockCompetence() && competence)
            {
                goldBar = 0;
                OnLossGold?.Invoke(goldToUnlockCompetence);
            }
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

    public void GainGold(int experience)
    {
        goldBar += experience;
        OnGainGold?.Invoke(experience);
    }

    public void LossGold(int experience)
    {
        goldBar -= experience;
        OnLossGold?.Invoke(experience);
    }

    public void AddMaximumDisc()
    {
        if (canUnlockComp)
        {
            DiscManager.Instance.AddOneMaxNumberOfPossessedDiscs();

            canUnlockComp = false;
            goldBar = 0;
            OnLossGold?.Invoke(100);
        }
    }

    public void AddMaximumRangeDisc()
    {
        if (canUnlockComp)
        {
            DiscManager.Instance.AddOneMaxRangeOfPlayer();

            canUnlockComp = false;
            goldBar = 0;
            OnLossGold?.Invoke(100);
        }
    }

    public void AddMaxHealth()
    {
        if (canUnlockComp)
        {
            GameManager.Instance.PlayerMaxLifeChange(1);

            canUnlockComp = false;
            goldBar = 0;
            OnLossGold?.Invoke(100);
        }
    }

    public void CloseCompetenceInterface()
    {
        isCanvasCompetenceShowed = !isCanvasCompetenceShowed;
        competenceCanvas.gameObject.SetActive(isCanvasCompetenceShowed);
        OnMenuOpenedOrClosed?.Invoke();
    }

    public void CanOpenCompetenceMenu(bool canOpen)
    {
        canOpenCompetenceMenu = canOpen;
        OnEnterTotemZone?.Invoke(canOpen);
    }
}