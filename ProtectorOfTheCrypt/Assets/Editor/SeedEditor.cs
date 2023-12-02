using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Seed))]
public class SeedEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Seed seedScript = (Seed)target;

        if(GUILayout.Button("Save Seed"))
        {
            seedScript.SaveCurrentSeed();
        }

        if(GUILayout.Button("Generate Grid"))
        {
            Undo.RecordObject(seedScript.transform.GetChild(1), "Removed Old Path");
            DestroyImmediate(seedScript.transform.GetChild(1).gameObject); // This will remove the child immediately without affecting the scene
            seedScript.InitializeSeedScriptEndlessModeEditor();
        }

        if(GUILayout.Button("Generate Grid With Random Seed"))
        {
            Undo.RecordObject(seedScript.transform.GetChild(1), "Removed Old Path");
            DestroyImmediate(seedScript.transform.GetChild(1).gameObject); // This will remove the child immediately without affecting the scene
            seedScript.InitializeSeedScriptEndlessModeAndRandomizeSeedEditor();
        }
    }
}
