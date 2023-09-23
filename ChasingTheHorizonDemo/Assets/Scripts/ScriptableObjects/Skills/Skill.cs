using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Skill : ScriptableObject
{
    [Tooltip("Most Skills Will Only Have 1 Effect But More Can Be Added.")]
    public List<Effect> effects;
}
