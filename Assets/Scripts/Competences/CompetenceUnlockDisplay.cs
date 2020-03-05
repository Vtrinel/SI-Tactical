using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompetenceUnlockDisplay : MonoBehaviour
{

    [Header("Interface of the competence")]
    public TextMeshProUGUI description;
    public TextMeshProUGUI ActionPoint;
    public TextMeshProUGUI UnlockCost;
    public Button unlockButton;
    public Button equipButton;

    private Competence Currentcompetence;

    #region Actions add

    private void OnEnable()
    {
        PlayerExperienceManager._instance.OnSelectCompetence += RefreshCompetence;
        PlayerExperienceManager._instance.OnTryUnlockCompetence += RefreshCompetence;
    }

    private void OnDisable()
    {
        PlayerExperienceManager._instance.OnSelectCompetence -= RefreshCompetence;
        PlayerExperienceManager._instance.OnTryUnlockCompetence -= RefreshCompetence;
    }

    #endregion

    // Change the UI for the competence info
    private void RefreshCompetence(Competence competence)
    {
        Currentcompetence = PlayerExperienceManager._instance.GetSelectedCompetence();

        if (Currentcompetence)
        {
            description.text = Currentcompetence.Getdescription;
            ActionPoint.text = Currentcompetence.GetActionPointsCost.ToString();
            UnlockCost.text = Currentcompetence.GetPointsCost.ToString();


            // See if the competence can be unlocked or equiped
            if (Currentcompetence.GetUnlockedState)
            {
                unlockButton.gameObject.SetActive(false);
                equipButton.gameObject.SetActive(true);
            }
            else
            {
                unlockButton.gameObject.SetActive(true);
                equipButton.gameObject.SetActive(false);
            }
        }
    }

    // Send the request to unlock the competence
    public void UnlockCompetence()
    {
        Debug.Log(Currentcompetence.ToString());

        PlayerExperienceManager._instance.CanUnlockCompetence(Currentcompetence);
    }

    public void EquipCompetence()
    {
        PlayerExperienceManager._instance.EquipCompetence(Currentcompetence);
    }
}
