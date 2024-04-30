using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Event/Unit Interaction Event")]
public class UnitInteractionEventReference : ScriptableObject
{
    public event EventHandler<UnitInteractionEventReferenceArgs> Event;

    public class UnitInteractionEventReferenceArgs : EventArgs 
    {
        public Unit initiator;
        public Unit target;

        public UnitInteractionEventReferenceArgs(Unit initiatorUnit, Unit targetUnit)
        {
            initiator = initiatorUnit;
            target = targetUnit;
        }
    }

    public void Trigger(Unit initiator, Unit target)
    {
        Event?.Invoke(this, new UnitInteractionEventReferenceArgs(initiator, target));
    }
}
