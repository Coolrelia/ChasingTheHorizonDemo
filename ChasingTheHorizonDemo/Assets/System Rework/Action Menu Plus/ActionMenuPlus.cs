using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMenuPlus : MonoBehaviour
{
    public static ActionMenuPlus instance;

    private TileMap map;
    private GameObject actionMenu;
    [HideInInspector] public CursorController cursor;
    public GameObject inventory;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        map = FindObjectOfType<TileMap>();
        cursor = FindObjectOfType<CursorController>();
        actionMenu = transform.GetChild(0).gameObject;
    }

    public void Toggle() // like a switch!
    {
        // enables the actions menu, shifts the cursor from map control to ui control
        if(actionMenu.activeSelf)
        {
            actionMenu.SetActive(false);
        }
        else
        {
            actionMenu.SetActive(true);
        }
    }

    public void Unlock()
    {
        // check if there is a chest adjacent to the selected unit
        // if there's a chest, open the chest and add the item to the selected units inventory, or convoy if full

        List<TileType> adjacentTiles = map.selectedUnit.ReturnAdjacentTiles();
        TileType chestTile = null;

        foreach(TileType tile in adjacentTiles)
        {
            if (tile.name != "Chest") return;
            chestTile = tile;
        }
        cursor.cursorControls.SwitchCurrentActionMap("MapScene");
        cursor.SetState(new UnlockState(cursor));
    }

    public void Trade()
    {
        // this function only works if the selected unit is adjacent to another allied unit
        print("trade");
        List<UnitLoader> adjacentUnits = map.selectedUnit.ReturnAdjacentUnits();
        if (adjacentUnits.Count <= 0) return;
        print("setting cursor state");
        cursor.cursorControls.SwitchCurrentActionMap("MapScene");
        cursor.SetState(new TradeState(cursor));
        // the cursor is put into trade mode and the player selects which unit to trade with, pressing cancel returns to the action menu
        // displays both units inventories.
        // selecting an item from one inventory will move it to the other inventory unless that inventory is full.
        // exiting this menu will rest the unit who engaged the action, a unit can only trade once per turn.
    }

    public void Convoy()
    {
        List<UnitLoader> adjacentUnits = map.selectedUnit.ReturnAdjacentUnits();
        if (adjacentUnits.Count <= 0) return;
        cursor.cursorControls.SwitchCurrentActionMap("MapScene");
        cursor.SetState(new ConvoyState(cursor));
    }

    public void Inventory()
    {
        // displays the units inventory.
        inventory.SetActive(true);
    }

    public void Wait()
    {
        map.selectedUnit.Rest();
        map.selectedUnit = null;
        map.DehighlightTiles();
        cursor.cursorControls.SwitchCurrentActionMap("MapScene");
        cursor.SetState(new MapState(cursor));
    }

    public void CloseInventory()
    {
        inventory.SetActive(false);
    }
}
