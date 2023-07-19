using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public GameObject objetoParaAtivarDesativar;
    private bool cursorVisible = false;


    public BuildingSystem _buildingSystem;
    private void Start()
    {
        // Esconde o cursor ao iniciar o jogo
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _buildingSystem = FindObjectOfType<BuildingSystem>();
    }


    public void OnBotaoClicado()
    {
        // Verifica o estado atual do GameObject e alterna entre ativar e desativar
        if (objetoParaAtivarDesativar != null)
        {
            _buildingSystem.ToggleBuildingSystem(false);
            objetoParaAtivarDesativar.SetActive(!objetoParaAtivarDesativar.activeSelf);
            
        }
    }
    private void Update()
    {
        // Verifica se o botão configurado foi pressionado (neste caso, usaremos a tecla 'T')
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            cursorVisible = !cursorVisible;

            // Alterna a visibilidade do cursor
            Cursor.visible = cursorVisible;

            if (cursorVisible)
            {
                // Cursor está visível, então destrava o cursor para movimento livre
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                // Cursor está oculto, então trava o cursor no centro da tela
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void Tower1()
    {
        _buildingSystem.ToggleBuildingSystem(false);
        _buildingSystem.SelectBlock(0);
    }
    public void Tower2()
    {
        _buildingSystem.ToggleBuildingSystem(false);
        _buildingSystem.SelectBlock(1);
    }
    public void Tower3()
    {
        _buildingSystem.ToggleBuildingSystem(false);
        _buildingSystem.SelectBlock(2);
    }
}

