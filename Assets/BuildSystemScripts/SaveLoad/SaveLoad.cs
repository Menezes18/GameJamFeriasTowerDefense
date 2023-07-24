using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public static class SaveLoad
{
    public static UnityAction OnSaveGame;
    public static UnityAction<SaveData> OnLoadGame;

    public static string directory = "/SaveData/";
    public static string fileName = "SaveGame.sav";

    public static void Save(SaveData data)
    {
        OnSaveGame?.Invoke();

        string dir = Application.persistentDataPath + directory;

        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(dir + fileName, json);
        
        Debug.Log("Saving game...");
    }

    public static void Load()
    {
        string fullPath = Application.persistentDataPath + directory + fileName;
        

        if (File.Exists(fullPath))
        {
            SaveData data = new SaveData();
            string json = File.ReadAllText(fullPath);
            data = JsonUtility.FromJson<SaveData>(json);
            
            OnLoadGame?.Invoke(data);
            Debug.Log("Loading save...");
        }
        else
        {
            Debug.Log("Save file does not exist...");
        }
    }
}
