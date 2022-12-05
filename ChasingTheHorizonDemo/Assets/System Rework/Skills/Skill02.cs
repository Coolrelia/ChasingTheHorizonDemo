using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill02 : Skill
{
    // conditional skill that doubles defense if unit is below 25% HP
    
    private bool skillUsed = false;

    private void Update()
    {
        if (!skillUsed)
        {
            SkillEffect();
        }
    }

    public void SkillEffect(UnitLoader unit)
    {
        int requiredHP = (int)(unit.unitHP * 0.25f);

        if(unit.currentHealth <= requiredHP)
        {
            unit.resBuff  += unit.resBuff * 2;
            skillUsed = true;
        }
    }
}
