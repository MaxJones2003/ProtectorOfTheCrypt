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
    }
}
