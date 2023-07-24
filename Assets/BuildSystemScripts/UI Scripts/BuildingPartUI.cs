using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPartUI : MonoBehaviour
{
    private Button _button;
    private BuildingData _assignedData;
    private BuildingPanelUI _parentDisplay;


    public void Init(BuildingData assignedData, BuildingPanelUI parentDisplay)
    {
        _assignedData = assignedData;
        _parentDisplay = parentDisplay;
        _button = GetComponentInChildren<Button>();
        _button.onClick.AddListener(OnButtonClick);
        _button.GetComponent<Image>().sprite = _assignedData.Icon;
    }

    private void OnButtonClick()
    {
        _parentDisplay.OnClick(_assignedData);
    }
}
