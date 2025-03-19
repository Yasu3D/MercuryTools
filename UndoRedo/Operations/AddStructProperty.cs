using System.Collections.Generic;
using UAssetAPI.PropertyTypes.Structs;

namespace MercuryTools.UndoRedo.Operations;

public class AddStructProperty(List<StructPropertyData> table, StructPropertyData data, int index) : IOperation
{
    public readonly List<StructPropertyData> Table = table;
    public readonly StructPropertyData Data = data;
    public readonly int Index = index;
    
    public void Undo()
    {
        Table.Remove(Data);
    }

    public void Redo()
    {
        Table.Insert(Index, Data);
    }
}