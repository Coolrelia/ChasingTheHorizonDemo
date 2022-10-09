using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapConditions : MonoBehaviour
{
    public static MapConditions instance;
    public enum WinCondition { Seize, Escape, Route, Hold}
    public enum LoseCondition { All, Any, Specific}
    
    public WinCondition wincon;
    public LoseCondition losecon;
    public int minimumDeployableUnits;
    public int maximumDeployableUnits;

    private void Awake()
    {
        instance = this;
    }
}
