using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public enum SkillType { Passive, Conditional, Active}

    public string skillName;
    public SkillType skillType;

    public void SkillEffect()
    {

    }
}
