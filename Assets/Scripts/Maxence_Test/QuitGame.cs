
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
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        else
        {
            ReturnEvent.Invoke();
        }
      
    }
}
