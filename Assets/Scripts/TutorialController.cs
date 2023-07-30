using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; // Ou use "using UnityEngine.UI;" se estiver usando o pacote Text regular.

public class TutorialController : MonoBehaviour
{
    public TMP_Text infoText; // Referência ao objeto de texto na cena.

    bool shopOpen = false;
    bool towerSelected = false;
    private bool tower1Selected = false;
    private bool passo1 = false;
    private bool passo2 = true;
    private bool click = false;
    
    // Variáveis para armazenar se cada tecla foi pressionada
    public bool wPressed = false;
    public bool aPressed = false;
    public bool sPressed = false;
    public bool dPressed = false;
    
    void Update()
    {   
        if (shopOpen && towerSelected)
        {
            
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                ShowInfoText("Muito bem! Coloque ela no chão, use o R para rotacionar");
                
                if (Mouse.current.leftButton.wasPressedThisFrame && click)
                {
                    passo1 = true;
                }

                click = true;


            }
        }
        if (shopOpen && !towerSelected)
        {
            if (!tower1Selected)
            {
                ShowInfoText("Muito bem! Click na primeira torre! Ganhe mais dinheiro eliminando os inimigos");
                //ShowInfoText("Muito bem! Coloque ela no chão, use o R para rotacionar");
                towerSelected = true;
                
            }
        }
        if (passo2)
        {
            ShowInfoText("Aperte W, A, S, D para movimentar-se");
            
            // Verifica se as teclas foram pressionadas nesta frame e atualiza as variáveis bool
            if (Keyboard.current.wKey.wasPressedThisFrame) wPressed = true;
            if (Keyboard.current.aKey.wasPressedThisFrame) aPressed = true;
            if (Keyboard.current.sKey.wasPressedThisFrame) sPressed = true;
            if (Keyboard.current.dKey.wasPressedThisFrame) dPressed = true;

            // Se alguma das teclas foi pressionada, avança para o próximo passo
            if (wPressed && aPressed && sPressed && dPressed)
            {
                passo2 = false;
                ShowInfoText("Muito bem! Pressione Tab para abrir a loja de Towers use para proteger o seu castelo! ");
            }
        }

        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (!shopOpen && !towerSelected)
            {
                shopOpen = true;
                ShowInfoText("Muito bem! Click na primeira torre!");
            }
        }

        // Cuidado com o dinheiro

        if (passo1)
        {
            ShowInfoText("Muito Bem! Aperte T para iniciar a wave de inimigos!");
            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                ShowInfoText(" ");
            }
        }
    }

    void ShowInfoText(string message)
    {
        infoText.text = message;
    }

    public void Concluido()
    {
        ShowInfoText("Você completou o tutorial! PARABENS!");
    }
}