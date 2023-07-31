using System;
using OToon;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public Button[] buttons;
    public Image[] image;

    private void Awake()
    {
        
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel",1);

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
            
        }

        for (int i = 0; i < unlockedLevel; i++)
        {
            buttons[i].interactable = true;
        }
    }

    public void OpenLevel(int levelid)
    {
        string levelname = "Level" + levelid;
        SceneManager.LoadScene(levelname);
    }

    public void UnLockNewLevel()
    {
        
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }
    
}
