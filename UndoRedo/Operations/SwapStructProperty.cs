using System.Collections.Generic;
using UAssetAPI.PropertyTypes.Structs;

namespace MercuryTools.UndoRedo.Operations;

public class SwapStructProperty(List<StructPropertyData> table, StructPropertyData dataA, StructPropertyData dataB, int indexA, int indexB) : IOperation
{
    public readonly List<StructPropertyData> Table = table;
    public readonly StructPropertyData DataA = dataA;
    public readonly StructPropertyData DataB = dataB;
    public readonly int IndexA = indexA;
    public readonly int IndexB = indexB;
    
    public void Undo()
    {
        Table[IndexA] = DataA;
        Table[IndexB] = DataB;
    }

    public void Redo()
    {
        Table[IndexA] = DataB;
        Table[IndexB] = DataA;
    }
}