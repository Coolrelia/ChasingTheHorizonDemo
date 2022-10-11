using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseSystem : MonoBehaviour
{
    public enum Phase { PlayerPhase, EnemyPhase, AlliedPhase}
    // Event Check - Checks for events
    // Begin Step - Checks for skills that trigger at the beginning of the turn
    // Action Step - Either allows the player to move their units or starts the enemy ai
    // End Step - Checks for skills that trigger at the end of the turn
    // Spawn Step - Checks for reinforcments that spawn at the beginning or end of the turn


    public Phase currentPhase;

    private void PlayerPhase()
    {
        // set current phase to player phase
        EventCheck(true, currentPhase);
        SkillCheck(true, currentPhase);
        PlayerAction();
        SkillCheck(false, currentPhase);
        EventCheck(false, currentPhase);
    }

    private void EnemyPhase()
    {
        // set current phase to enemy phase
        EventCheck(true, currentPhase);
        SkillCheck(true, currentPhase);
        SpawnCheck(true, currentPhase);
        EnemyAction();
        SkillCheck(false, currentPhase);
        SpawnCheck(false, currentPhase);
        EventCheck(false, currentPhase);
    }

    private void AlliedPhase()
    {
        SkillCheck(true, currentPhase);
        SpawnCheck(true, currentPhase);
        AlliedAction();
        SkillCheck(false, currentPhase);
    }

    private void EventCheck(bool step1, Phase phase)
    {
        if(step1 && phase == Phase.PlayerPhase)
        {
            // check for events that trigger at the beginning of the player phase
        }
        else if(step1 && phase == Phase.EnemyPhase)
        {
            // check for events that trigger at the beginning of the enemy phase
        }

        if (!step1 && phase == Phase.PlayerPhase)
        {
            // check for events that trigger at the end of the player phase
        }
        else if (!step1 && phase == Phase.EnemyPhase)
        {
            // check for events that trigger at the end of the enemy phase
        }
    }
    private void SkillCheck(bool step1, Phase phase)
    {
        if (step1 && phase == Phase.PlayerPhase)
        {
            // check for skills that trigger at the beginning of the player phase
        }
        else if (step1 && phase == Phase.EnemyPhase)
        {
            // check for skills that trigger at the beginning of the enemy phase
        }

        if (!step1 && phase == Phase.PlayerPhase)
        {
            // check for skills that trigger at the end of the player phase
        }
        else if (!step1 && phase == Phase.EnemyPhase)
        {
            // check for skills that trigger at the end of the enemy phase
        }
    }

    private void PlayerAction()
    {
        // allows the player to take their turn
    }
    private void EnemyAction()
    {
        // starts the enemy ai
    }
    private void AlliedAction()
    {
        // the allied ai starts
    }

    private void SpawnCheck(bool step1, Phase phase)
    {
        if (step1 && phase == Phase.PlayerPhase)
        {
            // check for reinforcement that trigger at the beginning of the player phase
        }
        else if (step1 && phase == Phase.EnemyPhase)
        {
            // check for reinforcement that trigger at the beginning of the enemy phase
        }

        if (!step1 && phase == Phase.PlayerPhase)
        {
            // check for reinforcement that trigger at the end of the player phase
        }
        else if (!step1 && phase == Phase.EnemyPhase)
        {
            // check for reinforcement that trigger at the end of the enemy phase
        }
    }
}
