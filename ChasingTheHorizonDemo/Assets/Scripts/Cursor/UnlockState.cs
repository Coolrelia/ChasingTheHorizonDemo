using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockState : CursorState
{
    public UnlockState(CursorController cursor) : base(cursor)
    {
    }

    public override void Confirm()
    {
        cursorController.Unlock();
    }

    public override void Cancel()
    {
        cursorController.cursorControls.SwitchCurrentActionMap("UI");
        cursorController.SetState(new ActionMenuState(cursorController));
    }
}
