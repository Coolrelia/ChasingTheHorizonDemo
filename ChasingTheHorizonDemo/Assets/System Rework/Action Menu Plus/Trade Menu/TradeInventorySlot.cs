using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeInventorySlot : MonoBehaviour
{
    public Text itemName = null;
    public Image itemIcon = null;
    public Item item;
    public bool inventory1;
    public bool convoy;
    public int convoyType = 0; //set to one for unit inventory type, 2 for convoy inventory type

    public void Awake()
    {
        itemName = transform.GetChild(0).GetComponent<Text>();
        itemIcon = transform.GetChild(1).GetComponent<Image>();
    }

    public void SetSlot(Item i)
    {
        item = i;
        itemName.text = i.itemName;
        itemIcon.sprite = i.itemIcon;
    }

    public void ReloadSlot(Item i)
    {
        item = null;
        itemName.text = "";
        itemIcon.sprite = null;

        item = i;
        itemName.text = i.itemName;
        itemIcon.sprite = i.itemIcon;
    }

    public void MoveItem() 
    {
        if (!convoy)
        {
            TradeMenu.instance.SelectSlot(this);
        }
        else
        {
            ConvoyTradeMenu.instance.SelectSlot(this);
        }
    }
}
