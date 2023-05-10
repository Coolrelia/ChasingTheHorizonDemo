﻿using UnityEngine;

public class ConditionalSkill : Skill
{
    protected UnitLoader unit;

    [Header("Change Stats")]
    // increase or reduce stats (use negative numbers for reducing stats if using addition, 0 < number < 1 if multiplication)
    // set values to 0 if unchanged and addtition, 1 if multiplication
    public Statistics stats;

    [Header("Multiply")]
    public bool multiply; // uses multiplication if true, addition if not

    public Statistics GetStats()
    {
        return stats;
    }

    public virtual bool CheckCondition(UnitLoader unit) // will be overriden by subclasses (don't use this class, since the condition isn't specified)
    {
        return false;
    }

    public virtual bool CheckCondition(UnitLoader unit, float randonNumber)
    {
        return false;
    }
}