using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUtilities : MonoBehaviour
{
    public bool statut = false;

    public Image myIcon;
    public Image myOutline;

    public Color selectedColor = Color.red;

    public ActionType myActionType;

    public AudioSource myAudioSource;
    public AudioClip clickButtonSound;

    private void OnEnable()
    {
        switch (myActionType)
        {
            case ActionType.None:
                return;

            case ActionType.Move:
                GameManager.Instance.OnMoveActionSelectionStateChanged += ActiveOrDisable;
                break;

            case ActionType.Throw:
                GameManager.Instance.OnThrowCompetenceSelectionStateChanged += ActiveOrDisable;
                break;

            case ActionType.Recall:
                GameManager.Instance.OnRecallCompetenceSelectionStateChanged += ActiveOrDisable;
                break;

            case ActionType.Special:
                GameManager.Instance.OnSpecialCompetenceSelectionStateChanged += ActiveOrDisable;
                break;
        }
    }

    private void OnDisable()
    {
        switch (myActionType)
        {
            case ActionType.None:
                return;

            case ActionType.Move:
                GameManager.Instance.OnMoveActionSelectionStateChanged -= ActiveOrDisable;
                break;

            case ActionType.Throw:
                GameManager.Instance.OnThrowCompetenceSelectionStateChanged -= ActiveOrDisable;
                break;

            case ActionType.Recall:
                GameManager.Instance.OnRecallCompetenceSelectionStateChanged -= ActiveOrDisable;
                break;

            case ActionType.Special:
                GameManager.Instance.OnSpecialCompetenceSelectionStateChanged -= ActiveOrDisable;
                break;
        }
    }


    public void Active()
    {
        statut = true;

        myIcon.color = selectedColor;
        myOutline.color = selectedColor;

        myAudioSource.PlayOneShot(clickButtonSound);

        myOutline.gameObject.SetActive(true);
    }

    public void Disable()
    {
        statut = false;

        myIcon.color = Color.white;
        myOutline.color = Color.white;

        myOutline.gameObject.SetActive(false);
    }

    public void OnClickButton()
    {
        ActiveOrDisable(!statut);
        GameManager.Instance.SelectAction(myActionType);
    }

    void ActiveOrDisable(bool value)
    {
        if (value)
        {
            Active();
            print("yes");
        }
        else
        {
            Disable();
            print("no");
        }
    }
}