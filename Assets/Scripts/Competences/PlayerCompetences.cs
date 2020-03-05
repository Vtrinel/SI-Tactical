using System.Collections.Generic;
using UnityEngine;

public class PlayerCompetences : MonoBehaviour
{

    [Header("PlayerStats")]
    public int competencesPoints = 3;

    [Header("Player Competences")]
    public List<Competence> Competences = new List<Competence>();
}
