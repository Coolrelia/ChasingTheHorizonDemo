using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Manages the Inventory Menu
//Works in conjunction with the InventorySlot script
//Displays the items of the selected unit in each of the inventory slots
//Also displays item information

namespace InventorySystem
{
    public class InventoryMenu : MonoBehaviour
    {
        [SerializeField] private TileMap map = null;
        [SerializeField] private CursorController cursor = null;

        [SerializeField] private List<InventorySlot> inventorySlots = new List<InventorySlot>();

        [Header("Item Preview")]
        [SerializeField] private Text itemName = null;
        [SerializeField] private Image itemIcon = null;
        [SerializeField] private Text itemDescription = null;

        [Header("Weapon Preview")]
        [SerializeField] private Text weaponMight = null;
        [SerializeField] private Text weaponHit = null;
        [SerializeField] private Text weaponCrit = null;
        [SerializeField] private Text weaponRange = null;
        [SerializeField] private Text weaponWeight = null;

        [SerializeField] private GameObject healingEffect = null;

        private bool previewReady = false;

        private void Start()
        {
            SetSlots();
        }

        private void Update()
        {
            if(previewReady == true)
            {
                ItemPreview();
            }
        }

        private void OnEnable()
        {
            Invoke("FillSlots", 0.1f);
            Invoke("ResetEquippedWeapon", 0.1f);
            Invoke("HighlightButton", 0.2f);
            StartCoroutine(StartItemPreview());
            SoundManager.instance.PlayFX(11);
        }

        private void OnDisable()
        {
            ClearSlots();
            ClearItemPreview();
            previewReady = false;
        }

        public void UseItem(InventorySlot item)
        {
            //check what kind of item you have                        
            if(item.item != null)
            {
                if(item.item.type == ItemType.Weapon)
                {
                    map.selectedUnit.equippedWeapon = null;
                    map.selectedUnit.equippedWeapon = (Weapon)item.item;
                    SoundManager.instance.PlayFX(8);
                    Invoke("ResetEquippedWeapon", 0.05f);
                }
                else if(item.item.type == ItemType.Consumable)
                {
                    Consumable consumable = (Consumable)item.item;
                    if (map.selectedUnit.currentHealth == map.selectedUnit.unit.statistics.health)
                    {
                        return;
                    }
                    else if(map.selectedUnit.currentHealth + consumable.healValue > map.selectedUnit.unit.statistics.health)
                    {
                        map.selectedUnit.currentHealth += map.selectedUnit.unit.statistics.health - map.selectedUnit.currentHealth;
                        SoundManager.instance.PlayFX(9);
                        Vector3 targetPosition = new Vector3(map.selectedUnit.transform.position.x, map.selectedUnit.transform.position.y - 0.3f);
                        Instantiate(healingEffect, targetPosition, healingEffect.transform.rotation);

                        map.selectedUnit.Rest();
                        ActionMenuPlus.instance.CloseInventory();
                        //ActionMenuManager.instance.gameObject.SetActive(false);
                        map.selectedUnit.inventory.Remove(consumable);
                        map.selectedUnit = null;
                        cursor.cursorControls.SwitchCurrentActionMap("MapScene");
                        cursor.SetState(new MapState(cursor));
                    }
                    else
                    {
                        map.selectedUnit.currentHealth += consumable.healValue;
                        SoundManager.instance.PlayFX(9);
                        Vector3 targetPosition = new Vector3(map.selectedUnit.transform.position.x, map.selectedUnit.transform.position.y - 0.3f);
                        Instantiate(healingEffect, targetPosition, healingEffect.transform.rotation);
                        
                        map.selectedUnit.Rest();
                        ActionMenuPlus.instance.CloseInventory();
                        //ActionMenuManager.instance.gameObject.SetActive(false);
                        map.selectedUnit.inventory.Remove(consumable);
                        map.selectedUnit = null;
                        cursor.cursorControls.SwitchCurrentActionMap("MapScene");
                        cursor.SetState(new MapState(cursor));
                    }
                }
            }
        }

        private void ItemPreview()
        {
            if(EventSystem.current.currentSelectedGameObject.GetComponent<InventorySlot>() && EventSystem.current.currentSelectedGameObject.GetComponent<InventorySlot>().item != null)
            {
                InventorySlot highlightedSlot = EventSystem.current.currentSelectedGameObject.GetComponent<InventorySlot>();

                if(highlightedSlot.item.type == ItemType.Weapon)
                {
                    Weapon weapon = (Weapon)highlightedSlot.item;

                    itemName.text = highlightedSlot.item.itemName;
                    itemIcon.sprite = highlightedSlot.item.itemIcon;
                    itemIcon.color = new Color32(255, 255, 255, 255);
                    itemDescription.text = highlightedSlot.item.itemDescription;

                    weaponMight.text = "Might: " + weapon.might.ToString();
                    weaponHit.text = "Hit: " + weapon.hit.ToString();
                    weaponCrit.text = "Crit: " + weapon.crit.ToString();
                    weaponRange.text = "Range: " + weapon.range.ToString();
                    weaponWeight.text = "Weight: " + weapon.weight.ToString();
                }
                else
                {
                    ClearItemPreview();
                    itemName.text = highlightedSlot.item.itemName;
                    itemIcon.sprite = highlightedSlot.item.itemIcon;
                    itemIcon.color = new Color32(255, 255, 255, 255);
                    itemDescription.text = highlightedSlot.item.itemDescription;
                }
            }
            else
            {
                ClearItemPreview();
            }
        }
        private void ClearItemPreview()
        {
            itemName.text = null;
            itemIcon.sprite = null;
            itemIcon.color = new Color32(255, 255, 255, 0);
            itemDescription.text = null;
            weaponMight.text = null;
            weaponHit.text = null;
            weaponCrit.text = null;
            weaponRange.text = null;
            weaponWeight.text = null;
        }
        private void SetSlots()
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                if (gameObject.transform.GetChild(i).GetComponent<InventorySlot>())
                {
                    inventorySlots.Add(gameObject.transform.GetChild(i).GetComponent<InventorySlot>());
                }
            }
        }
        private void FillSlots()
        {
            for (int i = 0; i < map.selectedUnit.inventory.Count; i++)
            {
                inventorySlots[i].item = map.selectedUnit.inventory[i];
                inventorySlots[i].FillSlot();
            }
        }
        private void ResetEquippedWeapon()
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                slot.equippedIcon.SetActive(false);
            }
            foreach (InventorySlot slot in inventorySlots)
            {
                if (map.selectedUnit.equippedWeapon == slot.item)
                {
                    slot.equippedIcon.SetActive(true);
                    return;
                }
            }
        }
        private void ClearSlots()
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                inventorySlots[i].ClearSlot();
            }
        }

        private void HighlightButton()
        {
            EventSystem.current.SetSelectedGameObject(inventorySlots[0].gameObject);
        }

        private IEnumerator StartItemPreview()
        {
            yield return new WaitForSeconds(0.26f);
            previewReady = true;

        }
    }
}

