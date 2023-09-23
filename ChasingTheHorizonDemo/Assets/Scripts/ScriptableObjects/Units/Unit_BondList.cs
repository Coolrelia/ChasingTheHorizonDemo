using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New BondList", menuName = "Units/BondList")]
public class Unit_BondList : SerializedScriptableObject
{
    public Dictionary<Unit, int> bondList = new Dictionary<Unit, int>();
}
