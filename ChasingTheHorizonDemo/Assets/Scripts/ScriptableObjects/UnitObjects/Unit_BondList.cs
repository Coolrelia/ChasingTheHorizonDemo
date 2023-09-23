using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BondList", menuName = "Unit Lists/BondList")]
public class Unit_BondList : ScriptableObject
{    
    public Dictionary<Unit, int> BondList = new Dictionary<Unit, int>();
}
