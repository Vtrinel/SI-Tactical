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

    bool canFill = false;

    // Update is called once per frame
    void Update()
    {
        if (!canFill)
        {
            return;
        }

        float toThisValue = 0;
        if (isClicking) { toThisValue = 1; }

        myImage.fillAmount = Mathf.Lerp(myImage.fillAmount, toThisValue, timerToFill * Time.deltaTime);

        if(myImage.fillAmount >= 0.99f)
        {
            OnButtonFilled.Invoke();
            Restart();
        }
    }

    void Restart() {
        canFill = false;
        myImage.fillAmount = 0;

        print("fin de tour");

        GameManager.Instance.SelectAction(ActionType.None);
        TurnManager.Instance.EndPlayerTurn();
        CameraManager.instance.GetPlayerCamera.ResetPlayerCamera();
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
