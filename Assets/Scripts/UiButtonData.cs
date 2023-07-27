using System;
using UnityEngine;
using UnityEngine.UI;

public class UiButtonData : MonoBehaviour
{
    public BuildingData[] towers;
    public BuildTool buildTool;
    public UIManager uiManager;
    public GameManager gameManager;
    public int money;

    public void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnButtonClick(int towerIndex)
    {
        if (towerIndex >= 0 && towerIndex < towers.Length)
        {
            buildTool.SetCurrentBuildingData(towers[towerIndex]);
            gameManager.LoseMoney(money);
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