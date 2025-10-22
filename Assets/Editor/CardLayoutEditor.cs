using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardLayout))]
public class CardLayoutEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CardLayout layoutScript = (CardLayout)target;
        EditorGUILayout.Space(10); 
        if (GUILayout.Button("Update Layout (Editor)"))
        {
            layoutScript.UpdateLayout();
        }
    }
}