using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int maxCastleHealth = 100;
    private int castleHealth;
    public int playerMoney = 0;
    public GameManagerUI gameManagerUI;

    void Start()
    {
        gameManagerUI = FindObjectOfType<GameManagerUI>();
        castleHealth = maxCastleHealth;
    }

    public int GetCastleHealth()
    {
        return castleHealth;
    }

    public int GetMaxCastleHealth()
    {
        return maxCastleHealth;
    }

    public int GetPlayerMoney()
    {
        return playerMoney;
    }

    public void DamageCastle(int damageAmount)
    {
        castleHealth -= damageAmount;
        
        if (castleHealth <= 0)
        {
            castleHealth = 0;
            GameOver();
        }
    }

    public void EarnMoney(int amount)
    {
        playerMoney += amount;
    }

    public void LoseMoney(int amount)
    {
        playerMoney -= amount;
        if (playerMoney < 0)
        {
            playerMoney = 0;
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over - Castle Destroyed!");
    }
}