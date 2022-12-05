using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill01 : Skill
{
    // passive skill gives unit +2 attack and +2 defense

    public void SkillEffect(UnitLoader unit)
    {
        unit.strBuff += 2;
        unit.resBuff += 2;
    }
}
