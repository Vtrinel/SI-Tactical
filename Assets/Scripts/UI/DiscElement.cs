using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscElement : MonoBehaviour
{
    [SerializeField] List<Image> AllSprite = new List<Image>();

    [SerializeField] Text textValue;

    public DiscType discType = DiscType.None;

    public void SetIcon(int indexSprite, int number)
    {
        RefreshImage(indexSprite);
        textValue.text = number.ToString();

        discType = (DiscType)indexSprite;
    }

    void RefreshImage(int index)
    {
        foreach(Image img in AllSprite)
        {
            img.gameObject.SetActive(false);
        }

        AllSprite[index].gameObject.SetActive(true);
    }
}
