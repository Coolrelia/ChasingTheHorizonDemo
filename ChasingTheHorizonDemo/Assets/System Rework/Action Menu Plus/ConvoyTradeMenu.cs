using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvoyTradeMenu : MonoBehaviour
{
    public static ConvoyTradeMenu instance;

    public GameObject slotPrefab;
    private GameObject unitInventory;
    private GameObject convoyInventory;
    private GameObject menuObject;
    private TileMap map;
    private TradeInventorySlot selectedSlot;
    private TradeInventorySlot previousSlot;
    private UnitLoader convoyUnit;
    private bool itemsTraded = false;

    private void Awake()
    {
        instance = this;
        menuObject = transform.GetChild(0).gameObject;
        unitInventory = menuObject.transform.GetChild(0).gameObject;
        convoyInventory = menuObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
    }

    private void Start()
    {
        map = FindObjectOfType<TileMap>();
    }

    public void OpenConvoy(UnitLoader unit)
    {
        convoyUnit = unit;
        menuObject.SetActive(true);

        ActionMenuPlus.instance.cursor.cursorControls.SwitchCurrentActionMap("UI");
        ActionMenuPlus.instance.cursor.SetState(new ConvoyState(ActionMenuPlus.instance.cursor));

        for (int i = 0; i < map.selectedUnit.inventory.Count; i++)
        {
            GameObject go = Instantiate(slotPrefab, unitInventory.transform);
            go.GetComponent<TradeInventorySlot>().SetSlot(map.selectedUnit.inventory[i]);
            go.GetComponent<TradeInventorySlot>().convoy = true;
            go.GetComponent<TradeInventorySlot>().convoyType = 1;
        }
        for (int i = 0; i < convoyUnit.smallConvoy.Count; i++)
        {
            GameObject go = Instantiate(slotPrefab, convoyInventory.transform);
            go.GetComponent<TradeInventorySlot>().SetSlot(convoyUnit.smallConvoy[i]);
            go.GetComponent<TradeInventorySlot>().convoy = true;
            go.GetComponent<TradeInventorySlot>().convoyType = 2;
        }
    }

    public void SelectSlot(TradeInventorySlot slot)
    {
        if (selectedSlot == null)
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

    public void CloseConvoy()
    {
        if (map.selectedUnit.inventory.Count <= 0) return;

        for (int i = 0; i < unitInventory.transform.childCount; i++)
        {
            Destroy(unitInventory.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < convoyInventory.transform.childCount; i++)
        {
            Destroy(convoyInventory.transform.GetChild(i).gameObject);
        }
        if (itemsTraded)
        {
            menuObject.SetActive(false);
            convoyUnit = null;
            ActionMenuPlus.instance.Wait();
        }
        else
        {
            ActionMenuPlus.instance.cursor.cursorControls.SwitchCurrentActionMap("MapScene");
            ActionMenuPlus.instance.cursor.SetState(new ConvoyState(ActionMenuPlus.instance.cursor));
            menuObject.SetActive(false);
            convoyUnit = null;
        }
    }

    private void TradeItems()
    {
        itemsTraded = false;
        if(selectedSlot.convoyType == 1 && previousSlot.convoyType == 2)
        {
            if (map.selectedUnit.inventory.Count >= 5) return;
            convoyUnit.smallConvoy.Remove(previousSlot.item);
            convoyUnit.smallConvoy.Add(selectedSlot.item);
            map.selectedUnit.inventory.Add(previousSlot.item);
            map.selectedUnit.inventory.Remove(selectedSlot.item);
            itemsTraded = true;
            ReloadInventory();
            return;
        }
        else if(selectedSlot.convoyType == 2 && previousSlot.convoyType == 1)
        {
            if (map.selectedUnit.inventory.Count >= 5) return;
            map.selectedUnit.inventory.Add(previousSlot.item);
            map.selectedUnit.inventory.Remove(selectedSlot.item);
            convoyUnit.smallConvoy.Add(selectedSlot.item);
            convoyUnit.smallConvoy.Remove(previousSlot.item);
            itemsTraded = true;
            ReloadInventory();
            return;
        }
        else if(selectedSlot.convoyType == 1 && previousSlot.convoyType == 1)
        {
            map.selectedUnit.inventory.Remove(selectedSlot.item);
            convoyUnit.smallConvoy.Add(selectedSlot.item);
            ReloadInventory();
            itemsTraded = true;
            return;
        }
        else if(selectedSlot.convoyType == 2 && previousSlot.convoyType == 2)
        {
            if (map.selectedUnit.inventory.Count >= 5) return;
            convoyUnit.smallConvoy.Remove(selectedSlot.item);
            map.selectedUnit.inventory.Add(selectedSlot.item);
            itemsTraded = true;
            ReloadInventory();
            return;
        }
    }

    private void ReloadInventory()
    {
        for (int i = 0; i < unitInventory.transform.childCount; i++)
        {
            Destroy(unitInventory.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < convoyInventory.transform.childCount; i++)
        {
            Destroy(convoyInventory.transform.GetChild(i).gameObject);
        }
        
        for (int i = 0; i < map.selectedUnit.inventory.Count; i++)
        {
            GameObject go = Instantiate(slotPrefab, unitInventory.transform);
            go.GetComponent<TradeInventorySlot>().SetSlot(map.selectedUnit.inventory[i]);
            go.GetComponent<TradeInventorySlot>().convoy = true;
            go.GetComponent<TradeInventorySlot>().convoyType = 1;
        }
        for (int i = 0; i < convoyUnit.smallConvoy.Count; i++)
        {
            GameObject go = Instantiate(slotPrefab, convoyInventory.transform);
            go.GetComponent<TradeInventorySlot>().SetSlot(convoyUnit.smallConvoy[i]);
            go.GetComponent<TradeInventorySlot>().convoy = true;
            go.GetComponent<TradeInventorySlot>().convoyType = 2;
        }
    }
}
