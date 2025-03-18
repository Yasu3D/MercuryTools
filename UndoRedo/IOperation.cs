namespace MercuryTools.UndoRedo;

public interface IOperation
{
    void Undo();
    void Redo();
}