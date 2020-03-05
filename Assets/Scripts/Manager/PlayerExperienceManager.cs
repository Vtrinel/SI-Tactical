using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExperienceManager : MonoBehaviour
{
    // Public attributes
    public static PlayerExperienceManager _instance;

    [Header("Lists competences")]
    public List<CompetenceRecall> RecallCompetences = new List<CompetenceRecall>();
    public List<CompetenceThrow> ThrowCompetence = new List<CompetenceThrow>();

    //Actions
    public event Action<Competence> OnSelectCompetence;
    public event Action<Competence> OnTryUnlockCompetence;
    public event Action<Competence> OnEquipCompetence;


    // Private Attributes
    private int competencesPoints = 2;
    private Competence selectedCompetence;

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

    // Check if a competence can be unlocked and unlock it if possible
    public void CanUnlockCompetence(Competence competence)
    {
        Debug.Log("Competences points : " + competencesPoints);

        if (competence.GetPointsCost <= competencesPoints)
        {
            competence.SetUnlockedState(true);
            competencesPoints -= competence.GetPointsCost;

            Debug.Log("Unlocked");

        }

        Debug.Log("Competences points : " + competencesPoints);

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
        Debug.Log("Is equiped : " + selectedCompetence.Getdescription);

        //bool isCompRecall = competence is DiscScript;

        OnEquipCompetence?.Invoke(competence);
    }

    // Getter SelectedCompetence
    public Competence GetSelectedCompetence()
    {
        return selectedCompetence;
    }
}