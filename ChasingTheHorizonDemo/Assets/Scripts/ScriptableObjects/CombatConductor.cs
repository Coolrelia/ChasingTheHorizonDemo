using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatConductor : ScriptableObject
{
    private Unit attackUnit;
    private Unit defendUnit;

    // On Combat Start
    // Check for Unit Skills
    // Check for Weapon Skills
    // Check if any unit can attack twice
    // Check if defender can counter attack
    // Perform rolls for both units
    // Calculate damage for both units
    // Check if a unit dies after damage

    public void CombatStart(Unit attacker, Unit defender)
    {
        LoadCombatants(attacker, defender);
    }    

    private void LoadCombatants(Unit attacker, Unit defender)
    {
        attackUnit = null;
        defendUnit = null;

        attackUnit = attacker;
        defendUnit = defender;
    }

    private void SkillCheck(Unit unit)
    {
        // If the unit has a skill that triggers, apply that skill
    }
    private bool HitRoll(Unit unit)
    {
        int roll = Random.Range(0, 99);

        if (roll > unit.CombatStatistics().hit)
            return false;

        else
            return true;
    }
    private bool CritRoll(Unit unit)
    {
        int roll = Random.Range(0, 99);

        if (roll > unit.CombatStatistics().crit)
            return false;

        else            
            return true;
    }
}
