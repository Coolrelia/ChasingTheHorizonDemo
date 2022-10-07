using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//The AI Manager handles all the enemy unit AI in a Map scene automatically given there are enemy units in the scene
//There should be 1 AI Manager per Map Scene
public class AIManager : MonoBehaviour
{
    public static AIManager instance { get; private set; }

    //REFERENCES
    [SerializeField] private Camera mainCamera = null;
    [SerializeField] private CursorController cursor = null;
    [SerializeField] private TileMap currentMap = null;

    private Animator enemyAnimator = null;
    [SerializeField] private UnitLoader currentEnemy = null;
    [SerializeField] private GameObject combatReadout = null;
    [SerializeField] private FastModeIndicator fastModeIndicator = null;

    public List<UnitLoader> enemyOrder = new List<UnitLoader>();
    [SerializeField] private List<Node> walkableTiles = new List<Node>();
    [SerializeField] private List<UnitLoader> enemiesInRange = new List<UnitLoader>();

    private Vector3 cameraTarget = new Vector3(0, 0, 0);

    private bool fastMode = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Invoke("SetEnemyOrder", 1f);
    }

    public void StartAI()
    {
        fastMode = false;
        StartCoroutine(BehaviorSystem());
    }

    public void FastAI(InputAction.CallbackContext context)
    {
        if(cursor.enemyTurn){
            if(context.started){
                if (!fastMode){
                    fastMode = true;
                    if (fastModeIndicator.gameObject.activeSelf){
                        fastModeIndicator.animator.SetTrigger("Enter");
                    }
                    else{
                        fastModeIndicator.gameObject.SetActive(true);
                    }
                }
                else{
                    fastMode = false;
                    fastModeIndicator.animator.SetTrigger("Exit");
                }
            }
        }
    }

    private IEnumerator BehaviorSystem()
    {
        cursor.enemyTurn = true;
        List<UnitLoader> enemies = enemyOrder;        
        //Iterates through each enemy in the enemy list
        for(int i = 0; i < enemies.Count; i++)
        {
            cursor.cursorControls.DeactivateInput();
            yield return new WaitForEndOfFrame();
            currentEnemy = enemies[i];
            enemyAnimator = currentEnemy.animator;

            if(enemies[i].behaviorTag.blitz)
            {               
                StartCoroutine(MoveCamera(enemies[i]));
                yield return new WaitUntil(() => mainCamera.transform.position == cameraTarget);

                Blitz(currentEnemy);

                yield return new WaitUntil(() => currentEnemy.rested || !currentEnemy);
                yield return new WaitForSeconds(0.5f);
                walkableTiles.Clear();
                enemiesInRange.Clear();
            }
            else if(enemies[i].behaviorTag.boss)
            {
                GetEnemiesInAttackRange();

                if(enemiesInRange.Count == 0) continue;
            
                walkableTiles.Clear();
                enemiesInRange.Clear();

                StartCoroutine(MoveCamera(enemies[i]));
                yield return new WaitUntil(() => mainCamera.transform.position == cameraTarget);

                Boss();

                if(IsLastEnemy() && combatReadout.activeSelf == false){
                    yield return new WaitForSeconds(0.5f);
                }
                else{
                    yield return new WaitUntil(() => currentEnemy.rested || !currentEnemy);
                    yield return new WaitForSeconds(0.5f);
                }
                
                walkableTiles.Clear();
                enemiesInRange.Clear();
            }
            else if(enemies[i].behaviorTag.defensive)
            {
                GetEnemiesInWalkableRange();

                if(enemiesInRange.Count == 0) continue;
                
                walkableTiles.Clear();
                enemiesInRange.Clear();

                StartCoroutine(MoveCamera(enemies[i]));
                yield return new WaitUntil(() => mainCamera.transform.position == cameraTarget);

                Defensive();
                if(IsLastEnemy() && combatReadout.activeSelf == false){
                    yield return new WaitForSeconds(0.5f);
                }
                else{
                    yield return new WaitUntil(() => currentEnemy.rested || !currentEnemy);
                    yield return new WaitForSeconds(0.5f);
                }
            
                walkableTiles.Clear();
                enemiesInRange.Clear();
            }
            yield return new WaitUntil(() => !combatReadout.activeSelf);
            yield return new WaitForSeconds(0.6f);
        }
        enemyOrder.Clear();
        SetEnemyOrder();
        cursor.enemyTurn = false;
        if(fastModeIndicator.gameObject.activeSelf)
            fastModeIndicator.animator.SetTrigger("Exit");
        yield return null;
        TurnManager.instance.SetState(new PlayerTurnState(TurnManager.instance));
    }

    private void Blitz(UnitLoader currentEnemy)
    {
        currentEnemy.currentPath = null;
        GetEnemiesInAttackRange();
        if(enemiesInRange.Count == 0)
        {
            GetWalkableTiles();
            UnitLoader closestAlly = FindClosestAlly();
            Node closestTile = FindClosestTile(closestAlly);
            currentMap.GeneratePathTo(closestTile.x, closestTile.y, currentEnemy);            
            NodeMove(currentEnemy);
            return;
        }
        else if(enemiesInRange.Count == 1)
        {
            CombatManager.instance.EngageAttack(currentEnemy, enemiesInRange[0]);
            return;
        }
        else if(enemiesInRange.Count >= 2)
        {
            CombatManager.instance.EngageAttack(currentEnemy, DetermineWeakestUnit());
            return;
        }
    }
    private void Boss()
    {
        currentEnemy.currentPath = null;
        GetEnemiesInAttackRange();
        if(enemiesInRange.Count == 0)
        {
            currentEnemy.Rest();
        }
        else if(enemiesInRange.Count == 1)
        {
            CombatManager.instance.EngageAttack(currentEnemy, enemiesInRange[0]);
            return;
        }
        else if(enemiesInRange.Count >= 2)
        {
            UnitLoader weakestAlly = DetermineWeakestUnit();
            CombatManager.instance.EngageAttack(currentEnemy, weakestAlly);
            return;
        }
    }
    private void Defensive()
    {
        currentEnemy.currentPath = null;
        GetEnemiesInWalkableRange();
        if(enemiesInRange.Count == 0)
        {
            currentEnemy.Rest();
        }
        else if(enemiesInRange.Count == 1)
        {
            GetWalkableTiles();
            UnitLoader closestAlly = enemiesInRange[0];
            Node closestTile = FindClosestTile(closestAlly);

            Node currentTile = currentMap.ReturnNodeAt((int)currentEnemy.transform.localPosition.x, (int)currentEnemy.transform.localPosition.y);

            if (closestTile != null && closestTile.DistanceTo(currentTile) <= currentEnemy.unit.statistics.movement + currentEnemy.equippedWeapon.range)
            {
                currentMap.GeneratePathTo(closestTile.x, closestTile.y, currentEnemy);
                NodeMove(currentEnemy);
                enemiesInRange.Clear();
                return;
            }
            else
            {
                enemiesInRange.Clear();
                currentEnemy.Rest();
            }
        }
        else if(enemiesInRange.Count >= 2)
        {
            GetWalkableTiles();
            UnitLoader closestAlly = DetermineWeakestUnit();
            Node closestTile = FindClosestTile(closestAlly);
            
            Node currentTile = currentMap.ReturnNodeAt((int)currentEnemy.transform.localPosition.x, (int)currentEnemy.transform.localPosition.y);

            if(closestTile != null && closestTile.DistanceTo(currentTile) <= currentEnemy.unit.statistics.movement + currentEnemy.equippedWeapon.range)
            {
                currentMap.GeneratePathTo(closestTile.x, closestTile.y, currentEnemy);
                NodeMove(currentEnemy);
                enemiesInRange.Clear();
                return;
            }
            else
            {
                enemiesInRange.Clear();
                currentEnemy.Rest();
            }
        }
    }

    public void SetEnemyOrder()
    {
        int i = 0;
        foreach(UnitLoader unit in TurnManager.instance.enemyUnits)
        {
            enemyOrder.Insert(i, TurnManager.instance.enemyUnits[i]);
            i++;
        }
        enemyOrder.Sort((x, y) => x.behaviorTag.order.CompareTo(y.behaviorTag.order));
    }
    
    private void GetWalkableTiles()
    {
        currentMap.DehighlightTiles();
        walkableTiles = currentMap.GenerateWalkableRange((int)(currentEnemy.transform.localPosition.x), (int)(currentEnemy.transform.localPosition.y), currentEnemy.unit.statistics.movement, currentEnemy);
    }
    private void GetEnemiesInAttackRange()
    {
        Node currentNode = currentMap.ReturnNodeAt((int)currentEnemy.transform.localPosition.x, (int)currentEnemy.transform.localPosition.y);
        Node enemyNode = null;

        foreach(UnitLoader unit in TurnManager.instance.allyUnits)
        {
            if(unit.unit.allyUnit)
            {
                enemyNode = currentMap.ReturnNodeAt((int)unit.transform.localPosition.x, (int)unit.transform.localPosition.y);
                if(currentNode.DistanceTo(enemyNode) <= currentEnemy.equippedWeapon.range)
                {
                    if(!enemiesInRange.Contains(unit))
                    {
                        enemiesInRange.Add(unit);
                    }
                }
            }
        }
    }
    private void GetEnemiesInWalkableRange()
    {
        Node currentNode = currentMap.ReturnNodeAt((int)currentEnemy.transform.localPosition.x, (int)currentEnemy.transform.localPosition.y);
        Node enemyNode = null;

        foreach (UnitLoader unit in TurnManager.instance.allyUnits)
        {
            if(unit.unit.allyUnit)
            {
                enemyNode = currentMap.ReturnNodeAt((int)unit.transform.localPosition.x, (int)unit.transform.localPosition.y);
                if(currentNode.DistanceTo(enemyNode) <= currentEnemy.equippedWeapon.range + currentEnemy.unit.statistics.movement)
                {
                    if(!enemiesInRange.Contains(unit))
                    {
                        enemiesInRange.Add(unit);
                    }
                }
            }
        }
    }

    private UnitLoader DetermineWeakestUnit()
    {
        UnitLoader weakestUnit = null;
        int highestDamage = 0;
        foreach(UnitLoader unit in enemiesInRange)
        {
            if (CombatManager.instance.Hit(currentEnemy, unit) > highestDamage)
            {
                highestDamage = CombatManager.instance.Hit(currentEnemy, unit);
                weakestUnit = unit;
            }
        }
        return weakestUnit;
    }
    private UnitLoader FindClosestAlly()
    {
        int closestSoFar = 100;
        UnitLoader closestUnit = null;
        Node currentNode = currentMap.ReturnNodeAt((int)currentEnemy.transform.localPosition.x, (int)currentEnemy.transform.localPosition.y);
        Node unitNode = null;
        foreach (UnitLoader unit in TurnManager.instance.allyUnits)
        {
            unitNode = currentMap.ReturnNodeAt((int)unit.transform.localPosition.x, (int)unit.transform.localPosition.y);
            if((currentNode.DistanceTo(unitNode)) <= closestSoFar)
            {
                closestSoFar = currentNode.DistanceTo(unitNode);
                closestUnit = unit;
            }
        }
        return closestUnit;
    }
    private Node FindClosestTile(UnitLoader unit)
    {
        int lowestSoFar = 100;

        Node targetTile = currentMap.ReturnNodeAt((int)unit.transform.localPosition.x, (int)unit.transform.localPosition.y);
        Node closestTile = null;
        
        foreach(Node tile in walkableTiles)
        {
            if(tile.DistanceTo(targetTile) < lowestSoFar && currentMap.CanTraverse(tile.x, tile.y))
            {
                lowestSoFar = tile.DistanceTo(targetTile);
                closestTile = tile;
            }
        }
        return closestTile;
    }

    private bool IsLastEnemy()
    {
        foreach(UnitLoader unit in TurnManager.instance.enemyUnits) {
            if (unit.rested == false) return false;
        }
        return true;
    }

    private void NodeMove(UnitLoader currentEnemy)
    {
        StartCoroutine(NodeMovement(currentEnemy));
    }
    private IEnumerator NodeMovement(UnitLoader currentEnemy)
    {
        var moveSpeed = 2;
        if(fastMode)
            moveSpeed = 8;
        else
            moveSpeed = 2;

        yield return new WaitForEndOfFrame();

        if(currentEnemy.currentPath != null)
        {
            Vector3 finalNode = new Vector3(currentEnemy.currentPath[currentEnemy.currentPath.Count - 1].x, currentEnemy.currentPath[currentEnemy.currentPath.Count - 1].y);
            while(currentEnemy.transform.localPosition != finalNode)
            {
                // Turn off all other animations so they don't start overlapping
                currentEnemy.animator.SetBool("Up", false);
                currentEnemy.animator.SetBool("Down", false);
                currentEnemy.animator.SetBool("Left", false);
                currentEnemy.animator.SetBool("Right", false);

                Vector3 nextNode = new Vector3(currentEnemy.currentPath[1].x, currentEnemy.currentPath[1].y);

                if (nextNode.x > currentEnemy.transform.localPosition.x && nextNode.y == currentEnemy.transform.localPosition.y)
                    currentEnemy.animator.SetBool("Right", true);
                else if (nextNode.x < currentEnemy.transform.localPosition.x && nextNode.y == currentEnemy.transform.localPosition.y)
                    currentEnemy.animator.SetBool("Left", true);
                else if (nextNode.x == currentEnemy.transform.localPosition.x && nextNode.y > currentEnemy.transform.localPosition.y)
                    currentEnemy.animator.SetBool("Up", true);
                else if (nextNode.x == currentEnemy.transform.localPosition.x && nextNode.y < currentEnemy.transform.localPosition.y)
                    currentEnemy.animator.SetBool("Down", true);

                SoundManager.instance.PlayFX(10);
                yield return new WaitForSeconds(0.01f);

                while (currentEnemy.transform.localPosition != nextNode)
                {
                    currentEnemy.transform.localPosition = Vector2.MoveTowards(currentEnemy.transform.localPosition, nextNode, moveSpeed * Time.deltaTime);
                    yield return null;
                }

                currentEnemy.currentPath.RemoveAt(0);
            }
            GetEnemiesInAttackRange();
            if(enemiesInRange.Count == 1)
                CombatManager.instance.EngageAttack(currentEnemy, enemiesInRange[0]);
            else if (enemiesInRange.Count >= 2)
                CombatManager.instance.EngageAttack(currentEnemy, DetermineWeakestUnit());
            else
                currentEnemy.Rest();

            currentEnemy.animator.SetBool("Up", false);
            currentEnemy.animator.SetBool("Down", false);
            currentEnemy.animator.SetBool("Left", false);
            currentEnemy.animator.SetBool("Right", false);
            currentEnemy.animator.CrossFade("Idle", 0.3f);

            currentMap.DehighlightTiles();
        }
    }

    private IEnumerator MoveCamera(UnitLoader enemy)
    {
        var moveSpeed = 5;
        if (fastMode){
            moveSpeed = 9;
        }
        else{
            moveSpeed = 5;
        }

        cameraTarget = new Vector3(enemy.transform.position.x, enemy.transform.position.y, -10);
        if(cameraTarget.x > cursor.cameraRight){
            cameraTarget.x = cursor.cameraRight;
        }
        if(cameraTarget.x < cursor.cameraLeft){
            cameraTarget.x = cursor.cameraLeft;
        }
        if(cameraTarget.y > cursor.cameraTop){
            cameraTarget.y = cursor.cameraTop;
        }
        if(cameraTarget.y < cursor.cameraBottom){
            cameraTarget.y = cursor.cameraBottom;
        }

        while(mainCamera.transform.position != cameraTarget)
        {
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, cameraTarget, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}