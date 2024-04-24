public class CombatPreviewState : CursorState
{
    public CombatPreviewState(CursorController cursor) : base(cursor)
    {
    }

    public override void Confirm()
    {
        cursorController.SpellAttack();
    }

    public override void Cancel()
    {
        cursorController.CancelAttack();
    }
}
