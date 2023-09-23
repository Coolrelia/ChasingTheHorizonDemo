using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells/Weapon Spell")]
public class WeaponSpell : Spell
{
    public int might;
    public int hit;
    public int crit;
    public int range;
}
