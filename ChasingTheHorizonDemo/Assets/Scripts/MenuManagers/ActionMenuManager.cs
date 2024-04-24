using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Manages the Action Menu as well as the Inventory and the Combat Preview menu
//I want to seperate the Inventory and Combat Preview menu into their own scripts eventually but for now all of that is handled here
//There should only be 1 ActionMenuManager script per scene
public class ActionMenuManager : MonoBehaviour
{
    public static ActionMenuManager instance;

    [Header("Other References")]
    [SerializeField] private CursorController cursor;
    [SerializeField] private TileMap map;
    [SerializeField] private Button actionMenuButton = null;

    public GameObject inventoryMenu = null;
    public GameObject spellMenu = null;
    public GameObject combatPreview = null;
    public GameObject weaponSelection = null;

    private RectTransform rectTransform = null;

    private void Awake()
    {
        instance = this;
        rectTransform = GetComponent<RectTransform>();
        map = FindObjectOfType<TileMap>();
        cursor = FindObjectOfType<CursorController>();
    }

    private void OnEnable()
    {
        StartCoroutine(HighlightButton());
    }
    private void OnDisable()
    {
        inventoryMenu.SetActive(false);
        spellMenu.SetActive(false);
    }

    public void Attack()
    {
        if(map.selectedUnit.enemiesInRange.Count >= 1)
        {
            cursor.SetState(new ActionMenuState(cursor));
            HideActionMenu();
            EventSystem.current.SetSelectedGameObject(null);
            cursor.cursorControls.SwitchCurrentActionMap("MapScene");
            cursor.SetState(new AttackState(cursor));
            map.selectedUnit.HighlightEnemies();
            map.DehighlightTiles();
            map.attackableTiles = map.GenerateRange((int)map.selectedUnit.transform.localPosition.x, (int)map.selectedUnit.transform.localPosition.y, map.selectedUnit.equippedWeapon.range, map.selectedUnit, false);            
            map.HighlightTiles();
        }
    }
    public void Spells()
    {
        if (map.selectedUnit.activeSpells.Count == 0) return;

        if(spellMenu.activeSelf == false)
        {
            spellMenu.SetActive(true);
        }
        else
        {
            spellMenu.SetActive(false);
        }
    }
    public void Items()
    {
        if(inventoryMenu.activeSelf == false)
        {
            inventoryMenu.SetActive(true);
        }
        else
        {
            CloseInventory();
        }
    }
    public void Wait()
    {
        map.selectedUnit.Rest();
        map.selectedUnit = null;
        map.DehighlightTiles();
        gameObject.SetActive(false);
        cursor.cursorControls.SwitchCurrentActionMap("MapScene");
        cursor.SetState(new MapState(cursor));
    }

    public void HideActionMenu()
    {
        rectTransform.anchoredPosition = new Vector2(-1400, 40);
    }
    public void CloseInventory()
    {
        inventoryMenu.SetActive(false);
        spellMenu.SetActive(false);
        StartCoroutine(HighlightButton());
    }
    public void Highlight()
    {
        StartCoroutine(HighlightButton());
    }
    private IEnumerator HighlightButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return null;
        EventSystem.current.SetSelectedGameObject(actionMenuButton.gameObject);
    }
}
