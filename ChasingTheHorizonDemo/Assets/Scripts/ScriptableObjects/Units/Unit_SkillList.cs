using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New SkillList", menuName = "Units/SkillList")]
public class Unit_SkillList : SerializedScriptableObject
{
    public Dictionary<int, Skill> skillList = new Dictionary<int, Skill>();
}
