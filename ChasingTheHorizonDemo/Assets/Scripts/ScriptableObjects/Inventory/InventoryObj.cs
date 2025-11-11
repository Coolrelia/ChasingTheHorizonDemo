using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Units/Inventory")]
public class InventoryObj : ScriptableObject
{
    public List<Item> inventory = new List<Item>();
    public void AddItem(Item item)
    {
        if (inventory.Count < 5)
        {
            inventory.Add(item);
        }
    }
}
