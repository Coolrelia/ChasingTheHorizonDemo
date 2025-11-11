using System.Collections;
using UnityEngine;
public class EnemyTurnState : TurnState
{
    public EnemyTurnState(TurnManager manager) : base(manager)
    {
        stateType = StateType.Enemy;
    }

    public override IEnumerator Begin()
    {
        DisableCursor();
        turnManager.cursor.CloseMenu();
        turnManager.allyTurnGraphic.SetActive(false);
        turnManager.map.selectedUnit = null;
        
        //Checks for Events
        if(EventManager.instance.EnemyEventCheck()){
            EventManager.instance.ActivateEvent();
            yield return new WaitUntil(() => EventManager.instance.currentEvent.finished);
            EventManager.instance.ReloadEvent();
        }

        DisableCursor();
        turnManager.enemyTurnGraphic.SetActive(true);
        turnManager.screenDim.gameObject.SetActive(true);
        turnManager.screenDim.animator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(0.85f);
        turnManager.screenDim.animator.SetTrigger("FadeOut");
        AIManager.instance.enemyOrder.Clear();
        //AIManager.instance.SetEnemyOrder();
        turnManager.RefreshEnemies();
        turnManager.cursor.CloseMenu();
        turnManager.RefreshAllySprites();
        DisableCursor();
    }
}
