using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class CameraTest : MonoBehaviour
{
    [SerializeField] GameObject[] cameras;
    [SerializeField] Color textColor = Color.red;
    [SerializeField] int camID;
    [SerializeField] KeyCode key;

    private void Start()
    {
        SetCamera();
    }

    private void Update()
    {
        if (Input.GetKeyDown(key))
        {
            SwitchCam();
        }
    }

    [Button]
    void SwitchCam()
    {
        camID++;
        if (camID >= cameras.Length) camID = 0;
        SetCamera();
    }

    void SetCamera()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].SetActive(i == camID);
        }
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperRight;
        style.fontSize = h * 4 / 100;
        style.normal.textColor = textColor;
        string text = "Camera ID : " + camID.ToString();
        GUI.Label(rect, text, style);
    }
}
