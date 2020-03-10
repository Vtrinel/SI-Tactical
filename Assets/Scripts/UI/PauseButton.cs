using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    public UnityEvent OnButtonFilled;
    public Canvas PauseMenu;

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

        if (myImage.fillAmount >= 0.99f)
        {
            OnButtonFilled.Invoke();
            Pause();
        }
    }

    void Pause()
    {
        canFill = false;
        myImage.fillAmount = 0;

        print("Jeu en pause");

        GameManager.Instance.SelectAction(ActionType.None);
        TimeManager.Instance.Pause();
        CameraManager.instance.GetPlayerCamera.ResetPlayerCamera();
        PauseMenu.gameObject.SetActive(true);
    }

    public void SetClickStatement(bool value)
    {
        if (isClicking == false && value && !canFill)
        {
            canFill = true;
        }

        isClicking = value;
    }
}
