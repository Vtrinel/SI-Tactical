using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public GameObject MenuCanvas;

    public void ContinueGame()
    {
        Debug.Log("ContinueGame");

        TimeManager.Instance.Resume();
        MenuCanvas.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
}
