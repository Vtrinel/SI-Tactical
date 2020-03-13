using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanel : MonoBehaviour
{
    public void Resume()
    {
        print("test");
        TimeManager.Instance.Resume();

        gameObject.SetActive(false);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu_testv2");
    }

}
