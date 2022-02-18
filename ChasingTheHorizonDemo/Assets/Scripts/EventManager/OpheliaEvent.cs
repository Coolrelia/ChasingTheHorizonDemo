﻿using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpheliaEvent : Event
{
    [SerializeField] private GameObject opheliaObject = null;
    [SerializeField] private GameObject screenDim = null;
    [SerializeField] private GameObject opheliaCG = null;
    [SerializeField] private UnitLoader rolandObject = null;
    [SerializeField] private MapDialogue[] opheliaDialogue = null;
    [SerializeField] private MapDialogue[] opheliaDialogue2 = null;
    [SerializeField] private GameObject thanksForPlaying = null;
    [SerializeField] private GameObject restartButton = null;
    private CursorController cursor;
    private LoseManager loseManager;
    private bool eventPlayed = false;

    private void Start()
    {
        cursor = FindObjectOfType<CursorController>();
        loseManager = FindObjectOfType<LoseManager>();
    }
    private void Update()
    {
        if(TurnManager.instance.enemyUnits.Count <= 0 && eventPlayed == false)
        {
            StartCoroutine(Event());
        }        
    }

    private IEnumerator Event()
    {
        eventPlayed = true;
        loseManager.gameObject.SetActive(false);
        //Disable player controls + disable cursor
        cursor.controls.MapScene.Disable();
        cursor.controls.UI.Disable();
        cursor.GetComponent<SpriteRenderer>().sprite = null;
        yield return new WaitUntil(() => !screenDim.activeSelf);
        yield return new WaitForSeconds(1f);
        //Move Ophelia on screen
        opheliaObject.SetActive(true);
        StartCoroutine(MoveUnit(opheliaObject.GetComponent<UnitLoader>(), new Vector2(-6.5f, 0.5f)));
        yield return new WaitForSeconds(2f);
        MapDialogueManager.instance.ClearDialogue();
        yield return new WaitForSeconds(1f);
        //Send dialogue to dialogue manager
        MapDialogueManager.instance.WriteMultiple(opheliaDialogue);
        yield return new WaitUntil(() => !screenDim.activeSelf);
        //Ophelia attacks Roland
        yield return new WaitForSeconds(0.5f);
        //Move next to Roland
        StartCoroutine(MoveUnit(opheliaObject.GetComponent<UnitLoader>(), new Vector2(rolandObject.transform.position.x - 1, rolandObject.transform.position.y)));
        yield return new WaitUntil(() => (Vector2)opheliaObject.transform.position == new Vector2(rolandObject.transform.position.x - 1, rolandObject.transform.position.y));
        yield return new WaitForSeconds(0.5f);
        //Attack Roland
        CombatManager.instance.EngageAttack(opheliaObject.GetComponent<UnitLoader>(), rolandObject);
        //Ophelia CG appears and the rest of the dialogue is displayed
        yield return new WaitForSeconds(3f);
        opheliaCG.SetActive(true);
        yield return new WaitForSeconds(1f);
        MapDialogueManager.instance.WriteMultiple(opheliaDialogue2);
        yield return new WaitUntil(() => MapDialogueManager.instance.dialogueFinished);
        thanksForPlaying.SetActive(true);
        yield return new WaitForSeconds(1f);
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(restartButton.gameObject);
    }
    private IEnumerator MoveUnit(UnitLoader currentEnemy, Vector2 targetPosition)
    {
        while(currentEnemy.transform.position.x != targetPosition.x)
        {
            if (currentEnemy.transform.position.x > targetPosition.x)
            {
                currentEnemy.GetComponent<Animator>().SetBool("Left", true);
            }
            else
            {
                currentEnemy.GetComponent<Animator>().SetBool("Right", true);
            }
            currentEnemy.transform.position = Vector2.MoveTowards(currentEnemy.transform.position, new Vector2(targetPosition.x, currentEnemy.transform.position.y), 2f * Time.deltaTime);
            yield return null;
        }
        while(currentEnemy.transform.position.y != targetPosition.y)
        {
            if (currentEnemy.transform.position.y > targetPosition.y)
            {
                currentEnemy.GetComponent<Animator>().SetBool("Down", true);
            }
            else
            {
                currentEnemy.GetComponent<Animator>().SetBool("Up", true);
            }
            currentEnemy.transform.position = Vector2.MoveTowards(currentEnemy.transform.position, new Vector2(currentEnemy.transform.position.x, targetPosition.y), 2f * Time.deltaTime);
            yield return null;
        }
        currentEnemy.GetComponent<Animator>().SetBool("Right", false);
    }
}