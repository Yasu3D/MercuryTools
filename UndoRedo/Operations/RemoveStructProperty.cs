using System.Collections.Generic;
using UAssetAPI.PropertyTypes.Structs;

namespace MercuryTools.UndoRedo.Operations;

public class RemoveItem<T>(List<T> table, T data, int index) : Operation
{
    public readonly List<T> Table = table;
    public readonly T Data = data;
    public readonly int Index = index;
    
    public override void Undo()
    {
        Table.Insert(Index, Data);
    }
    
    public override void Redo()
    {
        Table.Remove(Data);
    }
}