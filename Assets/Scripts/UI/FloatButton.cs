using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FloatButton : MonoBehaviour
{
    public UnityEvent OnButtonFilled;

    bool isClicking = false;

    [SerializeField] Image myImage;
    [SerializeField] float timerToFill;
    [SerializeField] float timer = 0;
    [SerializeField] AnimationCurve fillCurve;

    bool canFill = false;

    // Update is called once per frame
    void Update()
    {
        if (!canFill)
        {
            return;
        }

        //float toThisValue = 0;
        if (isClicking) 
        { 
            //toThisValue = 1;
            timer += Time.deltaTime;
        }
        else
        {
            timer -= Time.deltaTime;
        }

        timer = Mathf.Clamp(timer, 0.0f, timerToFill);

        myImage.fillAmount = Mathf.Lerp(0, 1, fillCurve.Evaluate(timer/timerToFill));

        if(myImage.fillAmount >= 0.99f)
        {
            OnButtonFilled.Invoke();
            Restart();
        }
    }

    void Restart() 
    {
        canFill = false;
        myImage.fillAmount = 0;
        timer = 0.0f;

        print("fin de tour");

    }

    public void SetClickStatement(bool value)
    {
        if(isClicking == false && value && !canFill)
        {
            canFill = true;
        }

        isClicking = value;
    }
}
