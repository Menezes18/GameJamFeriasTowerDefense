using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Nome da cena que será carregada quando o botão "Play" for pressionado
    public string sceneToLoad = "Game1";

    // Método chamado quando o botão "Play" é pressionado
    public void OnPlayButtonPressed()
    {
        // Carrega a cena do jogo
        SceneManager.LoadScene(sceneToLoad);
        Debug.Log("play");
    }

    // Método chamado quando o botão "Quit" é pressionado
    public void OnQuitButtonPressed()
    {
        // Sai do jogo (funciona apenas na build final, não no editor)
        Application.Quit();
    }

    public void menu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SelectLevel");
    }
        public void menuprin()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
}