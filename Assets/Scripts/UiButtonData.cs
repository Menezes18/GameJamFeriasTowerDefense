using System;
using UnityEngine;
using UnityEngine.UI;

public class UiButtonData : MonoBehaviour
{
    public BuildingData[] towers;
    public BuildTool buildTool;
    public UIManager uiManager;

    public void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    public void OnButtonClick(int towerIndex)
    {
        if (towerIndex >= 0 && towerIndex < towers.Length)
        {
            buildTool.SetCurrentBuildingData(towers[towerIndex]);
            buildTool.ActivatePreviewAndPrefab();
            buildTool.towerActivated = true;
            uiManager.BuildPanel.SetActive(false);
            uiManager.SetMouseCursorState(false);

        }
        else
        {
            Debug.LogError("Invalid tower index.");
        }
    }


    public void test()
    {
        Debug.Log("Aaaaaaa");
    }
}