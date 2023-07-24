using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
    public static SaveData Data;

    private void Awake()
    {
        SaveLoad.OnLoadGame += LoadData;
        Data = new SaveData();
    }

    public static void SaveData()
    {
        var saveData = Data;
        SaveLoad.Save(saveData);
        Debug.Log("Save key has been pressed...");
    }

    private static void LoadData(SaveData saveData)
    {
        Data = saveData;
        Debug.Log("Save data loaded...");
    }

    public static void TryLoadData()
    {
        Debug.Log("Trying to load data...");
        SaveLoad.Load();
    }

    private void OnDestroy()
    {
        //SaveLoad.OnLoadGame -= LoadData;
    }
}
