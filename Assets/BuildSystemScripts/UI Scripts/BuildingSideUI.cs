using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSideUI : MonoBehaviour
{
   public Image BuildingImage;
   public TMP_Text BuildingText;

   private void Start()
   {
      BuildingImage.sprite = null;
      BuildingImage.color = Color.clear;
      BuildingText.text = "";
   }

   public void UpdateSideDisplay(BuildingData data)
   {
      BuildingImage.sprite = data.Icon;
      BuildingImage.color = Color.white;
      BuildingText.text = data.DisplayName;
   }
}
