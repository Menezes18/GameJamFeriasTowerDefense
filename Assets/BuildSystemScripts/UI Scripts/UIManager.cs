using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public GameObject BuildPanel;
    private bool isPanelOpen = false;

    private void Start()
    {
        BuildPanel.SetActive(false);
        SetMouseCursorState(false); // Definir o cursor como invisível e confinado no início.
    }

    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (isPanelOpen)
            {
                ClosePainel();
            }
            else
            {
                OpenPanel();
            }
        }
    }

    public void OpenPanel()
    {
        BuildPanel.SetActive(true);
        SetMouseCursorState(true); // Ativar o cursor quando o painel estiver visível.
        isPanelOpen = true;
    }

    public void ClosePainel()
    {
        BuildPanel.SetActive(false);
        SetMouseCursorState(false); // Desativar o cursor ao fechar o painel.
        isPanelOpen = false;
    }

    public void SetMouseCursorState(bool newState)
    {
        Cursor.visible = newState;
        Cursor.lockState = newState ? CursorLockMode.Confined : CursorLockMode.Locked;
    }
}