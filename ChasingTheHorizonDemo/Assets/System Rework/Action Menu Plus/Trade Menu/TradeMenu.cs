using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeMenu : MonoBehaviour
{
    public static TradeMenu instance;
    // when this script is enabled two menus should appear
    // each menu will display the inventories of the trading units
    // selecting an item in an inventory will send it to another if that inventory has room
    private TileMap map;
    private GameObject inventory1;
    private GameObject inventory2;
    private GameObject menuObject;
    private UnitLoader secondUnit;
    private TradeInventorySlot selectedSlot;
    private TradeInventorySlot previousSlot;

    public GameObject slotPrefab;

    private void Awake()
    {
        instance = this;
        menuObject = transform.GetChild(0).gameObject;
        inventory1 = transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        inventory2 = transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
    }

    private void Start()
    {
        map = FindObjectOfType<TileMap>();
    }

    public void BeginTrade(UnitLoader unit)
    {
        menuObject.SetActive(true);
        secondUnit = unit;
        // instantiante as many inventory slots as there are items in each units menu.
        for (int i = 0; i < map.selectedUnit.inventory.Count; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventory1.transform);
            slot.GetComponent<TradeInventorySlot>().SetSlot(map.selectedUnit.inventory[i]);
            slot.GetComponent<TradeInventorySlot>().inventory1 = true;
        }
        for (int i = 0; i < secondUnit.inventory.Count; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventory2.transform);
            slot.GetComponent<TradeInventorySlot>().SetSlot(secondUnit.inventory[i]);
            slot.GetComponent<TradeInventorySlot>().inventory1 = false;
        }
    }

    public void SelectSlot(TradeInventorySlot slot)
    {
        if(selectedSlot == null)
        {
            selectedSlot = slot;
        }
        else
        {
            previousSlot = selectedSlot;
            selectedSlot = slot;
            TradeItems();
        }
    }

    public void TradeItems()
    {        
        if(selectedSlot == previousSlot)
        {
            if (previousSlot.inventory1)
            {
                map.selectedUnit.inventory.Remove(previousSlot.item);
                secondUnit.inventory.Add(previousSlot.item);

            }
            else if (!previousSlot.inventory1)
            {
                secondUnit.inventory.Remove(previousSlot.item);
                map.selectedUnit.inventory.Add(previousSlot.item);
            }
            ReloadInventory();
            return;
        }

        if (selectedSlot && !previousSlot) return;
        if (!selectedSlot && previousSlot) return;
        if (previousSlot.inventory1)
        {
            map.selectedUnit.inventory.Remove(previousSlot.item);
            secondUnit.inventory.Add(previousSlot.item);
            map.selectedUnit.inventory.Add(selectedSlot.item);
            secondUnit.inventory.Remove(selectedSlot.item);
        }
        else if (!previousSlot.inventory1)
        {
            secondUnit.inventory.Remove(previousSlot.item);
            map.selectedUnit.inventory.Add(previousSlot.item);
            secondUnit.inventory.Add(selectedSlot.item);
            map.selectedUnit.inventory.Remove(selectedSlot.item);
        }
        ReloadInventory();
    }

    public void CloseTrade()
    {
        if (map.selectedUnit.inventory.Count <= 0) return;
        if (secondUnit.inventory.Count <= 0) return;

        for (int i = 0; i < inventory1.transform.childCount; i++)
        {
            Destroy(inventory1.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < inventory2.transform.childCount; i++)
        {
            Destroy(inventory2.transform.GetChild(i).gameObject);
        }
        ActionMenuPlus.instance.cursor.cursorControls.SwitchCurrentActionMap("UI");
        ActionMenuPlus.instance.cursor.SetState(new ActionMenuState(ActionMenuPlus.instance.cursor));
        menuObject.SetActive(false);
        secondUnit = null;
    }

    private void ReloadInventory()
    {
        selectedSlot = null;
        previousSlot = null;
        for (int i = 0; i < inventory1.transform.childCount; i++)
        {
            Destroy(inventory1.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < inventory2.transform.childCount; i++)
        {
            Destroy(inventory2.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < map.selectedUnit.inventory.Count; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventory1.transform);
            slot.GetComponent<TradeInventorySlot>().SetSlot(map.selectedUnit.inventory[i]);
            slot.GetComponent<TradeInventorySlot>().inventory1 = true;
        }
        for (int i = 0; i < secondUnit.inventory.Count; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventory2.transform);
            slot.GetComponent<TradeInventorySlot>().SetSlot(secondUnit.inventory[i]);
            slot.GetComponent<TradeInventorySlot>().inventory1 = false;
        }
    }
}
