using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "MapUnit")]
public class Unit : ScriptableObject
{
    public enum UnitType { Standard, Flying, Horse, Armored}

    public Sprite sprite;
    public Sprite portrait;

    public string unitName;
    public bool allyUnit;
    public bool diagonal;
    public UnitType unitType;

    //Put the statistics
    public Statistics statistics;
    public Growthrates growthRates;

    public List<Spell> spellList1 = new List<Spell>();
    public List<Spell> spellList2 = new List<Spell>();
}
