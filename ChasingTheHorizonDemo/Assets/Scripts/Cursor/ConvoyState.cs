using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvoyState : CursorState
{
    public ConvoyState(CursorController cursor) : base(cursor)
    {
    }

    public override void Confirm()
    {
        cursorController.ConvoySelect();
    }

    public override void Cancel()
    {
        if(ConvoyTradeMenu.instance.gameObject.transform.GetChild(0).gameObject.activeSelf == false)
        {
            cursorController.cursorControls.SwitchCurrentActionMap("UI");
            cursorController.SetState(new ActionMenuState(ActionMenuPlus.instance.cursor));
        }
        else
        {
            ConvoyTradeMenu.instance.CloseConvoy();
        }
    }
}
