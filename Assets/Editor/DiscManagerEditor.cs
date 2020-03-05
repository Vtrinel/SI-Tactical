using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(DiscManager))]
public class DiscManagerEditor : Editor
{
    DiscManager myTarget;

    float utilisation;
    int used = 0;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        GUIStyle styleEnable = EditorStyles.miniButton;

        float percent = (float)used / (float)myTarget.allDiscs.Count * 100;
        GUILayout.Space(18);

        Rect r = EditorGUILayout.BeginVertical();
        EditorGUI.ProgressBar(r, ((float)Math.Round(percent) / 100), Math.Round(percent) + " %");
        GUILayout.Space(18);
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        //utilisation = EditorGUILayout.FloatField("Experience", percent);

        //[Space(10)]

        used = 0;
        GUILayout.BeginHorizontal();
        int i = 1;
        foreach (DiscScript element in myTarget.allDiscs)
        {
            if (element.gameObject.activeSelf)
            {
                GUI.backgroundColor = Color.green;
                used++;
            }
            else
            {
                GUI.backgroundColor = Color.red;
            }
            GUILayout.Button("ennemy", styleEnable, GUILayout.MaxWidth(Screen.width / 4.5f));

            if (i == 4)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                i = 0;
            }

            i++;
        }
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;

        serializedObject.ApplyModifiedProperties();
    }

    private void OnEnable()
    {
        myTarget = (DiscManager)target;

        myTarget.allDiscs.Clear();

        foreach (Transform child in myTarget.transform)
        {
            myTarget.allDiscs.Add(child.GetComponent<DiscScript>());
        }
    }
}