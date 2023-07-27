using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    public GameObject buildingPrefab; // Reference to the prefab you want to spawn

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // You can change the input to whatever you prefer
        {
            SpawnBuilding();
        }
    }

    private void SpawnBuilding()
    {
        // Instantiate the prefab and get the Building component (assuming the prefab has the Building script attached)
        GameObject newBuildingObj = Instantiate(buildingPrefab, transform.position, Quaternion.identity);
        Building newBuilding = newBuildingObj.GetComponent<Building>();

        // Replace the following lines with your initialization logic if required
        BuildingData buildingData = new BuildingData(); // You may have your own BuildingData setup
        BuildingSaveData buildingSaveData = new BuildingSaveData("BuildingName", buildingData, transform.position, Quaternion.identity);
        newBuilding.Init(buildingData, buildingSaveData);
        newBuilding.PlaceBuilding();
    }
}