using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FloatButton : MonoBehaviour
{
    public UnityEvent OnButtonFilled;
    [SerializeField] Animator buttonAnimator = default;

    bool isClicking = false;
    bool isKeyInputing = false;


    [SerializeField] List<Image> imagesToFill = new List<Image>();
    [SerializeField] float timerToFill;
    [SerializeField] float timer = 0;
    [SerializeField] AnimationCurve fillCurve;
    [SerializeField] Color fillColor = Color.yellow;

    bool canFill = false;

    private void Start()
    {
        foreach(Image im in imagesToFill)
        {
            im.color = fillColor;
            im.fillAmount = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!canFill)
        {
            return;
        }

        //float toThisValue = 0;
        if (isClicking || isKeyInputing) 
        { 
            //toThisValue = 1;
            timer += Time.deltaTime;
        }
        else
        {
            timer -= Time.deltaTime;
        }

        timer = Mathf.Clamp(timer, 0.0f, timerToFill);

        foreach (Image myImage in imagesToFill)
        {
            float fillAmount = Mathf.Lerp(0, 1, fillCurve.Evaluate(timer / timerToFill));
            myImage.fillAmount = fillAmount;

            if (fillAmount >= 0.99f)
            {
                OnButtonFilled.Invoke();
                Restart();
            }
        }
    }

    public void Restart() 
    {
        canFill = false;
        foreach (Image myImage in imagesToFill)
            myImage.fillAmount = 0;
        timer = 0.0f;
        isKeyInputing = false;

    }

    public void SetClickStatement(bool value)
    {
        if(isClicking == false && value && !canFill)
        {
            canFill = true;
        }

        isClicking = value;
    }

    public void PassTurn()
    {
        TurnManager.Instance.EndPlayerTurn();
    }

    bool _hover;
    public void SetHover(bool hover)
    {
        _hover = hover;
        buttonAnimator.SetBool("Hover", _hover || isKeyInputing);
    }

    public void ChangeIsKeyInputing(bool keyInputing)
    {
        isKeyInputing = keyInputing;
        canFill = true;
        buttonAnimator.SetBool("Hover", _hover || isKeyInputing);
    }
}
