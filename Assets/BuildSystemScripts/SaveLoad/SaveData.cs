using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public List<BuildingSaveData> BuildingSaveData;

    public SaveData()
    {
        BuildingSaveData = new List<BuildingSaveData>();
    }
}
