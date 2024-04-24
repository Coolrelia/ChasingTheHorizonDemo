public class UnitState : MapState
{
    public UnitState(CursorController cursor):base(cursor)
    {
    }

    public override void Confirm()
    {
        cursorController.MoveUnit();
        //cursorController.AttackMove();
    }

    public override void Cancel()
    {
        cursorController.DeselectUnit();
    }
}
