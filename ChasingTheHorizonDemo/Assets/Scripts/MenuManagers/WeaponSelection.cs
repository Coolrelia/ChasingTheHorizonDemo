using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using InventorySystem;

public class WeaponSelection : MonoBehaviour
{
    [SerializeField] private CombatPreview combatPreview = null;
    [SerializeField] private InventorySlot[] slots = null;
    [SerializeField] private CursorController cursor = null;
    [SerializeField] private TileMap map = null;
    public List<Item> unitWeapons = new List<Item>();

    private void OnEnable()
    {
        Invoke("GetUnitWeapons", 0.17f);
        Invoke("FillSlots", 0.18f);
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
        for(int i = 0; i < map.selectedUnit.inventory.Count; i++){
            if(map.selectedUnit.inventory[i].type == ItemType.Weapon){
                unitWeapons.Add(map.selectedUnit.inventory[i]);
            }
        }
    }
    private void FillSlots()
    {
        for (int i = 0; i < unitWeapons.Count; i++){
            slots[i].gameObject.SetActive(true);
        }
        for(int i = 0; i < unitWeapons.Count; i++){
            slots[i].item = unitWeapons[i];
            slots[i].FillSlot();
        }
        foreach(InventorySlot slot in slots){
            if(map.selectedUnit.equippedWeapon == slot.item){
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
            if (map.selectedUnit.equippedWeapon == slot.item)
            {
                slot.equippedIcon.SetActive(true);
                return;
            }
        }
    }
    public void EquipWeapon(InventorySlot item)
    {
        if(map.selectedUnit.equippedWeapon == (Weapon)item.item){
            cursor.AttackTarget();
        }
        else{
            SoundManager.instance.PlayFX(8);
            map.selectedUnit.equippedWeapon = null;
            map.selectedUnit.equippedWeapon = (Weapon)item.item;
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
