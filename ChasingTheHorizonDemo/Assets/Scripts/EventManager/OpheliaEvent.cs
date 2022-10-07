using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpheliaEvent : Event
{
    [Header("Unique References")]
    [SerializeField] private UnitLoader opheliaObject = null;
    [SerializeField] private GameObject opheliaCG = null;
    [SerializeField] private UnitLoader rolandObject = null;
    [SerializeField] private MapDialogue[] opheliaDialogue = null;
    [SerializeField] private MapDialogue[] opheliaDialogue2 = null;
    [SerializeField] private GameObject thanksForPlaying = null;
    [SerializeField] private GameObject restartButton = null;
    [SerializeField] private AudioClip enterOphelia = null;
    [SerializeField] private UnitLoader royBoss = null;
    private bool eventPlayed = false;
    private string difficulty = string.Empty;

    private void Start()
    {
        difficulty = PlayerPrefs.GetString("Difficulty", "Normal");
    }

    private void Update()
    {
        if(difficulty == "Normal")
        {
            if (TurnManager.instance.enemyUnits.Count <= 0 && eventPlayed == false)
            {
                StartCoroutine(Event());
            }
        }
        else if(difficulty == "Hard")
        {
            if(!TurnManager.instance.enemyUnits.Contains(royBoss) && eventPlayed == false)
            {
                StartCoroutine(Event());
            }
        }
    }

    private IEnumerator Event()
    {
        cursor.cursorControls.DeactivateInput();
        MusicPlayer.instance.source.clip = enterOphelia;
        MusicPlayer.instance.FadeMusic(true);
        cursor.spriteRenderer.color = new Color32(255, 255, 255, 0);
        TurnManager.instance.gameObject.SetActive(false);
        eventPlayed = true;
        loseManager.gameObject.SetActive(false);
        yield return new WaitUntil(() => !screenDim.gameObject.activeSelf);
        yield return new WaitForSeconds(1f);
        //Move Ophelia on screen
        cursor.cursorControls.DeactivateInput();
        opheliaObject.gameObject.SetActive(true);
        StartCoroutine(MoveUnit(opheliaObject, new Vector2(2f, 10f)));
        yield return new WaitUntil(() => opheliaObject.transform.localPosition == new Vector3(2, 10));
        yield return new WaitUntil(() => !screenDim.gameObject.activeSelf);
        MapDialogueManager.instance.ClearDialogue();
        yield return new WaitForSeconds(0.4f);
        //Send dialogue to dialogue manager
        MapDialogueManager.instance.WriteMultiple(opheliaDialogue);
        yield return new WaitUntil(() => !screenDim.gameObject.activeSelf);
        //Ophelia attacks Roland
        yield return new WaitForSeconds(0.5f);
        //Move next to Roland
        StartCoroutine(MoveUnit(opheliaObject, new Vector2(rolandObject.transform.localPosition.x - 1, rolandObject.transform.localPosition.y)));
        yield return new WaitUntil(() => (Vector2)opheliaObject.transform.localPosition == new Vector2(rolandObject.transform.localPosition.x - 1, rolandObject.transform.localPosition.y));
        yield return new WaitForSeconds(0.5f);
        //Attack Roland
        CombatManager.instance.EngageAttack(opheliaObject, rolandObject);
        //Ophelia CG appears and the rest of the dialogue is displayed
        yield return new WaitForSeconds(2f);
        opheliaCG.SetActive(true);
        yield return new WaitForSeconds(1f);
        MapDialogueManager.instance.WriteMultiple(opheliaDialogue2);
        yield return new WaitUntil(() => MapDialogueManager.instance.dialogueFinished);
        thanksForPlaying.SetActive(true);
        yield return new WaitForSeconds(1f);
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        cursor.cursorControls.ActivateInput();
        cursor.SetState(new MenuState(cursor));
        cursor.cursorControls.SwitchCurrentActionMap("UI");
        EventSystem.current.SetSelectedGameObject(restartButton.gameObject);
    }
    private IEnumerator MoveUnit(UnitLoader currentEnemy, Vector2 targetPosition)
    {
        while(currentEnemy.transform.localPosition.x != targetPosition.x)
        {
            if (currentEnemy.transform.localPosition.x > targetPosition.x)
            {
                currentEnemy.animator.SetBool("Left", true);
            }
            else
            {
                currentEnemy.animator.SetBool("Right", true);
            }
            currentEnemy.transform.localPosition = Vector2.MoveTowards(currentEnemy.transform.localPosition, new Vector2(targetPosition.x, currentEnemy.transform.localPosition.y), 2f * Time.deltaTime);
            yield return null;
        }
        while(currentEnemy.transform.localPosition.y != targetPosition.y)
        {
            if (currentEnemy.transform.localPosition.y > targetPosition.y)
            {
                currentEnemy.animator.SetBool("Down", true);
            }
            else
            {
                currentEnemy.animator.SetBool("Up", true);
            }
            currentEnemy.transform.localPosition = Vector2.MoveTowards(currentEnemy.transform.localPosition, new Vector2(currentEnemy.transform.localPosition.x, targetPosition.y), 2f * Time.deltaTime);
            yield return null;
        }
        currentEnemy.animator.SetBool("Right", false);
    }
}
