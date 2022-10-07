using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "MapUnit")]
public class Unit : ScriptableObject
{
    public Sprite sprite;
    public Sprite portrait;

    public string unitName;
    public bool allyUnit;
    public bool diagonal;

    //Put the statistics
    public Statistics statistics;
    public Growthrates growthRates;
}
