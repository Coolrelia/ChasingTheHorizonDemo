using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public enum Element { Fire, Ice, Wind, Elec, White, Dark}
    public int might = 0;
    public int hit = 0;
    public int crit = 0;
    public int range = 0;
    public int spellCharges = 0;
    public int levelUnlocked = 0;
    public Element element;
}
