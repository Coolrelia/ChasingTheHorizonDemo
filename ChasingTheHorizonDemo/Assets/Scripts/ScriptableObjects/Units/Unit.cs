using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Units/Unit")]
public class Unit : ScriptableObject
{
    public Sprite sprite;
    public Sprite portrait;

    public string unitName;
    public bool allyUnit;
    public bool diagonal;

    public Statistics statistics; // Stats
    public Growthrates growthRates; // Growths
    public WeaponRanks weaponRanks; // Weapon Ranks
    public Unit_BondList bondList; // Bonds
    public Unit_SkillList skillList; // Skills
    public Unit_SpellList spellList; // Spells
}
