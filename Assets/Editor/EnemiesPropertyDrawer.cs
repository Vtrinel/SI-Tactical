using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SpawnPointEnemyCouple))]
public class SpawnPointEnemyCouplePropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 1.2f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float spaceBewteen = 2f;
        float thirdWidth = (position.width - spaceBewteen) * 0.33f;

        SerializedProperty enemyTypeProperty = property.FindPropertyRelative("enemyType");
        SerializedProperty spawnPointProperty = property.FindPropertyRelative("spawnPoint");
        SerializedProperty attachedDiscTypeProperty = property.FindPropertyRelative("attachedDiscType");

        Rect enemyTypeRect = new Rect(position.x, position.y, thirdWidth, EditorGUIUtility.singleLineHeight);
        Rect spawnPointRect = new Rect(position.x + thirdWidth + spaceBewteen, position.y, thirdWidth, EditorGUIUtility.singleLineHeight);
        Rect attachedDiscTypeRect = new Rect(position.x + (thirdWidth + spaceBewteen) * 2, position.y, thirdWidth, EditorGUIUtility.singleLineHeight);

        EditorGUI.PropertyField(enemyTypeRect, enemyTypeProperty, GUIContent.none);
        EditorGUI.PropertyField(spawnPointRect, spawnPointProperty, GUIContent.none);
        EditorGUI.PropertyField(attachedDiscTypeRect, attachedDiscTypeProperty, GUIContent.none);
    }
}