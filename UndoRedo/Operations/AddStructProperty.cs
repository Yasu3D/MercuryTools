using System.Collections.Generic;
using UAssetAPI.PropertyTypes.Objects;

namespace MercuryTools.UndoRedo.Operations;

public class AddItem<T>(List<T> table, T data, int index) : Operation
{
    public readonly List<T> Table = table;
    public readonly T Data = data;
    public readonly int Index = index;
    
    public override void Undo()
    {
        Table.Remove(Data);
    }

    public override void Redo()
    {
        Table.Insert(Index, Data);
    }
}