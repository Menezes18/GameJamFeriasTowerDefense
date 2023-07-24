using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class BuildingPanelUI : MonoBehaviour
{
    public BuildingSideUI SideUI;
    public static UnityAction<BuildingData> OnPartChosen;

    public BuildingData[] KnownBuildingParts;
    public BuildingPartUI BuildingButtonPrefab;

    public GameObject ItemWindow;

    public void OnClick(BuildingData chosenData)
    {
        OnPartChosen?.Invoke(chosenData);
        SideUI.UpdateSideDisplay(chosenData);
    }

    public void OnClickAllParts()
    {
        PopulateButtons();
    }

    public void OnClickRoomParts()
    {
        PopulateButtons(PartType.Room);
    }

    public void OnClickCorridorParts()
    {
        PopulateButtons(PartType.Corridor);
    }

    public void PopulateButtons()
    {
        SpawnButtons(KnownBuildingParts);
    }
    
    public void PopulateButtons(PartType chosenPartType)
    {
        var BuildingPieces = KnownBuildingParts.Where(p => p.PartType == chosenPartType).ToArray();
        SpawnButtons(BuildingPieces);
    }

    public void SpawnButtons(BuildingData[] buttonData)
    {
        ClearButtons();
        
        foreach (var data in buttonData)
        {
            var spawnedButton = Instantiate(BuildingButtonPrefab, ItemWindow.transform);
            spawnedButton.Init(data, this);
        }
    }

    public void ClearButtons()
    {
        foreach (var button in ItemWindow.transform.Cast<Transform>())
        {
            Destroy(button.gameObject);
        }
    }
}
