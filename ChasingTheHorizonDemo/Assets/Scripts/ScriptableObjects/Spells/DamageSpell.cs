using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells/Damage Spell")]
public class DamageSpell : Spell
{
    public Weapon spellAsWeapon;
    public Weapon suicideSpellAsWeapon;

    public bool friendlyFire;
}
