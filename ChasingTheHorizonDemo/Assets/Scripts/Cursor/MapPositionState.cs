using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPositionState : CursorState
{
    public MapPositionState(CursorController cursor) : base(cursor)
    {
    }
    public override void Confirm()
    {
        cursorController.MoveSelectUnit();
    }

    public override void Cancel()
    {
        cursorController.MoveDeselectUnit();
    }
}
