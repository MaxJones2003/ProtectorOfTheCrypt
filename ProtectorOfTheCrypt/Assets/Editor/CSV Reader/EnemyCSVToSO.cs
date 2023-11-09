using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyCSVToSO : MonoBehaviour
{
    private static readonly string SAVE_FOLDER_Editor = Application.dataPath + "/Stats";
    private static readonly string CSV_File = "/EnemyStats.csv";

    [MenuItem("Utilities/Generate Enemies")]
    public static void GenerateWeapons()
    {
        string[] allLines = null;

        if (File.Exists(SAVE_FOLDER_Editor + CSV_File))
        {
            allLines = File.ReadAllLines(SAVE_FOLDER_Editor + CSV_File);
        }

        foreach (string s in allLines)
        {
            string[] splitData = s.Split(',');

            EnemyScriptableObject enemy = ScriptableObject.CreateInstance<EnemyScriptableObject>();

            enemy.name = splitData[0];
            enemy.Description = splitData[1];
            enemy.BaseHealth = int.Parse(splitData[2]);
            enemy.BaseSpeed = int.Parse(splitData[3]);
            enemy.Hunger = int.Parse(splitData[4]);

            if(splitData[5] == "Yes")
            {
                // Shield stuff
            }
            AssetDatabase.SaveAssets();
        }

    }
}
