using System.Collections.Generic;
using UAssetAPI.PropertyTypes.Structs;

namespace MercuryTools.UndoRedo.Operations;

public class SwapStructProperty(List<StructPropertyData> table, StructPropertyData dataA, StructPropertyData dataB, int indexA, int indexB) : IOperation
{
    private readonly List<StructPropertyData> Table = table;
    private readonly StructPropertyData DataA = dataA;
    private readonly StructPropertyData DataB = dataB;
    private readonly int IndexA = indexA;
    private readonly int IndexB = indexB;
    
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