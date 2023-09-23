using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New SpellList", menuName = "Units/SpellList")]
public class Unit_SpellList : SerializedScriptableObject
{
    public Dictionary<int, Spell> spellList = new Dictionary<int, Spell>();
}
