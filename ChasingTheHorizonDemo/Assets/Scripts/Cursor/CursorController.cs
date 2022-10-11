using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
//Handles the functionality of the Cursor
//There should only be 1 CursorController script in any given scene
public class CursorController : MonoBehaviour
{
    public PlayerInput cursorControls;

    private TileMap currentMap = null;

    private UnitLoader selectedUnit;

    public Sprite highlight = null;
    public Vector2 currentPosition = new Vector3(0, 0);
    public bool enemyTurn = false;
    public GameObject enemyInventory = null;
    public SpriteRenderer spriteRenderer = null;

    [Header("Movement Contraints")] //These variables define the limits of the cursors range of movement
    [SerializeField] private float topMost = 0;
    [SerializeField] private float bottomMost = 0;
    [SerializeField] private float leftMost = 0;
    [SerializeField] private float rightMost = 0;
    [Header("Camera Constraints")] //These variables define the limits where the camera can move
    public float cameraTop = 0;
    public float cameraBottom = 0;
    public float cameraLeft = 0;
    public float cameraRight = 0;

    //REFERENCES
    [SerializeField] private GameObject menu = null;
    [SerializeField] private Camera mapCamera = null;
    [Header("Map Frame Points")] //These variables define at which point your cursor needs to be for the camera to move in the respective direction
    [SerializeField] private Transform frameTop = null;
    [SerializeField] private Transform frameBottom = null;
    [SerializeField] private Transform frameLeft = null;
    [SerializeField] private Transform frameRight = null;

    public Animator animator = null;

    public CursorState currentState;
    public CursorState previousState;

    private bool cursorMoving = false;
    private Stack movementRequests = new Stack();
    public bool buttonHeld = false;
    
    private IEnumerator buttonHeldCoroutine = null;

    public void SetState(CursorState state)
    {
        previousState = currentState;
        currentState = state;
        currentState.Start();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentPosition = transform.localPosition;
        highlight = GetComponent<SpriteRenderer>().sprite;
        enemyTurn = false;
        currentMap = FindObjectOfType<TileMap>();
        SetState(new MapState(this));
    }    

    public void RequestMove(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            buttonHeldCoroutine = ButtonHeld(ctx.ReadValue<Vector2>());
            StartCoroutine(buttonHeldCoroutine);
            if(!cursorMoving)
            {
                movementRequests.Push(ctx.ReadValue<Vector2>());
                StartCoroutine(StartMovement(10f));
            }
        }
        if(ctx.canceled)
        {
            ButtonReleased();
        }
    }
    private void ButtonReleased()
    {
        buttonHeld = false;
        movementRequests.Clear();
        StopCoroutine(buttonHeldCoroutine);
    }
    private IEnumerator ButtonHeld(Vector2 direction)
    {
        yield return new WaitForSeconds(0.2f);
        buttonHeld = true;
        while(buttonHeld)
        {
            movementRequests.Push(direction);
            StartCoroutine(StartMovement(10f));
            yield return new WaitForSeconds(0.1f);
        }
    }
    private IEnumerator StartMovement(float moveSpeed)
    {
        SoundManager.instance.PlayFX(2);

        Vector2 poppedRequest = (Vector2)movementRequests.Peek();
        currentPosition += (Vector2)movementRequests.Pop();

        if(currentPosition.x < leftMost || currentPosition.x > rightMost){
            currentPosition -= poppedRequest;
            yield break;
        }
        if(currentPosition.y < bottomMost || currentPosition.y > topMost){
            currentPosition -= poppedRequest;
            yield break;
        }
        SoundManager.instance.PlayFX(2);
        //MoveCamera();

        while((Vector2)transform.localPosition != currentPosition)
        {
            cursorMoving = true;
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, currentPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        cursorMoving = false;
    }
    
    private void MoveCamera()
    {
        if(currentPosition.x >= frameRight.position.x)
        {
            if(mapCamera.transform.position.x < cameraRight)
            {
                StartCoroutine(StartCameraMove(new Vector3(mapCamera.transform.position.x + 1f, mapCamera.transform.position.y, -10)));
            }
        }
        if(currentPosition.x <= frameLeft.position.x)
        {
            if(mapCamera.transform.position.x > cameraLeft)
            {
                StartCoroutine(StartCameraMove(new Vector3(mapCamera.transform.position.x - 1f, mapCamera.transform.position.y, -10)));
            }
        }

        if(currentPosition.y >= frameTop.position.y)
        {
            if(mapCamera.transform.position.y < cameraTop)
            {
                StartCoroutine(StartCameraMove(new Vector3(mapCamera.transform.position.x, mapCamera.transform.position.y + 1f, -10)));
            }
        }
        if(currentPosition.y <= frameBottom.position.y)
        {
            if(mapCamera.transform.position.y > cameraBottom)
            {
                StartCoroutine(StartCameraMove(new Vector3(mapCamera.transform.position.x, mapCamera.transform.position.y - 1f, -10)));
            }
        }
    }
    private IEnumerator StartCameraMove(Vector3 targetPosition)
    {
        while(mapCamera.transform.position != targetPosition)
        {
            mapCamera.transform.position = Vector3.MoveTowards(mapCamera.transform.position, targetPosition, 20f * Time.deltaTime);
            yield return null;
        }
    }   

    public void Confirm(InputAction.CallbackContext ctx)
    {
        if(ctx.performed && currentState != null)
            currentState.Confirm();
    }
    public void Cancel(InputAction.CallbackContext ctx)
    {
        if(ctx.performed && currentState != null)
            currentState.Cancel();
    }

    public void SelectUnit()
    {
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        if(col.Length > 0)
        {
            UnitLoader unit = col[0].GetComponent<UnitLoader>();
            if(unit.unit.allyUnit && unit.rested == false)
            {
                animator.SetTrigger("Rotate");
                enemyInventory.SetActive(false);
                SoundManager.instance.PlayFX(0);
                currentMap.selectedUnit = unit;
                unit.Selected();
                SetState(new UnitState(this));
            }
        }
    }
    public void MoveSelectUnit()
    {       
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        if(col.Length > 0)
        {
            if(selectedUnit)
            {
                UnitLoader secondUnit = col[0].GetComponent<UnitLoader>();
                Vector3 selectedUnitPos = selectedUnit.transform.position;
                selectedUnit.transform.position = secondUnit.transform.position;
                selectedUnit.startPosition = transform.position;
                secondUnit.transform.position = selectedUnitPos;
                secondUnit.startPosition = transform.position;
                selectedUnit = null;
            }
            else
            {
                selectedUnit = col[0].GetComponent<UnitLoader>();
            }
        }
    }
    public void MoveDeselectUnit()
    {
        selectedUnit = null;
    }
    public void TradeSelectUnit()
    {
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        if (col.Length <= 0) return;
        UnitLoader unit = col[0].GetComponent<UnitLoader>();
        if (!unit.unit.allyUnit) return;
        TradeMenu.instance.BeginTrade(unit);
    }
    public void ConvoySelect()
    {
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        if (col.Length <= 0) return;
        UnitLoader unit = col[0].GetComponent<UnitLoader>();
        if (!unit.unit.allyUnit) return;
        if (unit.smallConvoy.Count <= 0) return;
        ConvoyTradeMenu.instance.OpenConvoy(unit);
    }
    public void DeselectUnit()
    {
        if(currentMap.selectedUnit != null)
        {
            if(currentMap.selectedUnit.currentPath == null)
            {
                animator.SetTrigger("CounterRotate");
                SoundManager.instance.PlayFX(1);
                currentMap.selectedUnit.animator.SetBool("Selected", false);
                currentMap.selectedUnit = null;
                currentMap.DehighlightTiles();
                SetState(new MapState(this));
            }
        }
    }
    public void SelectEnemy()
    {
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        if (col.Length > 0)
        {
            UnitLoader unit = col[0].GetComponent<UnitLoader>();
            if (!unit.unit.allyUnit)
            {
                SoundManager.instance.PlayFX(0);
                currentMap.DehighlightTiles();
                currentMap.walkableTiles = currentMap.GenerateWalkableRange((int)unit.transform.localPosition.x, (int)unit.transform.localPosition.y, unit.unit.statistics.movement, unit);
                currentMap.HighlightTiles();
                unit.spriteRenderer.color = Color.red;
                enemyInventory.SetActive(true);
                enemyInventory.GetComponent<EnemyInventory>().DisplayInventory(unit);
            }
        }
    }
    public void ResetTiles()
    {
        currentMap.DehighlightTiles();
        foreach(UnitLoader unit in currentMap.enemyUnits)
        {
            if(unit.spriteRenderer.color == Color.red)
            {
                unit.spriteRenderer.color = Color.white;
                enemyInventory.SetActive(false);
            }
        }
    }
    public void MoveUnit()
    {
        foreach(Node n in currentMap.walkableTiles)
        {
            if(new Vector3(currentPosition.x, currentPosition.y) == new Vector3(n.x, n.y) && currentMap.CanTraverse(n.x, n.y))
            {                
                if(currentMap.selectedUnit.hasMoved == false)
                {
                    SoundManager.instance.PlayFX(0);
                    currentMap.selectedUnit.originalPosition = currentMap.selectedUnit.transform.localPosition;
                    currentMap.GeneratePathTo(n.x, n.y, currentMap.selectedUnit);
                    currentMap.selectedUnit.FollowPath();
                    break;
                }
            }
        }
    }
    public void AttackMove()
    {
        if(transform.localPosition == currentMap.selectedUnit.transform.localPosition) { return; }

        Vector3 currentEnemy = new Vector3(0, 0);
        foreach(UnitLoader unit in currentMap.enemyUnits)
        {
            if(transform.localPosition == unit.transform.localPosition && !unit.unit.allyUnit)
            {
                if(Vector2.Distance(currentMap.selectedUnit.transform.localPosition, unit.transform.localPosition) <= currentMap.selectedUnit.unit.statistics.movement + currentMap.selectedUnit.equippedWeapon.range)
                {
                    currentEnemy = unit.transform.localPosition;
                }
            }
        }

        foreach (Node n in currentMap.walkableTiles)
        {
            if(Vector2.Distance(new Vector2(n.x, n.y), currentEnemy) == currentMap.selectedUnit.equippedWeapon.range && currentMap.CanTraverse(n.x, n.y))
            {
                SoundManager.instance.PlayFX(0);
                currentMap.selectedUnit.originalPosition = currentMap.selectedUnit.transform.localPosition;
                currentMap.GeneratePathTo(n.x, n.y, currentMap.selectedUnit);
                currentMap.selectedUnit.FollowPath();
                break;
            }
        }
    }

    public void UndoMove()
    {
        SoundManager.instance.PlayFX(1);
        foreach (UnitLoader unit in currentMap.enemyUnits)
        {
            unit.spriteRenderer.color = Color.white;
        }
        currentMap.selectedUnit.enemiesInRange.Clear();
        currentMap.selectedUnit.transform.localPosition = currentMap.selectedUnit.originalPosition;
        currentMap.selectedUnit.hasMoved = false;
        currentMap.selectedUnit.currentPath = null;
        currentMap.DehighlightTiles();
        currentMap.selectedUnit.ActionMenu();
        currentMap.selectedUnit.target = null;
        currentMap.selectedUnit = null;
        SetState(new MapState(this));
        cursorControls.SwitchCurrentActionMap("MapScene");
    }
    
    public void SelectTarget()
    {
        SoundManager.instance.PlayFX(0);
        foreach (UnitLoader unit in currentMap.enemyUnits)
        {
            if(transform.localPosition == unit.transform.localPosition && currentMap.selectedUnit.enemiesInRange.Contains(unit) && 
                Vector2.Distance(currentMap.selectedUnit.transform.localPosition, unit.transform.localPosition) <= currentMap.selectedUnit.equippedWeapon.range)
            {
                currentMap.selectedUnit.target = unit;
                ActionMenuManager.instance.combatPreview.SetActive(true);
                ActionMenuManager.instance.weaponSelection.SetActive(true);
                cursorControls.SwitchCurrentActionMap("UI");
                SetState(new CombatPreviewState(this));
            }
        }
    }    
    public void CancelAttack()
    {
        SoundManager.instance.PlayFX(1);
        currentMap.selectedUnit.target = null;
        ActionMenuManager.instance.combatPreview.SetActive(false);
        ActionMenuManager.instance.weaponSelection.SetActive(false);
        cursorControls.SwitchCurrentActionMap("MapScene");
        SetState(new AttackState(this));
    }
    public void AttackTarget()
    {
        currentMap.DehighlightTiles();
        CombatManager.instance.EngageAttack(currentMap.selectedUnit, currentMap.selectedUnit.target);
        currentMap.selectedUnit.target.spriteRenderer.color = Color.white;
        currentMap.selectedUnit = null;
        ActionMenuManager.instance.combatPreview.SetActive(false);
        ActionMenuManager.instance.weaponSelection.SetActive(false);
        ActionMenuManager.instance.gameObject.SetActive(false);
    }
    public void UndoAttack()
    {
        SoundManager.instance.PlayFX(1);
        currentMap.DehighlightTiles();
        cursorControls.SwitchCurrentActionMap("UI");
        SetState(new ActionMenuState(this));
        ActionMenuPlus.instance.Toggle();
        //ActionMenuManager.instance.Highlight();
    }

    public void DisplayMenu()
    {
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        if (col.Length <= 0)
        {
            menu.SetActive(true);
            SoundManager.instance.PlayFX(11);
            SetState(new MenuState(this));
            cursorControls.SwitchCurrentActionMap("UI");
        }
    }
    public void CloseMenu()
    {
        if(enemyInventory.activeSelf)
        {
            enemyInventory.SetActive(false);
        }
        if(menu.transform.GetChild(0).gameObject.activeSelf == true && menu.transform.GetChild(1).gameObject.activeSelf == true)
        {
            menu.transform.GetChild(0).gameObject.SetActive(false);
            menu.transform.GetChild(1).gameObject.SetActive(false);
            return;
        }
        else if(menu.transform.GetChild(0).gameObject.activeSelf == true)
        {
            menu.transform.GetChild(0).gameObject.SetActive(false);
            return;
        }
        else if(menu.transform.GetChild(1).gameObject.activeSelf == true)
        {
            menu.transform.GetChild(1).gameObject.SetActive(false);
            menu.GetComponent<MenuManager>().Highlight();
            return;
        }        
        else
        {
            menu.SetActive(false);
            SetState(new MapState(this));
            cursorControls.SwitchCurrentActionMap("MapScene");
            return;
        }
    }    
}