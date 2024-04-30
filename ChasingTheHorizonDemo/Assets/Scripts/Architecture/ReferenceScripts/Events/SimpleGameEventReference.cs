using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Event/Simple Event")]
public class SimpleGameEventReference : ScriptableObject
{
    public event Action Event;

    public void Trigger()
    {
        Event?.Invoke();
    }
}
