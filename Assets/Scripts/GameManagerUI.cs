using TMPro;
using UnityEngine;
using UnityEngine.UI;
    public class GameManagerUI : MonoBehaviour
    {
        public Slider castleHealthSlider;
        public TextMeshProUGUI moneyText;

        private GameManager gameManager;

        void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
            UpdateUI();
        }

        void Update()
        {
            // Verifica constantemente a vida do castelo para atualizar o Slider
            UpdateUI();
        }

        // Método para atualizar a UI do Slider da vida do castelo e do dinheiro do jogador
        private void UpdateUI()
        {
            UpdateCastleHealthUI();
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
    }
    
