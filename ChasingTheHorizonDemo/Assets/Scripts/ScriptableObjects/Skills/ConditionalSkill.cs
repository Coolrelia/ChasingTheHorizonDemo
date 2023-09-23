using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Conditional Skill", menuName = "Skills/Conditional Skill")]
public class ConditionalSkill : Skill
{
    public SkillCondition[] skillCondition;
}
