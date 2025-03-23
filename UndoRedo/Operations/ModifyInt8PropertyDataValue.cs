using UAssetAPI.PropertyTypes.Objects;

namespace MercuryTools.UndoRedo.Operations;

public class ModifyInt8PropertyDataValue : Operation
{
    public ModifyInt8PropertyDataValue(object? parent, Int8PropertyData int8PropertyData, sbyte oldValue, sbyte newValue)
    {
        Parent = parent;
        Int8PropertyData = int8PropertyData;
        OldValue = oldValue;
        NewValue = newValue;
    }
    
    public readonly Int8PropertyData Int8PropertyData;
    public readonly sbyte OldValue;
    public readonly sbyte NewValue;

    public override void Undo()
    {
        Int8PropertyData.Value = OldValue;
    }

    public override void Redo()
    {
        Int8PropertyData.Value = NewValue;
    }
}