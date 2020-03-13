using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiscElement : MonoBehaviour
{
    [SerializeField] List<Image> AllSprite = new List<Image>();
    [SerializeField] List<Image> AllImages = new List<Image>();
    [SerializeField] List<Text> AllTexts = new List<Text>();

    [SerializeField] TextMeshProUGUI textValue;

    [SerializeField] TooltipColliderUI tooltipCollider = default;
    [SerializeField] Animator buttonAnimator = default;

    public DiscType discType = DiscType.None;

    private void Awake()
    {
        tooltipCollider.SetTooltipable(discType != DiscType.None);
    }

    public void SetIcon(int indexSprite, int number)
    {
        RefreshImage(indexSprite);
        textValue.text = number.ToString();

        discType = (DiscType)indexSprite;

        tooltipCollider.SetTooltipInformations(TooltipInformationFactory.GetDiscTypeInformations(DiscManager.Instance.GetDiscInformations(discType),
            tooltipCollider.GetTooltipInformations.forcedTooltipLPosition, tooltipCollider.GetTooltipInformations.tooltipForcedPositionType));
        tooltipCollider.SetTooltipable(discType != DiscType.None);
    }

    void RefreshImage(int index)
    {
        foreach (Image img in AllSprite)
        {
            img.gameObject.SetActive(false);
        }

        if (index - 1 < AllSprite.Count && index - 1 >= 0)
            AllSprite[index - 1].gameObject.SetActive(true);
    }

    public void StartTooltiped()
    {
        buttonAnimator.SetBool("Tooltiped", true);
    }

    public void EndTooltip()
    {
        buttonAnimator.SetBool("Tooltiped", false);
    }

    public void Select(Color selectionColor)
    {
        buttonAnimator.SetBool("Selected", true);

        foreach (Image im in AllImages)
        {
            im.color = selectionColor;
        }
        foreach (Text tx in AllTexts)
        {
            tx.color = selectionColor;
        }

    }

    public void Unselect()
    {
        buttonAnimator.SetBool("Selected", false);

        foreach (Image im in AllImages)
        {
            im.color = Color.white;
        }
        foreach (Text tx in AllTexts)
        {
            tx.color = Color.white;
        }
    }

    private void OnEnable()
    {
        buttonAnimator.SetBool("Usable", true);
        tooltipCollider.OnStartTooltip += StartTooltiped;
        tooltipCollider.OnEndTooltip += EndTooltip;
    }

    private void OnDisnable()
    {
        buttonAnimator.SetBool("Usable", true);
        tooltipCollider.OnStartTooltip -= StartTooltiped;
        tooltipCollider.OnEndTooltip -= EndTooltip;
    }
}
