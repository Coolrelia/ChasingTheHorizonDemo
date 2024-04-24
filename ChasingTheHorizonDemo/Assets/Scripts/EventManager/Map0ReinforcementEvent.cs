using System.Collections;
using UnityEngine;

public class Map0ReinforcementEvent : Event
{
    [Header("Unique References")]
    public GameObject reinforcements = null;
    public UnitLoader[] reinforcementUnits = null;

    public override void StartEvent()
    {
        reinforcements.SetActive(true);
        for(int i = 0; i < reinforcementUnits.Length; i++)
        {
            TurnManager.instance.enemyUnits.Add(reinforcementUnits[i]);
        }
        //AIManager.instance.SetEnemyOrder();
        finished = true;
    }
}
