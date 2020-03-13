using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class CanvasMenuManager : MonoBehaviour
{
    public Animator AnimatorSelf;
    public static CanvasMenuManager CanvasMenuManagerStatic;
    public int LevelDebloqued;
    public List<GameObject> Levels = new List<GameObject>();



    private void Awake()
    {
        CanvasMenuManagerStatic = this;
    }


    public void DebloqueNewLevel()
    {
        LevelDebloqued++;
        if (LevelDebloqued < Levels.Count)
        {
            Levels[LevelDebloqued].GetComponent<Button>().interactable = true;
        }
      
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("");
    }

    private void Start()
    {
        AnimatorSelf = GetComponent<Animator>();
    }

    public void MenuStep1()
    {
        AnimatorSelf.SetInteger("Step", 1);
    }

    public void MenuStep2_1()
    {
        AnimatorSelf.SetInteger("Step", 2);
    }

    public void MenuStep2_2()
    {
        AnimatorSelf.SetInteger("Step", 3);
    }

    public void MenuStep2_3()
    {
        AnimatorSelf.SetInteger("Step", 4);
    }


    public void MenuReturnStep0()
    {
        AnimatorSelf.SetInteger("Step", 0);
    }

}
