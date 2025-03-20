using UAssetAPI.PropertyTypes.Objects;

namespace MercuryTools.UndoRedo.Operations;

public class ModifyInt8PropertyDataValue(object? parentStruct, Int8PropertyData int8PropertyData, sbyte oldValue, sbyte newValue) : IOperation
{
    public readonly object? ParentStruct = parentStruct;
    public readonly Int8PropertyData Int8PropertyData = int8PropertyData;
    public readonly sbyte OldValue = oldValue;
    public readonly sbyte NewValue = newValue;
    
    public void Undo()
    {
        Int8PropertyData.Value = OldValue;
    }

    public void Redo()
    {
        Int8PropertyData.Value = NewValue;
    }
}