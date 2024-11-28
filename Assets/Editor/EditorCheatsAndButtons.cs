using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Inventory))]
public class EditorCheatsAndButtons : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Add a button to the inspector
        Inventory myScript = (Inventory)target;
        if (GUILayout.Button("Add 10 Corn"))
        {
            myScript.AddPlant("Corn", 10);
        }

        if (GUILayout.Button("Add $10"))
        {
            myScript.AddCash(10);
        }


    }
}
