using System.Collections.Generic;
using UAssetAPI.PropertyTypes.Structs;

namespace MercuryTools.UndoRedo.Operations;

public class RemoveStructProperty(List<StructPropertyData> table, StructPropertyData data, int index) : IOperation
{
    private readonly List<StructPropertyData> Table = table;
    private readonly StructPropertyData Data = data;
    private readonly int Index = index;
    
    public void Undo()
    {
        Table.Insert(Index, Data);
    }
    
    public void Redo()
    {
        Table.Remove(Data);
    }
}