using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeList<T> : ScriptableObject
{
    public List<T> objects = new List<T>();

    public void Add(T item)
    {
        if (!objects.Contains(item)) objects.Add(item);
    }

    public void Remove(T item)
    {
        if (objects.Contains(item)) objects.Remove(item);
    }

    public void Clear()
    {
        objects.Clear();
    }
}
