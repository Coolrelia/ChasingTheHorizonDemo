﻿using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;
    
    public List<Event> events = new List<Event>();
    public Event currentEvent;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentEvent = events[0];
    }

    public void ActivateEvent()
    {
        currentEvent.StartEvent();
    }
    public bool PlayerEventCheck()
    {
        if(currentEvent.turn == TurnManager.instance.turnNumber && currentEvent.playerTurn){
            return true;
        }
        return false;
    }
    public bool EnemyEventCheck()
    {
        if(currentEvent.turn == TurnManager.instance.turnNumber && currentEvent.enemyTurn){
            return true;
        }
        return false;
    }

}
