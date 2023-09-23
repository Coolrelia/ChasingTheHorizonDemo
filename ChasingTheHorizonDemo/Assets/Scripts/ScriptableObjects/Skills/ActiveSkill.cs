using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Active Skill", menuName = "Skills/Active Skill")]
public class ActiveSkill : Skill
{
    public int might;
    public int hit;
    public int crit;
    public int range;
}
