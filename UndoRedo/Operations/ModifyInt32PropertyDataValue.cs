using UAssetAPI.PropertyTypes.Objects;

namespace MercuryTools.UndoRedo.Operations;

public class ModifyInt32PropertyDataValue(object? parentStruct, IntPropertyData intPropertyData, int oldValue, int newValue) : IOperation
{
    public readonly object? ParentStruct = parentStruct;
    public readonly IntPropertyData IntPropertyData = intPropertyData;
    public readonly int OldValue = oldValue;
    public readonly int NewValue = newValue;
    
    public void Undo()
    {
        IntPropertyData.Value = OldValue;
    }

    public void Redo()
    {
        IntPropertyData.Value = NewValue;
    }
}