using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceList<T> : ScriptableObject
{
    public List<T> items = new List<T>();
}
