using UAssetAPI.PropertyTypes.Objects;

namespace MercuryTools.UndoRedo.Operations;

public class ModifyUInt64PropertyDataValue : Operation
{
    public ModifyUInt64PropertyDataValue(object? parent, UInt64PropertyData uInt64PropertyData, ulong oldValue, ulong newValue)
    {
        Parent = parent;
        UInt64PropertyData = uInt64PropertyData;
        OldValue = oldValue;
        NewValue = newValue;
    }
    
    public readonly UInt64PropertyData UInt64PropertyData;
    public readonly ulong OldValue;
    public readonly ulong NewValue;

    public override void Undo()
    {
        UInt64PropertyData.Value = OldValue;
    }

    public override void Redo()
    {
        UInt64PropertyData.Value = NewValue;
    }
}