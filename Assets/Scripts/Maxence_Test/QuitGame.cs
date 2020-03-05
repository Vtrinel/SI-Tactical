
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public bool ModeReturn;
    public UnityEvent ReturnEvent;

    public void QuitGameButton()
    {
        if(ModeReturn == false)
        {
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        }
        else
        {
            ReturnEvent.Invoke();
        }
      
    }
}
