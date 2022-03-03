using UnityEngine;
using UnityEditor;

// [CustomEditor(typeof(SlantedImage))]
[CanEditMultipleObjects]
public class SlantedImageEditor : Editor 
{
    SerializedProperty skewX;
    SerializedProperty skewY;
    
    void OnEnable()
    {
        skewX = serializedObject.FindProperty("skewX");
        skewY = serializedObject.FindProperty("skewY");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(skewX);
        EditorGUILayout.PropertyField(skewY);
        serializedObject.ApplyModifiedProperties();
    }
}