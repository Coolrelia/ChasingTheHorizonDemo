using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill03 : Skill
{
    // command skill
    // crit rate becomes 100%
    // select unit to attack
    // after combat take 50% max hp as damage
    // if unit has less than 50% max hp, they enter critical mode

    public void SkillEffect(UnitLoader unit)
    {
        unit.critBuff = 100;
    }
}
