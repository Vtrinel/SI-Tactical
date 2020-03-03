using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    #region Dummy public attributes

    public int maxHealth = 3;
    public int currentHealth = 3;
    public int actionPoint = 3;

    #endregion


    public UnityAction<float> OnPlayerTakeDamage;

    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #region Menu interactions

    public void Menu()
    {
        Debug.Log("Menu");
    }

    public void EndTurn()
    {
        Debug.Log("EndTurn");
    }

    public void UndoMove()
    {
        Debug.Log("UndoMove");
    }

    #endregion


    #region Skills

    public void Move()
    {
        Debug.Log("Move");
    }

    public void Special()
    {
        Debug.Log("Special");
    }

    public void Launch()
    {
        Debug.Log("Launch");
    }

    public void Recall()
    {
        Debug.Log("Recall");
    }

    public void Swap()
    {
        Debug.Log("Swap");

        OnPlayerTakeDamage(100256.6f);
    }

    #endregion
}
