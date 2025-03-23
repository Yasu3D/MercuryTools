namespace MercuryTools.UndoRedo;

public abstract class Operation
{
    public object? Parent;
    public abstract void Undo();
    public abstract void Redo();
}