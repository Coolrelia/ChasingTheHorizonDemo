using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeploymentInventorySlot : MonoBehaviour
{
    public Item item;
    public Text itemName;
    public Image itemIcon;

    public void LoadItem(Item loadedItem)
    {
        item = loadedItem;
        itemName.text = item.itemName;
        itemIcon.sprite = item.itemIcon;
    }

    public void RemoveItem()
    {
        // remove item from the inventory 
        DeploymentArmy.instance.selectedUnit.inventory.Remove(item);
        // add item to the convoy
        DeploymentConvoy.instance.AddItem(item);
        DeploymentInventory.instance.CreateInventory();
    }
}
