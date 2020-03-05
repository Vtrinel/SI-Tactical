using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompetenceTypeChange : MonoBehaviour
{
    public void ChangeTypeCompetence(int type)
    {
        PlayerExperienceManager._instance.SelectTypeCompetence(type);
    }
}
