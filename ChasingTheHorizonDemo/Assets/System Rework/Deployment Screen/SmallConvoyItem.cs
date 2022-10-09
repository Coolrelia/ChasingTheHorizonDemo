using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmallConvoyItem : MonoBehaviour
{
    public Text itemName;
    public Image itemIcon;
    [HideInInspector] public Item item;    

    public void LoadItem(Item loadedItem)
    {
        item = loadedItem;
        itemName.text = item.itemName;
        itemIcon.sprite = item.itemIcon;
    }

    public void Interact()
    {
        DeploymentConvoy.instance.InteractMenu();
        DeploymentConvoy.instance.selectedItem = item;
    }
}
