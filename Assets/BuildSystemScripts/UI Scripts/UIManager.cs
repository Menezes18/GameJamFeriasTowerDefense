using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public BuildingPanelUI BuildPanel;

    private void Start()
    {
        BuildPanel.gameObject.SetActive(false);
        SetMouseCursorState(BuildPanel.gameObject.activeInHierarchy);
    }

    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            BuildPanel.gameObject.SetActive(!BuildPanel.gameObject.activeInHierarchy);
            if (BuildPanel.gameObject.activeInHierarchy) BuildPanel.PopulateButtons();
            SetMouseCursorState(BuildPanel.gameObject.activeInHierarchy);
        }
    }

    private void SetMouseCursorState(bool newState)
    {
        Cursor.visible = newState;
        Cursor.lockState = newState ? CursorLockMode.Confined : CursorLockMode.Locked;
    }
}
