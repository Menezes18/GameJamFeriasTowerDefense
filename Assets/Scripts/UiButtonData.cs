using System;
using UnityEngine;
using UnityEngine.UI;

public class UiButtonData : MonoBehaviour
{
    public BuildingData[] towers;
    public BuildTool buildTool;
    public UIManager uiManager;
    public GameManager gameManager;
    public int[] towerCosts;
    public int money;

    public GameObject pop;

    public void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        gameManager = FindObjectOfType<GameManager>();
        money = gameManager.playerMoney;
    }

    public void OnButtonClick(int towerIndex)
    {
        if (towerIndex >= 0 && towerIndex < towers.Length)
        {
            int towerCost = towerCosts[towerIndex];

            // Verifique se o jogador tem dinheiro suficiente
            if (money >= towerCost)
            {
                buildTool.SetCurrentBuildingData(towers[towerIndex]);
                gameManager.LoseMoney(towerCost);
                buildTool.ActivatePreviewAndPrefab();
                buildTool.towerActivated = true;
                uiManager.BuildPanel.SetActive(false);
                uiManager.SetMouseCursorState(false);

                // Atualize o valor do dinheiro (money) após a compra da torre
                money -= towerCost;
            }
            else
            {
                pop.SetActive(true);
                Invoke("desativarpop", 1.8f);
            }
        }
        else
        {
            Debug.LogError("Índice de torre inválido.");
        }
    }


    public void desativarpop()
    {
        pop.SetActive(false);
    }
}