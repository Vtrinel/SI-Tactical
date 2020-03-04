using TMPro;
using UnityEngine;

public class CompetenceUnlockDisplay : MonoBehaviour
{

    public TextMeshProUGUI description;
    public TextMeshProUGUI ActionPoint;
    public TextMeshProUGUI UnlockCost;

    private Competence competence;

    private void OnEnable()
    {
        PlayerExperienceManager._instance.OnSelectCompetence += RefreshCompetence;
    }

    private void OnDisable()
    {
        PlayerExperienceManager._instance.OnSelectCompetence -= RefreshCompetence;
    }

    // Change the UI for the competence info
    private void RefreshCompetence()
    {
        competence = PlayerExperienceManager._instance.GetSelectedCompetence();

        description.text = competence.Getdescription;
        ActionPoint.text = competence.GetActionPointsCost.ToString();
        UnlockCost.text = competence.GetPointsCost.ToString();
    }

    // Send the request to unlock the competence
    public void unlockCompetence()
    {
        PlayerExperienceManager._instance.CanUnlockCompetence(competence);
    }
}
