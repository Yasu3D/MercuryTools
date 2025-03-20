using UAssetAPI.PropertyTypes.Objects;

namespace MercuryTools.UndoRedo.Operations;

public class ModifyInt64PropertyDataValue(object? parentStruct, Int64PropertyData int64PropertyData, long oldValue, long newValue) : IOperation
{
    public readonly object? ParentStruct = parentStruct;
    public readonly Int64PropertyData Int64PropertyData = int64PropertyData;
    public readonly long OldValue = oldValue;
    public readonly long NewValue = newValue;
    
    public void Undo()
    {
        Int64PropertyData.Value = OldValue;
    }

    public void Redo()
    {
        Int64PropertyData.Value = NewValue;
    }
}