using UnityEngine;
using UnityEngine.UI;

//Manages the all the UI on screen
//Currently manages the Tile Info UI, I may move that to it's own script we'll see
public class UIManager : MonoBehaviour
{
    private TileMap map;

    [Header("Ally Unit UI")]
    public Text unitName;
    public Text unitHP;
    public Text unitLevel;
    public Text unitEXP;
    public Text unitAgility;

    public Text unitStrength;
    public Text unitMagic;
    public Text unitDefense;
    public Text unitResistance;

    public Text unitProficiency;
    public Text unitMovement;
    public Text unitMotivation;

    public Image unitWeapon;

    [Header("Enemy UI Info")]
    public Text enemyName;
    public Text enemyHP;
    public Text enemyLevel;
    public Text enemyAgility;

    public Text enemyStrength;
    public Text enemyMagic;
    public Text enemyDefense;
    public Text enemyResistance;

    public Image enemyWeapon;

    [Header("Tile UI Info")]
    public Text tileName;
    public Text tileCost;

    private void Start()
    {
        map = FindObjectOfType<TileMap>();
    }

    private void Update()
    {
        //AllyUnitUI();
        //EnemyUnitUI();
        TileUI();
    }


    private void AllyUnitUI()
    {
        foreach(UnitLoader unit in TurnManager.instance.allyUnits)
        {
            if(transform.position == unit.transform.position && unit.unit.allyUnit && unit.currentHealth > 0)
            {
                unitName.text = unit.unit.unitName;
                unitHP.text = "HP: " + unit.currentHealth.ToString() + "/" + unit.unitHP.ToString();
                unitEXP.text = "Exp: " + unit.exp.ToString();
                unitLevel.text = "Lvl: " + unit.level.ToString();
                unitAgility.text = "Agl: " + unit.unitAgi.ToString();

                unitStrength.text = "Str: " + unit.unitStr.ToString();
                unitMagic.text = "Mag: " + unit.unitMag.ToString();
                unitDefense.text = "Def: " + unit.unitDef.ToString();
                unitResistance.text = "Res: " + unit.unitRes.ToString();

                unitProficiency.text = "Pro: " + unit.unitPrf.ToString();
                unitMovement.text = "Mov: " + unit.unit.statistics.movement.ToString();
                unitMotivation.text = "Mot: " + unit.unit.statistics.motivation.ToString();

                unitWeapon.color = new Color32(255, 255, 255, 255);
                unitWeapon.sprite = unit.equippedWeapon.itemIcon;
            }
            else if(unit.unit.allyUnit && unit.currentHealth <= 0)
            {
                unitName.text = "";
                unitHP.text = "";
                unitEXP.text = "";
                unitLevel.text = "";
                unitAgility.text = "";

                unitStrength.text = "";
                unitMagic.text = "";
                unitDefense.text = "";
                unitResistance.text = "";

                unitProficiency.text = "";
                unitMovement.text = "";
                unitMotivation.text = "";

                unitWeapon.color = new Color32(255, 255, 255, 0);
                unitWeapon.sprite = null;
            }
        }
    }
    private void EnemyUnitUI()
    {
        foreach (UnitLoader unit in TurnManager.instance.enemyUnits)
        {
            if(transform.position == unit.transform.position && !unit.unit.allyUnit && unit.currentHealth > 0)
            {
                enemyName.text = unit.unit.unitName;
                enemyHP.text = "HP: " + unit.currentHealth.ToString() + "/" + unit.unitHP.ToString();
                enemyLevel.text = "Lvl: " + unit.level.ToString();
                enemyAgility.text = "Agl: " + unit.unitAgi.ToString();

                enemyStrength.text = "Str: " + unit.unitStr.ToString();
                enemyMagic.text = "Mag: " + unit.unitMag.ToString();
                enemyDefense.text = "Def: " + unit.unitDef.ToString();
                enemyResistance.text = "Res: " + unit.unitRes.ToString();

                enemyWeapon.color = new Color32(255, 255, 255, 255);
                enemyWeapon.sprite = unit.equippedWeapon.itemIcon;
            }
            else if(!unit.unit.allyUnit && unit.currentHealth <= 0)
            {
                enemyName.text = "";
                enemyHP.text = "";
                enemyLevel.text = "";
                enemyAgility.text = "";

                enemyStrength.text = "";
                enemyMagic.text = "";
                enemyDefense.text = "";
                enemyResistance.text = "";

                enemyWeapon.color = new Color32(255, 255, 255, 0);
                enemyWeapon.sprite = unit.equippedWeapon.itemIcon;
            }
        }
    }
    private void TileUI()
    {
        TileType currentTile = map.ReturnTileAt((int)(transform.localPosition.x), (int)(transform.localPosition.y));

        if(currentTile != null)
        {
            tileName.text = currentTile.name;
            tileCost.text = currentTile.movementCost.ToString();
        }
    }  
}
