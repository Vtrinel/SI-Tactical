using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanel : MonoBehaviour
{
    public void Resume()
    {
        TimeManager.Instance.Resume();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu_testv2");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Resume();

            gameObject.SetActive(false);
        }
    }
}
