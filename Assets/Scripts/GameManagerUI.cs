using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerUI : MonoBehaviour
{
    public Slider castleHealthSlider;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI waveText;
    public GameObject menuObject;

    private GameManager gameManager;
    private WaveManager waveManager;

    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        gameManager = FindObjectOfType<GameManager>();
        UpdateUI();
    }

    void Update()
    {
        // Verifica constantemente a vida do castelo para atualizar o Slider
        UpdateUI();

        // Verifica se a tecla "P" foi pressionada
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleMenuAndCursor();
        }
    }

    // Método para atualizar a UI do Slider da vida do castelo e do dinheiro do jogador
    private void UpdateUI()
    {
        UpdateCastleHealthUI();
        UpdateWaveUi();
        UpdateMoneyUI();
    }

    // Método para atualizar o Slider da vida do castelo
    private void UpdateCastleHealthUI()
    {
        castleHealthSlider.value = (float)gameManager.GetCastleHealth() / gameManager.GetMaxCastleHealth();
    }

    // Método para atualizar o texto de dinheiro do jogador usando TextMeshProUGUI
    private void UpdateMoneyUI()
    {
        moneyText.text = gameManager.GetPlayerMoney().ToString() + "$";
    }

    private void UpdateWaveUi()
    {
        waveText.text = "wave " + waveManager.currentWave.ToString() + "/" + waveManager.maxWaves.ToString();
    }
    // Método chamado quando o botão "QuitButton" é pressionado
    public void OnQuitButtonPressed()
    {
        // Fecha o jogo
        Application.Quit();
    }

    // Método chamado quando o botão "MainMenuButton" é pressionado
    public void OnMainMenuButtonPressed()
    {
        // Volta para a cena principal (outra cena)
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    // Método chamado quando o botão "RestartButton" é pressionado
    public void OnRestartButtonPressed()
    {
        // Reinicia o jogo (recarrega a cena atual)
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    // Método para ativar ou desativar o menu e o cursor
    private void ToggleMenuAndCursor()
    {
        // Ativa ou desativa o GameObject do menu
        menuObject.SetActive(!menuObject.activeSelf);

        // Ativa ou desativa o cursor dependendo do estado do GameObject do menu
        bool newState = menuObject.activeSelf;
        SetMouseCursorState(newState);
    }

    // Método para definir o estado do cursor
    private void SetMouseCursorState(bool newState)
    {
        Cursor.visible = newState;
        Cursor.lockState = newState ? CursorLockMode.Confined : CursorLockMode.Locked;
    }


    // Método para fechar o painel de menu e ocultar o cursor
    public void CloseMenu()
    {
        // Fecha o painel de menu
        menuObject.SetActive(false);

        // Oculta o cursor
        SetMouseCursorState(false);
        Debug.Log("A");
    }
}
