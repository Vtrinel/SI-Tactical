using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldBar : MonoBehaviour
{
    [SerializeField] Image goldBar;
    [SerializeField] Image feedbackBar;

    float currentGold = 0;

    [SerializeField] float timeToFill= 3;

    public void SetGoldValue(float value)
    {
        currentGold += value;
        feedbackBar.fillAmount = currentGold;
    }

    public void ForceSetValue(float value)
    {
        currentGold = value;
        feedbackBar.fillAmount = currentGold;
    }

    // Update is called once per frame
    void Update()
    {
        goldBar.fillAmount = Mathf.Lerp(goldBar.fillAmount, currentGold, Time.deltaTime * timeToFill);
    }
}
