using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscElement : MonoBehaviour
{
    [SerializeField] List<Image> AllSprite = new List<Image>();

    [SerializeField] Text textValue;

    [SerializeField] TooltipColliderUI tooltipCollider = default;

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

        tooltipCollider.SetTooltipInformations(TooltipInformationFactory.GetDiscTypeInformations(DiscManager.Instance.GetDiscInformations(discType)));
        tooltipCollider.SetTooltipable(discType != DiscType.None);
    }

    void RefreshImage(int index)
    {
        foreach(Image img in AllSprite)
        {
            img.gameObject.SetActive(false);
        }

        if (index - 1 < AllSprite.Count && index - 1 >= 0)
            AllSprite[index - 1].gameObject.SetActive(true);
    }
}
