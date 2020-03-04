using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExperienceManager : MonoBehaviour
{
    // Public attributes
    public static PlayerExperienceManager _instance;

    [Header("Recall Competences")]
    public List<CompetenceRecall> RecallCompetences = new List<CompetenceRecall>();

    [Header("Throw Competences")]
    public List<CompetenceThrow> ThrowCompetence = new List<CompetenceThrow>();

    //Actions
    public event Action OnSelectCompetence;
    public event Action<Competence> OnTryUnlockCompetence;


    // Private Attributes
    private int competencesPoints = 0;
    private Competence selectedCompetence;


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
        OnTryUnlockCompetence?.Invoke(competence);

        Debug.Log("Competences points : " + competencesPoints);

        if (competence.GetPointsCost <= competencesPoints)
        {
            competence.SetUnlockedState(true);
            competencesPoints -= competence.GetPointsCost;

            Debug.Log("Unlocked");

        }

        Debug.Log("Competences points : " + competencesPoints);
    }

    // Setter SelectedCompetence
    public void SelectCompetence(Competence competence)
    {
        OnSelectCompetence?.Invoke();

        selectedCompetence = competence;
    }
    
    // Getter SelectedCompetence
    public Competence GetSelectedCompetence()
    {
        return selectedCompetence;
    }
}
