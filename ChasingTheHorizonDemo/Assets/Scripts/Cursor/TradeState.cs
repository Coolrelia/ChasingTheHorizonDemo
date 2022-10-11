using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeState : CursorState
{
    public TradeState(CursorController cursor) : base(cursor)
    {
    }

    public override void Confirm()
    {
        cursorController.TradeSelectUnit();
    }

    public override void Cancel()
    {
        TradeMenu.instance.CloseTrade();
    }

}
