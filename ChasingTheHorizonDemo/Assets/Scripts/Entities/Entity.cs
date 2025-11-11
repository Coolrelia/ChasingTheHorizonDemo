using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enitiy
//think of this script as the base for objects on the map with functionality
//Units, Fire, Ice walls, etc.
public class Entity : MonoBehaviour
{
    public RuntimeList<Entity> entityList;

    public Coordinates coordinates;

    // On enable add itself to the runtime list
    private void OnEnable()
    {
        entityList.Add(this);
        Debug.Log("entity initiated");
    }
    
    //on disable remove from runtime list
    //the children yearn for C++
    private void OnDisable()
    {
        entityList.Remove(this);
    }

    public virtual void EntityDebug()
    {
        // Debug.Log("I am an Entity!");
    }
}
