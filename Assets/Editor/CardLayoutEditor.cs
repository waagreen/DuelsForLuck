using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HorizontalCardLayout))]
public class CardLayoutEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HorizontalCardLayout layoutScript = (HorizontalCardLayout)target;
        EditorGUILayout.Space(10); 
        if (GUILayout.Button("Update Layout (Editor)"))
        {
            layoutScript.UpdateLayout();
        }
    }
}