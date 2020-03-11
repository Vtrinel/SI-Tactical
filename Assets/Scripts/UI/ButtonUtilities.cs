using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUtilities : MonoBehaviour
{
    public bool statut = false;

    public Image myIcon;
    public Text costText = default;

    public Color selectedColor = Color.red;

    public ActionType myActionType;

    public AudioSource myAudioSource;
    public AudioClip clickButtonSound;

    Competence linkedCompetence = default;
    [SerializeField] TooltipColliderUI tooltipCollider = default;

    private void Awake()
    {
        if(myActionType == ActionType.Move)
            costText.text = "X";
    }

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

        if (myActionType == ActionType.Recall || myActionType == ActionType.Throw || myActionType == ActionType.Special)
        {
            PlayerExperienceManager.Instance.OnSetChanged += UpdateLinkedCompetence;
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

        if (myActionType == ActionType.Recall || myActionType == ActionType.Throw || myActionType == ActionType.Special)
        {
            PlayerExperienceManager.Instance.OnSetChanged -= UpdateLinkedCompetence;
        }
    }


    public void Active()
    {
        statut = true;

        myIcon.color = selectedColor;

        myAudioSource.PlayOneShot(clickButtonSound);
    }

    public void Disable()
    {
        statut = false;

        myIcon.color = Color.white;
    }

    public void OnClickButton()
    {
        GameManager.Instance.SelectAction(myActionType);
    }

    void ActiveOrDisable(bool value)
    {
        if (value)
        {
            Active();
        }
        else
        {
            Disable();
        }
    }


    public void UpdateLinkedCompetence(CompetenceThrow throwComp, CompetenceRecall recallComp, CompetenceSpecial specialCompetence)
    {
        switch (myActionType)
        {
            case ActionType.Throw:
                linkedCompetence = throwComp;
                break;
            case ActionType.Recall:
                linkedCompetence = recallComp;
                break;
            case ActionType.Special:
                linkedCompetence = specialCompetence;
                break;
        }

        tooltipCollider.SetTooltipInformations(TooltipInformationFactory.GetUsableCompetenceTooltip(linkedCompetence));
        costText.text = linkedCompetence.GetActionPointsCost.ToString();
        if (linkedCompetence.GetCompetenceImage != null)
            myIcon.sprite = linkedCompetence.GetCompetenceImage;
    }
}