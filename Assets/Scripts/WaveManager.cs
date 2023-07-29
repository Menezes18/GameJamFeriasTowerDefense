using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    public List<Transform> spawnPoints = new List<Transform>();
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    public int maxWaves = 50;
    public int baseNumEnemies = 1;
    public int maxExtraEnemiesPerWave = 2;
    public float waveInterval = 5f;
    public int currentWaveEnemiesAlive;
    public bool automatico = true;
    
    public int currentWave = 0;

    public GameManagerUI gamemanagerui;

    private void Start()
    {
        SpawnWave();
        gamemanagerui = FindObjectOfType<GameManagerUI>();
        //InvokeRepeating("SpawnWave", waveInterval, waveInterval);
    }

    private void SpawnWave()
    {
        if (currentWave < maxWaves)
        {
            currentWave++;
            int numEnemies = baseNumEnemies + Random.Range(0, maxExtraEnemiesPerWave * currentWave);
            currentWaveEnemiesAlive = numEnemies; // Definir a quantidade de inimigos vivos para a nova onda.
            SpawnEnemies(numEnemies);
        }
        else
        {
            Debug.Log("Max waves reached. Game Over!");
            // Aqui você pode adicionar a lógica para terminar o jogo ou mostrar uma tela de vitória.
        }
    }

    public void Update()
    {
        EnemyDefeated();
        if (Keyboard.current.tKey.wasPressedThisFrame && currentWaveEnemiesAlive <= 0)
        {
            Debug.Log("manual");
            SpawnWave();
        }
        ShowWaveProgress();
    }

    private void SpawnEnemies(int numEnemies)
    {
        for (int i = 0; i < numEnemies; i++)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Transform spawnPoint = spawnPoints[spawnIndex];

            GameObject enemyPrefab = GetRandomEnemyPrefab();
            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    public void EnemyDefeated()
    {
        
        if (automatico && currentWaveEnemiesAlive <= 0)
        {
            // Todos os inimigos da onda atual foram derrotados.
            // Inicie a próxima onda.
            SpawnWave();
            waveInterval--;
            if (waveInterval <= 0 )
            {
                Debug.Log("Acabou");
            }
        }
        
    }
    public void ShowWaveProgress()
    {
        Debug.Log($"Wave {currentWave}/{maxWaves}");
        
    }
    private GameObject GetRandomEnemyPrefab()
    {
        int randomIndex = Random.Range(0, enemyPrefabs.Count);
        return enemyPrefabs[randomIndex];
    }
}