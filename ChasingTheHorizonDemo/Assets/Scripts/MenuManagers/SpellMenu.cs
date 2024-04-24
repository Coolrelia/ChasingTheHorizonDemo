using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpellMenu : MonoBehaviour
{
    [SerializeField] private TileMap map = null;
    [SerializeField] private CursorController cursor = null;

    [SerializeField] private List<SpellMenuSlot> spellSlots = new List<SpellMenuSlot>();

    [Header("Spell Preview")]
    [SerializeField] private Text spellName = null;
    [SerializeField] private Image spellIcon = null;
    [SerializeField] private Text spellDescription = null;
    [SerializeField] private Text spellCost = null;
    [SerializeField] private Text spellType = null;
    [SerializeField] private Text spellElement = null;
    [SerializeField] private Text spellRange = null;

    [Header("Spell Weapon Preview")]
    [SerializeField] private Text spellMight = null;
    [SerializeField] private Text spellHit = null;
    [SerializeField] private Text spellCrit = null;

    private bool previewReady = false;

    private void Start()
    {
        SetSlots();
    }
    private void Update()
    {
        if (previewReady == true)
        {
            SpellPreview();
        }
    }

    private void OnEnable()
    {
        Invoke("FillSlots", 0.1f);
        Invoke("HighlightButton", 0.2f);
        StartCoroutine(StartSpellPreview());
        SoundManager.instance.PlayFX(11);
    }
    private void OnDisable()
    {
        ClearSlots();
        ClearSpellPreview();
        previewReady = false;
    }

    public void UseSpell(SpellMenuSlot spell)
    {
        if (spell == null) return;

        map.selectedUnit.equippedSpell = spell.spell;
        if (spell.spell.spellType == Spell.SpellType.Damage){
            DamageSpell dSpell = (DamageSpell)spell.spell;
            map.selectedUnit.equippedWeapon = dSpell.spellAsWeapon;
        }

        // close all menus on screen 
        cursor.SetState(new ActionMenuState(cursor));
        ActionMenuManager.instance.HideActionMenu();
        EventSystem.current.SetSelectedGameObject(null);
        cursor.cursorControls.SwitchCurrentActionMap("MapScene");
        cursor.SetState(new SpellState(cursor));

        // display the range of the spell selected
        map.selectedUnit.GetEnemies();
        map.selectedUnit.HighlightEnemies();
        map.DehighlightTiles();
        map.attackableTiles = map.GenerateRange((int)map.selectedUnit.transform.localPosition.x, (int)map.selectedUnit.transform.localPosition.y, spell.spell.range, map.selectedUnit, false);
        map.HighlightTiles();

        // selecting a unit in range should perform the spell action
    }

    private void SpellPreview()
    {
        if (EventSystem.current.currentSelectedGameObject == null) return;

        if (EventSystem.current.currentSelectedGameObject.GetComponent<SpellMenuSlot>() && EventSystem.current.currentSelectedGameObject.GetComponent<SpellMenuSlot>().spell != null)
        {
            SpellMenuSlot highlightedSlot = EventSystem.current.currentSelectedGameObject.GetComponent<SpellMenuSlot>();

            if (highlightedSlot.spell.spellType == Spell.SpellType.Damage)
            {
                DamageSpell ds = (DamageSpell)highlightedSlot.spell;
                Weapon dSpell = ds.spellAsWeapon;

                spellName.text = ds.spellName;
                spellIcon.sprite = ds.spellIcon;
                spellIcon.color = new Color32(255, 255, 255, 255);
                spellDescription.text = ds.spellDescription;
                spellCost.text = "Cost: " + ds.hpCost.ToString();
                spellElement.text = "Element: " + ds.element.elementName.ToString();
                spellType.text = "Type: " + ds.spellType.ToString();
                spellRange.text = "Range: " + ds.range.ToString();

                spellMight.text = "Might: " + dSpell.might.ToString();
                spellHit.text = "Hit: " + dSpell.hit.ToString();
                spellCrit.text = "Crit: " + dSpell.crit.ToString();
            }
            else
            {
                ClearSpellPreview();
                spellName.text = highlightedSlot.spell.spellName;
                spellIcon.sprite = highlightedSlot.spell.spellIcon;
                spellIcon.color = new Color32(255, 255, 255, 255);
                spellDescription.text = highlightedSlot.spell.spellDescription;
                spellCost.text = "Cost: " + highlightedSlot.spell.hpCost.ToString();
                spellElement.text = "Element: " + highlightedSlot.spell.element.elementName.ToString();
                spellType.text = "Type: " + highlightedSlot.spell.spellType.ToString();
                spellRange.text = "Range: " + highlightedSlot.spell.range.ToString();
            }
        }
        else
        {
            ClearSpellPreview();
        }
    }
    private void ClearSpellPreview()
    {
        spellName.text = null;
        spellIcon.sprite = null;
        spellIcon.color = new Color32(255, 255, 255, 0);
        spellDescription.text = null;
        spellMight.text = null;
        spellHit.text = null;
        spellCrit.text = null;
        spellRange.text = null;
    }
    private void SetSlots()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).GetComponent<SpellMenuSlot>())
            {
                spellSlots.Add(gameObject.transform.GetChild(i).GetComponent<SpellMenuSlot>());
            }
        }
    }
    private void FillSlots()
    {
        for (int i = 0; i < map.selectedUnit.activeSpells.Count; i++)
        {
            spellSlots[i].spell = map.selectedUnit.activeSpells[i];
            spellSlots[i].FillSlot();
        }
    }
    private void ClearSlots()
    {
        for (int i = 0; i < spellSlots.Count; i++)
        {
            spellSlots[i].ClearSlot();
        }
    }
    private void HighlightButton()
    {
        EventSystem.current.SetSelectedGameObject(spellSlots[0].gameObject);
    }
    private IEnumerator StartSpellPreview()
    {
        yield return new WaitForSeconds(0.26f);
        previewReady = true;
    }
}
