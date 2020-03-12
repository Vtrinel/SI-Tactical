using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUtilities : MonoBehaviour
{
    public bool statut = false;

    public Image myIcon;
    public Text costText = default;

    public Image[] allIcons = new Image[0];
    public Text[] allTexts = new Text[0];

    public Color selectedColor = Color.red;
    public Color unusableColor = Color.grey;

    public ActionType myActionType;

    Competence linkedCompetence = default;
    [SerializeField] TooltipColliderUI tooltipCollider = default;
    [SerializeField] Animator competenceButtonAnimator = default;

    private void Awake()
    {
        if(myActionType == ActionType.Move)
            costText.text = "";
    }

    private void OnEnable()
    {
        GameManager.Instance.OnCompetencesUsableChanged += CheckUsability;

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

        if(tooltipCollider != null)
        {
            tooltipCollider.OnStartTooltip += StartTooltiped;
            tooltipCollider.OnEndTooltip += EndTooltip;
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.OnCompetencesUsableChanged -= CheckUsability;

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

        if (tooltipCollider != null)
        {
            tooltipCollider.OnStartTooltip -= StartTooltiped;
            tooltipCollider.OnEndTooltip -= EndTooltip;
        }
    }


    public void Active()
    {
        statut = true;

        UpdateColor();

        SoundManager.Instance.PlaySound(Sound.SelectCompetence, Camera.main.transform.position);
        competenceButtonAnimator.SetBool("Selected", true);
    }

    public void Disable()
    {
        statut = false;

        UpdateColor();
        competenceButtonAnimator.SetBool("Selected", false);
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

        tooltipCollider.SetTooltipInformations(TooltipInformationFactory.GetUsableCompetenceTooltip(linkedCompetence, tooltipCollider.GetTooltipInformations.forcedTooltipLPosition, 
            tooltipCollider.GetTooltipInformations.tooltipForcedPositionType));
        costText.text = linkedCompetence.GetActionPointsCost.ToString();
        if (linkedCompetence.GetCompetenceImage != null)
            myIcon.sprite = linkedCompetence.GetCompetenceImage;
    }

    public void StartTooltiped()
    {
        competenceButtonAnimator.SetBool("Tooltiped", true);

    }

    public void EndTooltip()
    {
        competenceButtonAnimator.SetBool("Tooltiped", false);
    }

    bool usable = false;
    public void CheckUsability(List<bool> usables)
    {
        switch (myActionType)
        {
            case ActionType.Move:
                usable = usables[0];
                break;
            case ActionType.Throw:
                usable = usables[1];
                break;
            case ActionType.Recall:
                usable = usables[2];
                break;
            case ActionType.Special:
                usable = usables[3];
                break;
        }
        competenceButtonAnimator.SetBool("Usable", usable);
        UpdateColor();
    }

    public Color GetCurrentColor => !usable ? unusableColor : statut? selectedColor : Color.white;

    public void UpdateColor()
    {
        foreach (Image im in allIcons)
        {
            im.color = GetCurrentColor;
        }
        foreach (Text tx in allTexts)
        {
            tx.color = GetCurrentColor;
        }
    }
}