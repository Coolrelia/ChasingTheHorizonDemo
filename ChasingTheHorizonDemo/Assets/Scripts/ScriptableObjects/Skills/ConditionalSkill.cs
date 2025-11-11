using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Conditional Skill", menuName = "Skills/Conditional Skill")]
public class ConditionalSkill : Skill
{
    public SkillCondition[] skillCondition;

    public bool ConditionMet()
    {
        // loop through all skill conditions and check if they return true
        return false;
    }
}
