using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartLevelButton : MonoBehaviour
{
    public void Restart()
    {
        GameManager.Instance.RestartGame();
    }
}
