﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using InventorySystem;

public class WeaponSelection : MonoBehaviour
{
    [SerializeField] private CombatPreview combatPreview = null;
    [SerializeField] private InventorySlot[] slots = null;
    private CursorController cursor = null;
    public List<Item> unitWeapons = new List<Item>();

    private void Awake()
    {
        cursor = FindObjectOfType<CursorController>();
    }
    private void OnEnable()
    {
        Invoke("GetUnitWeapons", 0.26f);
        Invoke("FillSlots", 0.27f);
        StartCoroutine(HighlightButton());
    }
    private void OnDisable()
    {
        unitWeapons.Clear();
        foreach(InventorySlot slot in slots){
            slot.ClearSlot();
            slot.gameObject.SetActive(false);
        }
    }

    private void GetUnitWeapons()
    {
        for(int i = 0; i < cursor.selectedUnit.inventory.inventory.Count; i++){
            if(cursor.selectedUnit.inventory.inventory[i].type == ItemType.Weapon){
                unitWeapons.Add(cursor.selectedUnit.inventory.inventory[i]);
            }
        }
    }
    private void FillSlots()
    {
        for(int i = 0; i < unitWeapons.Count; i++){
            slots[i].gameObject.SetActive(true);
        }
        for(int i = 0; i < unitWeapons.Count; i++){
            slots[i].item = unitWeapons[i];
            slots[i].FillSlot();
        }
        foreach(InventorySlot slot in slots){
            if(cursor.selectedUnit.equippedWeapon == slot.item){
                slot.equippedIcon.SetActive(true);
            }
        }
    }
    private void ResetEquippedWeapon()
    {
        foreach (InventorySlot slot in slots)
        {
            if(slot.equippedIcon)
            {
                slot.equippedIcon.SetActive(false);
            }
        }
        foreach (InventorySlot slot in slots)
        {
            if (cursor.selectedUnit.equippedWeapon == slot.item)
            {
                slot.equippedIcon.SetActive(true);
                return;
            }
        }
    }
    public void EquipWeapon(InventorySlot item)
    {
        if(cursor.selectedUnit.equippedWeapon == (Weapon)item.item){
            cursor.AttackTarget();
        }
        else{
            cursor.selectedUnit.equippedWeapon = null;
            cursor.selectedUnit.equippedWeapon = (Weapon)item.item;
            ResetEquippedWeapon();
            combatPreview.gameObject.SetActive(true);
            combatPreview.ClearPreview();
            combatPreview.FillPreview();
        }
    }    
    private IEnumerator HighlightButton()
    {
        yield return new WaitForSeconds(0.28f);
        EventSystem.current.SetSelectedGameObject(null);        
        yield return new WaitForSeconds(0.05f);
        EventSystem.current.SetSelectedGameObject(slots[0].gameObject);
    }
}
