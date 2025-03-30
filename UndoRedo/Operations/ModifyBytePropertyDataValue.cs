using UAssetAPI.PropertyTypes.Objects;

namespace MercuryTools.UndoRedo.Operations;

public class ModifyBytePropertyDataValue : Operation
{
    public ModifyBytePropertyDataValue(object? parent, BytePropertyData bytePropertyData, byte oldValue, byte newValue)
    {
        Parent = parent;
        BytePropertyData = bytePropertyData;
        OldValue = oldValue;
        NewValue = newValue;
    }
    
    public readonly BytePropertyData BytePropertyData;
    public readonly byte OldValue;
    public readonly byte NewValue;

    public override void Undo()
    {
        BytePropertyData.Value = OldValue;
    }

    public override void Redo()
    {
        BytePropertyData.Value = NewValue;
    }
}