using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;

//UnitLoader script loads all the data and functions each Unit needs
//Each unit regardless of affiliation requires this script
//I think everything else in here is fairly self explanatory
public class UnitLoader : MonoBehaviour
{
    public List<Node> currentPath = new List<Node>();

    //VARIABLES
    [Header("Varibles")]
    public int currentHealth = 0;
    public int level = 1;
    public int exp = 0;
    public bool hasMoved = false;
    public bool hasAttacked = false;
    public bool rested = false;
    public bool attackable = false;
    public Vector2 originalPosition = new Vector2(0, 0);

    [Header("Stats")]
    public int unitHP;
    public int unitStr;
    public int unitMag;
    public int unitDef;
    public int unitRes;
    public int unitPrf;
    public int unitAgi;

    //REFERENCES
    [Header("References")]
    private CursorController cursor;    
    public Unit unit;
    public BehaviorTag behaviorTag;
    public BattleDialogue battleDialogue;
    public UnitLoader target;
    public Inventory inventory = null;
    public GameObject actionMenu = null;
    public Weapon equippedWeapon = null;
    public List<UnitLoader> enemiesInRange = new List<UnitLoader>();
    public Transform actionMenuSpawn = null;
    public MapDialogue attackedDialogue = null;
    public MapDialogue defeatedDialogue = null;
    public bool below50Quote = false;

    [HideInInspector] public TileMap map;
    [HideInInspector] public SpriteRenderer spriteRenderer = null;
    [HideInInspector] public Animator animator = null;

    private void Start()
    {
        map = FindObjectOfType<TileMap>();
        cursor = FindObjectOfType<CursorController>();
        currentHealth = unit.statistics.health;
        currentPath = null;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = unit.sprite;
        EquipWeapon();
    }

    public BattleStatistics CombatStatistics()
    {
        //Instead when we need the battle stats we can call this function to run the calculation.
        //It will return a battle stats struct containing the calculated stats for us to use
        BattleStatistics stats = new BattleStatistics();

        //Just use the same calculations here but make them apart of the struct
        stats.attack = unit.statistics.strength + equippedWeapon.might;
        stats.attackSpeed = unit.statistics.agility - ((unit.statistics.strength - equippedWeapon.weight) / 4);
        stats.protection = unit.statistics.defense; //plus any shield
        stats.resilience = unit.statistics.resistance; //plus any shield
        stats.hit = equippedWeapon.hit + (unit.statistics.proficiency / 2) + (unit.statistics.motivation / 4);
        stats.avoid = stats.attackSpeed + unit.statistics.motivation / 5;
        stats.crit = equippedWeapon.crit + (unit.statistics.proficiency / 2) + (unit.statistics.motivation / 5);
        stats.vigilance = (unit.statistics.proficiency / 3) + (unit.statistics.motivation / 5);
        
        return stats;
    }

    private void EquipWeapon()
    {
        if(!equippedWeapon)
        {
            foreach(Item item in inventory.inventory)
            {
                if(item.type == ItemType.Weapon)
                {
                    equippedWeapon = (Weapon)item;
                    return;
                }
            }
        }
    }

    public void Selected()
    {
        animator.SetBool("Selected", true);
        GetWalkableTiles();       
    }

    public void Rest()
    {
        foreach(UnitLoader unit in TurnManager.instance.enemyUnits)
        {
            if(unit.spriteRenderer.color == Color.red)
            {
                unit.spriteRenderer.color = Color.white;
            }
        }        
        hasMoved = true;
        rested = true;
        currentPath = null;
        enemiesInRange.Clear();
        target = null;
        spriteRenderer.color = Color.grey;
        map.DehighlightTiles();
    }
    public void Stand()
    {
        hasMoved = false;
        rested = false;
        spriteRenderer.color = Color.white;
    }
    public void Attack(UnitLoader enemy)
    {
        CombatManager.instance.EngageAttack(this, enemy);
    }
    public void GetWalkableTiles()
    {
        map.DehighlightTiles();

        map.walkableTiles = map.GenerateWalkableRange((int)transform.localPosition.x, (int)transform.localPosition.y, unit.statistics.movement, this);
        map.attackableTiles = map.GenerateAttackableRange((int)transform.localPosition.x, (int)transform.localPosition.y, (unit.statistics.movement + equippedWeapon.range), this);        
        map.CleanAttackableTiles();

        map.HighlightTiles();
    }
    public void GetEnemies()
    {
        Vector2 currentPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
        Vector2 enemyPosition = new Vector2(0, 0);

        foreach(UnitLoader unit in TurnManager.instance.enemyUnits)
        {
            enemyPosition = new Vector2(unit.transform.localPosition.x, unit.transform.localPosition.y);
            if (Vector2.Distance(currentPosition, enemyPosition) <= equippedWeapon.range)
            {
                unit.AttackableHighlight();
                if (!enemiesInRange.Contains(unit))
                {
                    enemiesInRange.Add(unit);
                }
            }
        }
    }
    private void AttackableHighlight()
    {        
        if(spriteRenderer.color == Color.red)
        {
            spriteRenderer.color = Color.white;
        }
        else
        {
            spriteRenderer.color = Color.red;
        }
    }

    private IEnumerator NodeMovement()
    {
        cursor.cursorControls.DeactivateInput(); 
        if(currentPath != null)
        {
            Vector3 finalNode = new Vector3(currentPath[currentPath.Count - 1].x, currentPath[currentPath.Count - 1].y);
            while (transform.localPosition != finalNode)
            {
                // Turn off all other animations so they don't start overlapping
                animator.SetBool("Up", false);
                animator.SetBool("Down", false);
                animator.SetBool("Left", false);
                animator.SetBool("Right", false);

                Vector3 nextNode = new Vector3(currentPath[1].x, currentPath[1].y);

                // Setting the proper animations
                if(nextNode.x > transform.localPosition.x && nextNode.y == transform.localPosition.y)
                    animator.SetBool("Right", true);
                else if(nextNode.x < transform.localPosition.x && nextNode.y == transform.localPosition.y)
                    animator.SetBool("Left", true);
                else if(nextNode.x == transform.localPosition.x && nextNode.y > transform.localPosition.y)
                    animator.SetBool("Up", true);
                else if (nextNode.x == transform.localPosition.x && nextNode.y < transform.localPosition.y)
                    animator.SetBool("Down", true);

                SoundManager.instance.PlayFX(10);
                yield return new WaitForSeconds(0.01f);

                while(transform.localPosition != nextNode)
                {
                    transform.localPosition = Vector2.MoveTowards(transform.localPosition, nextNode, 3f * Time.deltaTime);                    
                    yield return null;
                }

                currentPath.RemoveAt(0);
            }
        }
        cursor.cursorControls.ActivateInput();
        hasMoved = true;
        map.DehighlightTiles();
        ActionMenu();
        GetEnemies();
        map.HighlightTiles();
        cursor.cursorControls.SwitchCurrentActionMap("UI");
        cursor.SetState(new ActionMenuState(cursor));

        animator.SetBool("Up", false);
        animator.SetBool("Down", false);
        animator.SetBool("Selected", false);
        animator.SetBool("Left", false);
        animator.SetBool("Right", false);
        animator.CrossFade("Idle", 0.3f);
    }
    public void FollowPath()
    {
        StartCoroutine(NodeMovement());
    }

    public void GainExperience(int gainedEXP)
    {
        exp += gainedEXP;
    }
    public void LevelUp()
    {
        int hpRoll = Random.Range(0, 99);
        int strRoll = Random.Range(0, 99);
        int magRoll = Random.Range(0, 99);
        int defRoll = Random.Range(0, 99);
        int resRoll = Random.Range(0, 99);
        int prfRoll = Random.Range(0, 99);
        int agiRoll = Random.Range(0, 99);

        if(hpRoll < unit.growthRates.healthGrowth)
        {
            unitHP++;
        }
        if(strRoll < unit.growthRates.strengthGrowth)
        {
            unitStr++;
        }
        if(magRoll < unit.growthRates.magicGrowth)
        {
            unitMag++;
        }
        if(defRoll < unit.growthRates.defenseGrowth)
        {
            unitDef++;
        }
        if(resRoll < unit.growthRates.resistanceGrowth)
        {
            unitRes++;
        }
        if(prfRoll < unit.growthRates.proficiencyGrowth)
        {
            unitPrf++;
        }
        if(agiRoll < unit.growthRates.agilityGrowth)
        {
            unitAgi++;
        }
    }

    public void Death()
    {
        if(currentHealth <= 0 && gameObject != null)
        {
            if(unit.allyUnit)
            {
                TurnManager.instance.allyUnits.Remove(this);
                animator.SetTrigger("Death");
            }
            else
            {
                TurnManager.instance.enemyUnits.Remove(this);
                animator.SetTrigger("Death");
            }
        }
    }

    public void Destruct()
    {
        if(LoseManager.instance.gameObject.activeSelf)
        {
            if(unit.allyUnit)
            {
                if(LoseManager.instance.specificAlly && LoseManager.instance.specificAllies.Contains(this))
                {
                    LoseManager.instance.StartGameOver();
                    MusicPlayer.instance.PauseTrack();
                    Destroy(gameObject);
                }
                else if(LoseManager.instance.anyAlly)
                {
                    LoseManager.instance.StartGameOver();
                    MusicPlayer.instance.PauseTrack();
                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
    public void ActionMenu()
    {
        actionMenu.transform.position = actionMenuSpawn.position;
        if(actionMenu.activeSelf == true)
        {
            actionMenu.SetActive(false);            
        }
        else
        {
            actionMenu.SetActive(true);
        }
    }    

    private void OnDestroy()
    {
        if(GameObject.Find("LoseManager")){
            if(GetComponent<BattleDialogue>()){
                if(GetComponent<BattleDialogue>().deathQuote){
                    if(GameObject.Find("MapDialogueManager")){
                        MapDialogueManager.instance.WriteSingle(GetComponent<BattleDialogue>().deathQuote);
                    }
                }
            }
        }
    }
}
