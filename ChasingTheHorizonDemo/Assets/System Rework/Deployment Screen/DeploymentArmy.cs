using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeploymentArmy : MonoBehaviour
{
    public static DeploymentArmy instance;

    private SaveTest saveData;
    [SerializeField] private Transform armyUnitContent;
    [SerializeField] private Transform deployedUnitContent;
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private GameObject convoyMenu;
    [SerializeField] private GameObject inventoryMenu;

    public UnitLoader selectedUnit;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        saveData = FindObjectOfType<SaveTest>();
        CreateArmy();
        ShowDeployedUnits();
    }

    private void CreateArmy()
    {
        foreach (Transform child in armyUnitContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < saveData.saveObject.army.Count; i++)
        {
            GameObject go = Instantiate(unitPrefab, armyUnitContent);
            go.GetComponent<UnitItem>().LoadUnit(saveData.saveObject.army[i]);
        }
    }

    public void DeployUnit(UnitLoader unit)
    {
        // before we add a unit, make sure we dont exceed the maximum allowed units

        if (saveData.saveObject.deployedArmy.Contains(unit))
            saveData.saveObject.deployedArmy.Remove(unit);
        
        else if (saveData.saveObject.deployedArmy.Count + 1 > MapConditions.instance.maximumDeployableUnits) return;

        else
            saveData.saveObject.deployedArmy.Add(unit);

        ShowDeployedUnits();
    }

    public void ShowDeployedUnits()
    {
        if (deployedUnitContent == null) return;

        foreach (Transform child in deployedUnitContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < saveData.saveObject.deployedArmy.Count; i++)
        {
            GameObject go = Instantiate(unitPrefab, deployedUnitContent);
            go.GetComponent<UnitItem>().LoadUnit(saveData.saveObject.deployedArmy[i]);
        }
    }
    public void SelectUnit(UnitLoader unit)
    {
        if (!selectedUnit)
        {
            convoyMenu.SetActive(true);
            selectedUnit = unit;
            inventoryMenu.SetActive(true);
        }
        else
        {
            convoyMenu.SetActive(true);
            selectedUnit = unit;
            inventoryMenu.SetActive(true);
            DeploymentInventory.instance.CreateInventory();
        }
    }
}
