using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
    private List<Competence> listUnlockedCompetences = new List<Competence>();
    private List<Competence> listEquipedCompetences = new List<Competence>();


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

        if (competence.GetPointsCost <= competencesPoints)
        {
            competence.SetUnlockedState(true);
            competencesPoints -= competence.GetPointsCost;

            listUnlockedCompetences.Add(competence);

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
        Debug.Log(listEquipedCompetences.Count);

        bool competenceAdd = false;

        // Replace the type of competence by the new one
        for (int i = 0; i < listEquipedCompetences.Count; i++)
        {
            if (competence is CompetenceRecall && listEquipedCompetences[i] is CompetenceRecall)
            {
                listEquipedCompetences[i] = competence;
                competenceAdd = true;
                Debug.Log("Competence recall changed");

                break;
            }

            else if (competence is CompetenceThrow && listEquipedCompetences[i] is CompetenceThrow)
            {
                listEquipedCompetences[i] = competence;
                competenceAdd = true;
                Debug.Log("Competence throw changed");
                break;
            }

            else if (competence is CompetenceSpecial && listEquipedCompetences[i] is CompetenceSpecial)
            {
                listEquipedCompetences[i] = competence;
                competenceAdd = true;
                Debug.Log("Competence special changed");
                break;
            }
        }

        // Add the competence if nothing is found
        if (!competenceAdd)
        {
            listEquipedCompetences.Add(competence);
            Debug.Log("Competence add");
        }

        OnEquipCompetence?.Invoke(competence);
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
        Debug.Log("<color=blue>Competences points : </color>" + competencesPoints);
        Debug.Log("<color=red>Unlocked competences</color>");
        for (int i = 0; i < listUnlockedCompetences.Count; i++)
        {
            Debug.Log(listUnlockedCompetences[i].Getdescription);
        }
        Debug.Log("<color=yellow>Equiped competences</color>");
        for (int i = 0; i < listEquipedCompetences.Count; i++)
        {
            Debug.Log(listEquipedCompetences[i].Getdescription);
        }
    }

    // Clear the console
    public void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}