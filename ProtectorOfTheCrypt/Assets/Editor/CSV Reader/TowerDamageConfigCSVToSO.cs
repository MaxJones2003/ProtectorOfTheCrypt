using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TowerDamageConfigCSVToSO : MonoBehaviour
{
    private static readonly string SAVE_FOLDER_Editor = Application.dataPath + "/Stats";
    private static readonly string CSV_File = "/TowerDamageConfigStats.csv";

    [MenuItem("Utilities/Generate Towers")]
    public static void GenerateTowers()
    {
        string[] allLines = null;

        if (File.Exists(SAVE_FOLDER_Editor + CSV_File))
        {
            allLines = File.ReadAllLines(SAVE_FOLDER_Editor + CSV_File);
        }

        foreach (string s in allLines)
        {
            string[] splitData = s.Split(',');
            TowerScriptableObject damageConfig = ScriptableObject.CreateInstance<TowerScriptableObject>();

            damageConfig.name = splitData[0];
            damageConfig.AOEDamage = float.Parse(splitData[1]);
            damageConfig.AOERange = float.Parse(splitData[2]);

            ////damageConfig.BulletSpeed = float.Parse(splitData[3]);
            ////damageConfig.FireRate = float.Parse(splitData[4]);
            ////damageConfig.Range = float.Parse(splitData[5]);

            AssetDatabase.SaveAssets();
        }
    }

}
