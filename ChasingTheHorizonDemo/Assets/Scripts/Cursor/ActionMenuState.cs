public class ActionMenuState : CursorState
{
    public ActionMenuState(CursorController cursor):base(cursor)
    {
    }

    public override void Confirm()
    {
        return;
    }

    public override void Cancel()
    {
        if(ActionMenuPlus.instance.inventory.activeSelf) {
            ActionMenuPlus.instance.CloseInventory();
        }
        else {
            cursorController.UndoMove();
        }
    }
}
