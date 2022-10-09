using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitItem : MonoBehaviour
{
    public UnitLoader unit;
    public Text unitName;
    public Image unitIcon;

    public void LoadUnit(UnitLoader loadedUnit)
    {
        unit = loadedUnit;
        unitName.text = unit.name;
        unitIcon.sprite = unit.unit.sprite;
    }

    public void Interact()
    {
        string menu = DeploymentScreen.instance.activeMenu;

        switch (menu)
        {
            case "Convoy":
                DeploymentArmy.instance.SelectUnit(unit);
                break;

            case "Deployment":
                DeploymentArmy.instance.DeployUnit(unit);
                break;
        }
    }
}
