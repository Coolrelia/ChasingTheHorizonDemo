using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : ScriptableObject
{
    public Element element;
    public List<Effect> effects = new List<Effect>();
}
