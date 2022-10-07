using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//The CombatManager handles all combat between units
//There should only be 1 CombatManager in a given scene
//You can initiate combat between two units in any script

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance { get; private set; }

    //VARIABLES 
    private Vector3 originalCameraPosition = new Vector3(10.5f, 10.5f, -10);
    private bool dialoguePlayed = false;

    //REFERENCES
    [SerializeField] CursorController cursor = null;
    [SerializeField] private Camera mainCamera = null;
    [SerializeField] private Animator cameraAnimator = null;
    [SerializeField] private Canvas canvas = null;
    [SerializeField] private GameObject screenDim = null;
    [SerializeField] private GameObject combatReadout = null;
    [SerializeField] private Slider attackerHealth = null;
    [SerializeField] private Slider defenderHealth = null;
    [SerializeField] private Slider expSlider = null;
    [SerializeField] private BattleText battleText = null;

    private bool didHit;

    private int lowhealthIndicator = 0;

    private void Awake()
    {
        instance = this;
    }

    public void EngageAttack(UnitLoader attacker, UnitLoader defender)
    {
        StartCoroutine(Attack(attacker, defender));

        attackerHealth.maxValue = attacker.unit.statistics.health;
        attackerHealth.value = attacker.currentHealth;

        defenderHealth.maxValue = defender.unit.statistics.health;
        defenderHealth.value = defender.currentHealth;

        didHit = false;
    }
    private IEnumerator Attack(UnitLoader attacker, UnitLoader defender)
    {
        dialoguePlayed = false;

        foreach(UnitLoader unit in FindObjectsOfType<UnitLoader>()){
            if(unit.spriteRenderer.color == Color.red){
                unit.spriteRenderer.color = Color.white;
            }
        }

        cursor.cursorControls.currentActionMap.Disable();
        cursor.animator.SetBool("Invisible", true);

        //Check if defender has dialogue for when it's attacked
        if(defender.attackedDialogue != null){
            MapDialogueManager.instance.WriteSingle(defender.attackedDialogue);
            yield return new WaitForSeconds(0.5f);
            defender.attackedDialogue = null;
        }
        yield return new WaitUntil(() => !screenDim.activeSelf);

        attackerHealth.GetComponent<RectTransform>().anchoredPosition = WorldToCanvasSpace(attacker.gameObject, defender.gameObject);
        defenderHealth.GetComponent<RectTransform>().anchoredPosition = WorldToCanvasSpace(defender.gameObject, attacker.gameObject);

        combatReadout.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        AttackAnimation(attacker, defender);
        yield return new WaitForSeconds(attacker.animator.GetCurrentAnimatorClipInfo(0).Length - 0.2f);        

        if(attacker.equippedWeapon.animation)
        {
            PlayEffect(attacker, defender);
            yield return new WaitForSeconds(0.3f);
        }

        InitiatorAttack(attacker, defender);
        yield return new WaitForSeconds(0.3f);

        if(CheckForDeaths(attacker, defender) == "Attacker")
        {
            attacker.Death();
            yield return new WaitForSeconds(0.3f);            
            combatReadout.SetActive(false);
            if (!attacker.unit.allyUnit)
            {
                GainExperience(attacker, defender);
                yield return new WaitForSeconds(1f);
                yield return new WaitUntil(() => !expSlider.gameObject.activeSelf);
            }
            if (attacker.unit.allyUnit)
            {
                cursor.cursorControls.SwitchCurrentActionMap("MapScene");
                cursor.SetState(new MapState(cursor));
            }
            yield return null;
        }
        else if(CheckForDeaths(attacker, defender) == "Defender")
        {
            //Checks if the defender has dialogue for when its defeated
            if(!dialoguePlayed && defender.defeatedDialogue != null){
                MapDialogueManager.instance.WriteSingle(defender.defeatedDialogue);
                dialoguePlayed = true;
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitUntil(() => !screenDim.activeSelf);

            defender.Death();
            yield return new WaitForSeconds(0.2f);            
            combatReadout.SetActive(false);
            if (!defender.unit.allyUnit)
            {
                GainExperience(attacker, defender);
                yield return new WaitForSeconds(1f);
                yield return new WaitUntil(() => !expSlider.gameObject.activeSelf);
            }
            attacker.Rest();
            if(attacker.unit.allyUnit)
            {
                cursor.cursorControls.SwitchCurrentActionMap("MapScene");
                cursor.SetState(new MapState(cursor));
            }
            yield return null;
        }
        else
        {
            if(CheckDistance(defender, attacker) <= 1 || defender.equippedWeapon.range >= attacker.equippedWeapon.range)
            {
                AttackAnimation(defender, attacker);
                yield return new WaitForSeconds((defender.animator.GetCurrentAnimatorClipInfo(0).Length - 0.2f));

                if(defender.equippedWeapon.animation)
                {
                    PlayEffect(defender, attacker); 
                    yield return new WaitForSeconds(defender.equippedWeapon.animationLength.length);
                }

                DefenderAttack(attacker, defender);
                yield return new WaitForSeconds(0.3f);
            }

            if(CheckForDeaths(attacker, defender) == "Attacker")
            {
                attacker.Death();
                yield return new WaitForSeconds(0.2f);                
                combatReadout.SetActive(false);
                if (!attacker.unit.allyUnit)
                {
                    GainExperience(attacker, defender);
                    yield return new WaitForSeconds(1f);
                    yield return new WaitUntil(() => !expSlider.gameObject.activeSelf);
                }
                if (attacker.unit.allyUnit)
                {
                    cursor.cursorControls.SwitchCurrentActionMap("MapScene");
                    cursor.SetState(new MapState(cursor));
                }
                yield return null;
            }
            else if(CheckForDeaths(attacker, defender) == "Defender")
            {
                //Checks if the defender has dialogue for when its defeated
                if(!dialoguePlayed && defender.defeatedDialogue != null){
                    MapDialogueManager.instance.WriteSingle(defender.defeatedDialogue);
                    dialoguePlayed = true;
                    yield return new WaitForSeconds(0.5f);
                }
                yield return new WaitUntil(() => !screenDim.activeSelf);

                defender.Death();
                yield return new WaitForSeconds(0.2f);               
                combatReadout.SetActive(false);
                if (!defender.unit.allyUnit)
                {
                    GainExperience(attacker, defender);
                    yield return new WaitForSeconds(1f);
                    yield return new WaitUntil(() => !expSlider.gameObject.activeSelf);
                }
                attacker.Rest();
                if(attacker.unit.allyUnit)
                {
                    cursor.cursorControls.SwitchCurrentActionMap("MapScene");
                    cursor.SetState(new MapState(cursor));
                }
                yield break;
            }
            else
            {
                if(CheckAttackSpeed(attacker, defender))
                {
                    AttackAnimation(attacker, defender);
                    yield return new WaitForSeconds(attacker.animator.GetCurrentAnimatorClipInfo(0).Length - 0.2f);

                    if(attacker.equippedWeapon.animation)
                    {
                        PlayEffect(attacker, defender);
                        yield return new WaitForSeconds(attacker.equippedWeapon.animationLength.length);
                    }

                    InitiatorAttack(attacker, defender);
                    yield return new WaitForSeconds(0.3f);

                    cursor.SetState(new MapState(cursor));
                    yield return null;
                }
                yield return new WaitForSeconds(0.3f);
                combatReadout.SetActive(false);
                GainExperience(attacker, defender);
                yield return new WaitForSeconds(1f);
                yield return new WaitUntil(() => !expSlider.gameObject.activeSelf);
                attacker.Rest();
                if(attacker.unit.allyUnit)
                {
                    cursor.cursorControls.SwitchCurrentActionMap("MapScene");
                    cursor.SetState(new MapState(cursor));
                }
                yield return null;
            }
            combatReadout.SetActive(false);
            yield return new WaitUntil(() => !expSlider.gameObject.activeSelf);
            if (CheckForDeaths(attacker, defender) == "Defender" || CheckForDeaths(attacker, defender) == null)
            {
                defender.Death();
                attacker.Rest();
            }
            if (attacker.unit.allyUnit)
            {
                cursor.cursorControls.SwitchCurrentActionMap("MapScene");
                cursor.SetState(new MapState(cursor));
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        combatReadout.SetActive(false);
        yield return new WaitUntil(() => !expSlider.gameObject.activeSelf);
        if(CheckForDeaths(attacker, defender) == "Defender" || CheckForDeaths(attacker, defender) == null)
        {
            attacker.Rest();
        }
        if(attacker.unit.allyUnit && !cursor.enemyTurn)
        {
            cursor.cursorControls.SwitchCurrentActionMap("MapScene");
            cursor.SetState(new MapState(cursor));
        }

        //This block checks if an Ally Unit is below 50% health, which if true triggers their battle dialogue
        if(attacker.unit.allyUnit && attacker.currentHealth < (attacker.unit.statistics.health * .5f) && attacker.currentHealth > 0 && !dialoguePlayed && !attacker.below50Quote){
            MapDialogueManager.instance.WriteSingle(attacker.battleDialogue.under50Quote);
            dialoguePlayed = true;
            attacker.below50Quote = true;
        }
        else if(defender.unit.allyUnit && defender.currentHealth < (defender.unit.statistics.health * .5f) && defender.currentHealth > 0 && !dialoguePlayed && !defender.below50Quote)
        {
            MapDialogueManager.instance.WriteSingle(defender.battleDialogue.under50Quote);
            dialoguePlayed = true;
            defender.below50Quote = true;
        }

        if(TurnManager.instance.currentState.stateType == TurnState.StateType.Player)
        {
            cursor.animator.SetBool("Invisible", false);
        }

        yield return null;
    }

    private void ActionCamera(UnitLoader attacker, UnitLoader defender)
    {
        var point1 = attacker.transform.localPosition;
        var point2 = defender.transform.localPosition;       

        if(point1.x < cursor.cameraLeft)
        {
            point1.x = cursor.cameraLeft;
        }
        else if(point2.x < cursor.cameraLeft)
        {
            point2.x = cursor.cameraLeft;
        }

        var centerPoint = (point1 + point2) / 2;        

        Vector3 zoomPoint = new Vector3(centerPoint.x, centerPoint.y, -10);

        StartCoroutine(MoveCamera(zoomPoint));
    }
    private IEnumerator MoveCamera(Vector3 targetPosition)
    {
        while(mainCamera.transform.position != targetPosition)
        {
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, targetPosition, 7f * Time.deltaTime);
            yield return null;
        }
        cameraAnimator.SetTrigger("ZoomIn");
    }
    private void ResetCamera()
    {
        StartCoroutine(ZoomOutCamera());
    }
    private IEnumerator ZoomOutCamera()
    {
        Vector3 targetPosition = new Vector3(cursor.cameraLeft, mainCamera.transform.position.y, mainCamera.transform.position.z);
        cameraAnimator.SetTrigger("ZoomOut");
        yield return new WaitForSeconds(0.6f);
        if(mainCamera.transform.position.x < cursor.cameraLeft)
        {
            while(mainCamera.transform.position.x != cursor.cameraLeft)
            {
                mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, targetPosition, 7f * Time.deltaTime);
                yield return null;
            }
        }
    }
    
    private Vector3 WorldToCanvasSpace(GameObject unit)
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 uiOffset = new Vector2((float)canvasRect.sizeDelta.x / 2f, (float)canvasRect.sizeDelta.y / 2f);
        Vector2 viewPortPosition = mainCamera.WorldToViewportPoint(unit.transform.position);
        Vector2 proportionalPosition = new Vector2(viewPortPosition.x * canvasRect.sizeDelta.x, viewPortPosition.y * canvasRect.sizeDelta.y);
        return proportionalPosition - uiOffset - new Vector2(0, 50);
    }
    private Vector3 WorldToCanvasSpace(GameObject attacker, GameObject defender)
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 uiOffset = new Vector2((float)canvasRect.sizeDelta.x / 2f, (float)canvasRect.sizeDelta.y / 2f);
        Vector2 viewPortPosition = mainCamera.WorldToViewportPoint(attacker.transform.position);
        Vector2 proportionalPosition = new Vector2(viewPortPosition.x * canvasRect.sizeDelta.x, viewPortPosition.y * canvasRect.sizeDelta.y);
        
        if(attacker.transform.position.x > defender.transform.position.x)
        {
            return proportionalPosition - uiOffset - new Vector2(-46, 0);
        }
        else if(attacker.transform.position.x < defender.transform.position.x)
        {
            return proportionalPosition - uiOffset - new Vector2(46, 0);
        }
        else if(attacker.transform.position.y < defender.transform.position.y && attacker.transform.position.x == defender.transform.position.x)
        {
            return proportionalPosition - uiOffset - new Vector2(46, 0);
        }
        else if (attacker.transform.position.y > defender.transform.position.y && attacker.transform.position.x == defender.transform.position.x)
        {
            return proportionalPosition - uiOffset - new Vector2(-46, 0);
        }
        return new Vector3(0, 0);
    }

    private IEnumerator Shake(float intensity, bool attacker)
    {
        Vector3 originalPosition = new Vector3(0, 0, 0);
        GameObject healthBar = null;
        bool isShaking = true;
        var startTime = Time.time;
        
        //Determine which healthbar to shake
        if(attacker){
            originalPosition = defenderHealth.gameObject.transform.position;
            healthBar = defenderHealth.gameObject;
        }
        else{
            originalPosition = attackerHealth.gameObject.transform.position;
            healthBar = attackerHealth.gameObject;
        }

        //Determine intensity of shake
        if(intensity > 10){
            intensity = 0.4f;
        }
        else if(intensity < 10){
            intensity = 0.2f;
        }

        //Shake the healthbar
        while(isShaking && (Time.time - startTime) < 0.2f){
            healthBar.transform.position = originalPosition + Random.insideUnitSphere * intensity;
            yield return null;
        }

        //Resets healthbar position
        isShaking = false;
        if(attacker){
            defenderHealth.transform.position = originalPosition;
        }
        else{
            attackerHealth.transform.position = originalPosition;
        }
    }

    private void InitiatorAttack(UnitLoader attacker, UnitLoader defender)
    {
        //Check for a hit
        if(HitRoll(attacker))
        {
            //Checks for a crit
            if(CritRoll(attacker))
            {
                //Unit Crits                
                SoundManager.instance.PlayFX(4);
                battleText.SetText("Crit");                
                Instantiate(battleText, defender.transform.position, Quaternion.identity);                
                defender.currentHealth = defender.currentHealth - Critical(attacker, defender);
                StartCoroutine(Shake(Critical(attacker, defender), true));
                defenderHealth.value = defender.currentHealth;

                //Display critial quote
                if (attacker.unit.allyUnit && !defender.defeatedDialogue)
                {
                    MapDialogueManager.instance.WriteSingle(attacker.battleDialogue.RandomCritQuote());
                    dialoguePlayed = true;
                }
            }
            else
            {
                //Unit Hits
                SoundManager.instance.PlayFX(3);
                battleText.SetText("Hit"); 
                Instantiate(battleText, defender.transform.position, Quaternion.identity);
                defender.currentHealth = defender.currentHealth - Hit(attacker, defender);
                StartCoroutine(Shake(Hit(attacker, defender), true));
                defenderHealth.value = defender.currentHealth;
            }
        }
        else
        {
            //Unit Misses
            SoundManager.instance.PlayFX(5);
            battleText.SetText("Miss");
            Instantiate(battleText, defender.transform.position, Quaternion.identity);
            return;
        }
    }
    private void DefenderAttack(UnitLoader attacker, UnitLoader defender)
    {
        if(CheckDistance(defender, attacker) <= 1 || defender.equippedWeapon.range >= attacker.equippedWeapon.range)
        {
            //Check for a hit
            if(HitRoll(defender))
            {
                //Checks for a crit
                if(CritRoll(defender))
                {
                    //Unit Crits
                    SoundManager.instance.PlayFX(4);
                    battleText.SetText("Crit");
                    Instantiate(battleText, attacker.transform.position, Quaternion.identity);
                    attacker.currentHealth = attacker.currentHealth - Critical(defender, attacker);
                    StartCoroutine(Shake(Critical(defender, attacker), false));
                    attackerHealth.value = attacker.currentHealth;
                    
                    //Display critial quote
                    if(defender.unit.allyUnit && !attacker.defeatedDialogue)
                    {
                        MapDialogueManager.instance.WriteSingle(defender.battleDialogue.RandomCritQuote());
                        dialoguePlayed = true;
                    }
                }
                else
                {
                    //Unit Hits
                    SoundManager.instance.PlayFX(3);
                    battleText.SetText("Hit");
                    Instantiate(battleText, attacker.transform.position, Quaternion.identity);
                    attacker.currentHealth = attacker.currentHealth - Hit(defender, attacker);
                    StartCoroutine(Shake(Hit(defender, attacker), false));
                    attackerHealth.value = attacker.currentHealth;
                }
            }
            else
            {
                //Unit Misses
                SoundManager.instance.PlayFX(5);
                battleText.SetText("Miss");
                Instantiate(battleText, attacker.transform.position, Quaternion.identity);
                return;
            }
        }
    }        

    private void AttackAnimation(UnitLoader attacker, UnitLoader defender)
    {
        //XY Attacks
        if(attacker.transform.position.y == defender.transform.position.y)
        {
            if (attacker.transform.position.x < defender.transform.position.x)
            {
                attacker.animator.SetTrigger("AttackRight"); //Unit is to the left of enemy so it attacks right
                return;
            }
            else if (attacker.transform.position.x > defender.transform.position.x)
            {
                attacker.animator.SetTrigger("AttackLeft"); //Unit is to the right of the enemy so it attacks left
                return;
            }
        }
        else if (attacker.transform.position.x == defender.transform.position.x)
        {
            if (attacker.transform.position.y < defender.transform.position.y)
            {
                attacker.animator.SetTrigger("AttackUp"); //Unit is below the enemy so it attacks up
                return;
            }
            else if (attacker.transform.position.y > defender.transform.position.y)
            {
                attacker.animator.SetTrigger("AttackDown"); //Unit is above the enemy so it attacks down
                return;
            }
        }

        //Checks if a unit is in a diagonal position but doesn't have diagonal attack animations
        else if(!attacker.unit.diagonal)
        {
            if (attacker.transform.position.x < defender.transform.position.x && attacker.transform.position.y < defender.transform.position.y)
            {
                attacker.animator.SetTrigger("AttackUp");
                return;
            }
            if (attacker.transform.position.x > defender.transform.position.x && attacker.transform.position.y < defender.transform.position.y)
            {
                attacker.animator.SetTrigger("AttackUp");
                return;
            }
            if (attacker.transform.position.x < defender.transform.position.x && attacker.transform.position.y > defender.transform.position.y)
            {
                attacker.animator.SetTrigger("AttackDown");
                return;
            }
            if (attacker.transform.position.x > defender.transform.position.x && attacker.transform.position.y > defender.transform.position.y)
            {
                attacker.animator.SetTrigger("AttackDown");
                return;
            }
        }

        //Diagonal Attacks
        else if(attacker.unit.diagonal)
        {
            if (attacker.transform.position.x < defender.transform.position.x && attacker.transform.position.y < defender.transform.position.y)
            {
                attacker.animator.SetTrigger("AttackUpRight"); //Unit is down left of the enemy so it attacks up right
                return;
            }
            if (attacker.transform.position.x > defender.transform.position.x && attacker.transform.position.y < defender.transform.position.y)
            {
                attacker.animator.SetTrigger("AttackUpLeft"); //Unit is down right of the enemy so it attacks up left
                return;
            }
            if (attacker.transform.position.x < defender.transform.position.x && attacker.transform.position.y > defender.transform.position.y)
            {
                attacker.animator.SetTrigger("AttackDownRight"); //Unit is up right of the enemy so it attacks down left
                return;
            }
            if (attacker.transform.position.x > defender.transform.position.x && attacker.transform.position.y > defender.transform.position.y)
            {
                attacker.animator.SetTrigger("AttackDownLeft"); //Unit is up left of the enemy so it attacks down right
                return;
            }
        }
    }
    private void PlayEffect(UnitLoader attacker, UnitLoader defender)
    {
        StartCoroutine(EffectAnimation(attacker, defender));
    }
    private IEnumerator EffectAnimation(UnitLoader attacker, UnitLoader defender)
    {
        var effect = Instantiate(attacker.equippedWeapon.animation, defender.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(attacker.equippedWeapon.animationLength.length);
        Destroy(effect);        
        yield return null;
    }
    
    private bool HitRoll(UnitLoader unit)
    {
        int roll = Random.Range(0, 99);

        if (roll > unit.CombatStatistics().hit)
            return false;

        else
            if (unit.unit.allyUnit) didHit = true;            
            return true;
    }
    private bool CritRoll(UnitLoader unit)
    {
        int roll = Random.Range(0, 99);

        if (roll > unit.CombatStatistics().crit)
            return false;

        else
            if (unit.unit.allyUnit) didHit = true;
            return true;
    }

    public int Hit(UnitLoader attacker, UnitLoader defender)
    {
        return attacker.CombatStatistics().attack - defender.CombatStatistics().protection;
    }
    private int Critical(UnitLoader attacker, UnitLoader defender)
    {
        return attacker.CombatStatistics().attack * 2 - defender.CombatStatistics().protection;
    }

    private string CheckForDeaths(UnitLoader attacker, UnitLoader defender)
    {
        if(defender.currentHealth <= 0)
        {
            AIManager.instance.enemyOrder.Remove(defender);
            return "Defender";
        }
        else if(attacker.currentHealth <= 0)
        {
            return "Attacker";
        }
        return null;
    }
    private bool CheckAttackSpeed(UnitLoader attacker, UnitLoader defender)
    {
        if(attacker.CombatStatistics().attackSpeed > defender.CombatStatistics().attackSpeed + 5)
            return true;
        else
            return false;
    }
    private float CheckDistance(UnitLoader unit1, UnitLoader unit2)
    {
        return Mathf.Abs(unit1.transform.position.x - unit2.transform.position.x) + Mathf.Abs(unit1.transform.position.y - unit2.transform.position.y);
    }

    private void GainExperience(UnitLoader attacker, UnitLoader defender)
    {
        //if the ally doesn't hit, don't gain exp
        //if the ally kills, gain extra exp
        if(!didHit)
        {
            return;
        }
        else
        {
            if(attacker.unit.allyUnit)
            {
                int gainedExperience = (1 * (defender.level + 9) / 10) * (Mathf.Abs(attacker.level - defender.level) + 1);
                
                expSlider.gameObject.SetActive(true);
                expSlider.GetComponent<RectTransform>().anchoredPosition = WorldToCanvasSpace(attacker.gameObject);
                expSlider.value = attacker.exp;
                attacker.GainExperience(10);
                StartCoroutine(UpdateEXP(attacker));
                return;
            }
            else if(defender.unit.allyUnit)
            {
                int gainedExperience = (1 * (attacker.level + 9) / 10) * (Mathf.Abs(defender.level - attacker.level) + 1);
                
                expSlider.gameObject.SetActive(true);
                expSlider.GetComponent<RectTransform>().anchoredPosition = WorldToCanvasSpace(defender.gameObject);
                expSlider.value = defender.exp;
                defender.GainExperience(10);
                StartCoroutine(UpdateEXP(defender));
                return;
            }
        }
    }

    private IEnumerator UpdateEXP(UnitLoader unit)
    {
        yield return new WaitForSeconds(0.25f);
        while(expSlider.value != unit.exp)
        {
            expSlider.value += 1;
            yield return new WaitForSeconds(0.05f);
        }
        if(expSlider.value >= 100)
        {
            unit.LevelUp();
        }
        yield return new WaitForSeconds(0.25f);
        expSlider.gameObject.SetActive(false);        
    }
}
