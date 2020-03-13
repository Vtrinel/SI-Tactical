using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldBar : MonoBehaviour
{
    [SerializeField] Image goldBar;
    [SerializeField] Image feedbackBar;

    [SerializeField] Text maxGoldReachedText = default;
    [SerializeField] Image maxGoldReachedImage = default;
    [SerializeField] Color maxGoldReachedShownColor = default;
    [SerializeField] AnimationCurve maxGoldTextApparitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] float maxGoldTextApparitionDuration = 0.3f;
    Color maxGoldReachedHiddenColor= default;
    float maxGoldAnimationCounter = 0;

    float currentGold = 0;
    bool maxGold = false;
    public bool tooltipsDesactived;

    [SerializeField] float timeToFill = 3;

    private void Start()
    {
        tooltipCollider.SetName("Gold : " + PlayerExperienceManager.Instance.GetGoldQuantity + "/" + PlayerExperienceManager.Instance.GetGoldMaxQuantity);
        maxGoldReachedHiddenColor = maxGoldReachedShownColor;
        maxGoldReachedHiddenColor.a = 0;
        maxGoldReachedText.color = maxGoldReachedHiddenColor;
        maxGoldReachedImage.color = maxGoldReachedHiddenColor;
    }

    public void SetGoldValue(float value, float maxValue)
    {
        //currentGold += value;
        currentGold = value / maxValue;
        feedbackBar.fillAmount = currentGold;

        maxGold = currentGold == 1;

        barAnimator.SetTrigger("GainedGold");

        tooltipCollider.SetName("GOLD : " + (value == maxValue ? "MAX" : value + "/" + maxValue));
        tooltipCollider.SetSize(value != maxValue);
    }

    public void ForceSetValue(float value, float maxValue)
    {
        //currentGold = value;
        currentGold = value / maxValue;
        feedbackBar.fillAmount = currentGold;

        barAnimator.SetTrigger("GainedGold");

        tooltipCollider.SetName("GOLD : " + (value == maxValue ? "MAX" : value + "/" + maxValue));
        tooltipCollider.SetSize(value != maxValue);
    }

    // Update is called once per frame
    void Update()
    {
        goldBar.fillAmount = Mathf.Lerp(goldBar.fillAmount, currentGold, Time.deltaTime * timeToFill);

        if((maxGold && maxGoldAnimationCounter < maxGoldTextApparitionDuration) || (!maxGold && maxGoldAnimationCounter > 0))
        {
            maxGoldAnimationCounter += Time.deltaTime * (maxGold ? 1 : -1);
            maxGoldAnimationCounter = Mathf.Clamp(maxGoldAnimationCounter, 0, maxGoldTextApparitionDuration);
            Color newColor = Color.Lerp(maxGoldReachedHiddenColor, maxGoldReachedShownColor, maxGoldTextApparitionCurve.Evaluate(maxGoldAnimationCounter/ maxGoldTextApparitionDuration));
            maxGoldReachedText.color = newColor;
            maxGoldReachedImage.color = newColor;
        }
    }

    private void OnEnable()
    {
        tooltipCollider.OnStartTooltip += StartTooltip;
        tooltipCollider.OnEndTooltip += EndTooltip;
    }

    private void OnDisable()
    {
        tooltipCollider.OnStartTooltip -= StartTooltip;
        tooltipCollider.OnEndTooltip -= EndTooltip;
    }

    [SerializeField] TooltipColliderUI tooltipCollider = default;
    [SerializeField] Animator barAnimator = default;

    public void StartTooltip()
    {
        if(tooltipsDesactived == false)
        barAnimator.SetBool("Tooltiped", true);
    }

    public void EndTooltip()
    {
        if (tooltipsDesactived == false)
            barAnimator.SetBool("Tooltiped", false);
    }
}
