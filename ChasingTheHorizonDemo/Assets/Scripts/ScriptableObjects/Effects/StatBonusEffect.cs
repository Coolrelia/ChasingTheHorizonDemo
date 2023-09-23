using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect", menuName = "Effects/StatBonus")]
public class StatBonusEffect : Effect
{
    public int attack;
    public int protection;
    public int mitigation;
    public int attackSpeed;
    public int evasion;
    public int accuracy;
    public int critical;
    public int vigilance;

    public override void ActivateEffect(MonoBehaviour caster)
    {
        // add the values of all the bonus stats to the unit that casted the ability;
    }
}
