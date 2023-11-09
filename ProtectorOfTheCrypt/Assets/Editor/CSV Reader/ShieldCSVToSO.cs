using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShieldCSVToSO : MonoBehaviour
{
    private static readonly string SAVE_FOLDER_Game = System.IO.Directory.GetCurrentDirectory() + "/Stats/";
    private static readonly string SAVE_FOLDER_Editor = Application.dataPath + "/Stats/";
    private static readonly string CSV_File = "ShieldStats";

    [MenuItem("Utilities/Generate Shields")]
    public static void GenerateWeapons()
    {
        string[] allLines = null;
#if UNITY_EDITOR
        if (File.Exists(SAVE_FOLDER_Editor + CSV_File + ".csv"))
        {
            allLines = File.ReadAllLines(SAVE_FOLDER_Editor + CSV_File + ".csv");
        }
#endif
        if (File.Exists(SAVE_FOLDER_Game + CSV_File + ".csv"))
        {
            allLines = File.ReadAllLines(SAVE_FOLDER_Game + CSV_File + ".csv");
        }

        //Name,Description,Capacity,Reload Time,Firing Delay,Damage,Knockback,Spread,Projectiles,Automatic,Price
        foreach (string s in allLines)
        {
            string[] splitData = s.Split(',');

            ShieldScriptableObject shield = ScriptableObject.CreateInstance<ShieldScriptableObject>();

            shield.name = splitData[0];
            shield.Description = splitData[1];
            // shield.BaseShieldHealth = int.Parse(splitData[2]);            // COMMENTED TO AVOID ERROR
        }
    }

}
