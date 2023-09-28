using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SAVE_FOLDER = System.IO.Directory.GetCurrentDirectory() + "/Levels";

    public static void Save(string saveString, string fileName)
    {
        File.WriteAllText(SAVE_FOLDER + fileName + ".txt", saveString);
    }

    public static string Load(string fileName)
    {
        if(File.Exists(SAVE_FOLDER + fileName + ".txt")) 
        {
            string saveString = File.ReadAllText(SAVE_FOLDER + fileName + ".txt");
            return saveString;
        }
        return null;
    }
}
