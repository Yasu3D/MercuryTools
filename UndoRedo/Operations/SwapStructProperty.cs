using System.Collections.Generic;

namespace MercuryTools.UndoRedo.Operations;

public class SwapItems<T>(List<T> table, T dataA, T dataB, int indexA, int indexB) : Operation
{
    public readonly List<T> Table = table;
    public readonly T DataA = dataA;
    public readonly T DataB = dataB;
    public readonly int IndexA = indexA;
    public readonly int IndexB = indexB;
    
    public override void Undo()
    {
        Table[IndexA] = DataA;
        Table[IndexB] = DataB;
    }

    public override void Redo()
    {
        Table[IndexA] = DataB;
        Table[IndexB] = DataA;
    }
}