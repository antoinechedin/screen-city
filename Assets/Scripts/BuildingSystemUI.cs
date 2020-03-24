using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSystemUI : MonoBehaviour
{
    public BuildingSystem buildingSystem;
    public Text sizingInfosText;


    private void Update()
    {
        if (buildingSystem != null && sizingInfosText != null)
        {
            string x = "<color=" + (buildingSystem.sizingX ? "red" : "grey") + ">X</color>";
            string y = "<color=" + (buildingSystem.sizingY ? "green" : "grey") + ">Y</color>";
            string z = "<color=" + (buildingSystem.sizingZ ? "blue" : "grey") + ">Z</color>";

            sizingInfosText.text = x + " " + y + " " + z;
        }
    }
}
