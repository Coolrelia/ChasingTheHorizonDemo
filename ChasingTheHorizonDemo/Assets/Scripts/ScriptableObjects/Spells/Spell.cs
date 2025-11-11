using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : ScriptableObject
{   public enum SpellType { Damage, Healing, Tile, Creation, Manipulation, StatChange, Misc}

    public string spellName;
    public string spellDescription;
    public Sprite spellIcon;
    public Element element;
    public SpellType spellType;
    public int hpCost;
    public int range;
    public List<Effect> effects = new List<Effect>();
}
