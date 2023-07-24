using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
    public int _defaultLayerInt;
    private void OnEnable()
    {
        SaveLoad.OnLoadGame += LoadBase;
    }

    private void OnDisable()
    {
        SaveLoad.OnLoadGame -= LoadBase;
    }

    private void LoadBase(SaveData data)
    {
        Debug.Log($"Loading base {data.BuildingSaveData.Count}");
        foreach (var building in data.BuildingSaveData)
        {
            var go = new GameObject
            {
                layer = _defaultLayerInt,
                name = building.BuildingName
            };

            var _spawnedBuilding = go.AddComponent<Building>();
            _spawnedBuilding.Init(building.AssignedData, building);
            _spawnedBuilding.transform.rotation = building.Rotation;
            _spawnedBuilding.transform.position = building.Position;
            
            _spawnedBuilding.PlaceBuilding();
        }
    }
}
