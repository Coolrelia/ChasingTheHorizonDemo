using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeploymentInventory : MonoBehaviour
{
    public static DeploymentInventory instance;

    public GameObject inventorySlot;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CreateInventory();
    }

    public void CreateInventory()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < DeploymentArmy.instance.selectedUnit.inventory.Count; i++)
        {
            GameObject go = Instantiate(inventorySlot, transform);
            Item item = DeploymentArmy.instance.selectedUnit.inventory[i];
            go.GetComponent<DeploymentInventorySlot>().LoadItem(item);
        }
    }
}
