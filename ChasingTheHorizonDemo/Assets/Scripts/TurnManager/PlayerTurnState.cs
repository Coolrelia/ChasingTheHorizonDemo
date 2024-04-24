using System.Collections;
using UnityEngine;
public class PlayerTurnState : TurnState
{
    private bool eventPlayed = false;
    public PlayerTurnState(TurnManager manager):base(manager)
    {
        stateType = StateType.Player;
    }
    public override IEnumerator Begin()
    {
        DisableCursor();

        //Update Turn
        eventPlayed = false;
        turnManager.turnNumber++;
        turnManager.RefreshAllys();
        turnManager.RefreshEnemies();
        turnManager.enemyTurnGraphic.SetActive(false);
        
        //Checks For Events
        if(EventManager.instance.PlayerEventCheck()){
            EventManager.instance.ActivateEvent();
            yield return new WaitForSeconds(2f);
            yield return new WaitUntil(() => EventManager.instance.currentEvent.finished);
            EventManager.instance.ReloadEvent();
            eventPlayed = true;
        }
        //yield return new WaitUntil(() => MapDialogueManager.instance.screenDim.gameObject.activeSelf == false);

        //Resets Camera Position if an event did not occur        
        if(!eventPlayed)
        {
            Vector3 targetPosition = new Vector3(turnManager.cursor.transform.position.x, turnManager.cursor.transform.position.y, -10);
            if(targetPosition.x > turnManager.cursor.cameraRight){
                targetPosition.x = turnManager.cursor.cameraRight;
            }
            if(targetPosition.x < turnManager.cursor.cameraLeft){
                targetPosition.x = turnManager.cursor.cameraLeft;
            }
            if(targetPosition.y > turnManager.cursor.cameraTop){
                targetPosition.y = turnManager.cursor.cameraTop;
            }
            if(targetPosition.y < turnManager.cursor.cameraBottom){
                targetPosition.y = turnManager.cursor.cameraBottom;
            }

            while (turnManager.mainCamera.transform.position.x != targetPosition.x)
            {
                turnManager.mainCamera.transform.position =
                    Vector3.MoveTowards(turnManager.mainCamera.transform.position, new Vector3(targetPosition.x, turnManager.mainCamera.transform.position.y, -10), 5f * Time.deltaTime);
                yield return null;
            }
            while(turnManager.mainCamera.transform.position.y != targetPosition.y)
            {
                turnManager.mainCamera.transform.position =
                    Vector3.MoveTowards(turnManager.mainCamera.transform.position, new Vector3(turnManager.mainCamera.transform.position.x, targetPosition.y, -10), 5f * Time.deltaTime);
                yield return null;
            }
        }

        //Setup for Ally Turn
        turnManager.cursor.CloseMenu();
        turnManager.screenDim.gameObject.SetActive(true);
        turnManager.screenDim.animator.SetTrigger("FadeIn");
        turnManager.allyTurnGraphic.SetActive(true);
        yield return new WaitForSeconds(0.85f);
        turnManager.screenDim.animator.SetTrigger("FadeOut");
        EnableCursor();
        turnManager.cursor.enemyTurn = false;
        turnManager.allyTurn = true;
    }
}
