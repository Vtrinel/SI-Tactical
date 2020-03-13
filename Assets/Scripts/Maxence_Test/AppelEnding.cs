using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppelEnding : MonoBehaviour
{
   public GameObject YouWin;
   public GameObject YouLose;

    public void LoadYouWin()
    {
        YouWin.SetActive(true);
    }

    public void LoadYouLose()
    {
        YouLose.SetActive(true);
    }
 }
