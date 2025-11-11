public class SpellState : CursorState
{
    public SpellState(CursorController cursor) : base(cursor)
    {
    }

    public override void Confirm()
    {
        // select target for spell

        cursorController.UseSpell();
    }

    public override void Cancel()
    {
        cursorController.UndoAttack();
    }
}
