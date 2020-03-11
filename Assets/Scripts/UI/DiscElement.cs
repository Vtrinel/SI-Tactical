using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscElement : MonoBehaviour
{
    [SerializeField] List<Image> AllSprite = new List<Image>();

    [SerializeField] Text textValue;

    public void SetIcon(int indexSprite, int number)
    {
        RefreshImage(indexSprite);
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
