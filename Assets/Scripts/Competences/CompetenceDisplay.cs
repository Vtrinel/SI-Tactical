using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompetenceDisplay : MonoBehaviour
{
    public Competence competence;

    // Get the competence and show the image of the competence
    void Start()
    {
        if (competence)
        {
            var image = gameObject.GetComponent<Image>();
            image.sprite = competence.GetCompetenceImage;
        }
    }

    public void SelectCompetence()
    {
        PlayerExperienceManager._instance.SelectCompetence(competence);
    }
}
