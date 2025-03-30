using UAssetAPI.PropertyTypes.Objects;

namespace MercuryTools.UndoRedo.Operations;

public class ModifyUInt32PropertyDataValue : Operation
{
    public ModifyUInt32PropertyDataValue(object? parent, UInt32PropertyData uint32PropertyData, uint oldValue, uint newValue)
    {
        Parent = parent;
        UInt32PropertyData = uint32PropertyData;
        OldValue = oldValue;
        NewValue = newValue;
    }
    
    public readonly UInt32PropertyData UInt32PropertyData;
    public readonly uint OldValue;
    public readonly uint NewValue;

    public override void Undo()
    {
        UInt32PropertyData.Value = OldValue;
    }

    public override void Redo()
    {
        UInt32PropertyData.Value = NewValue;
    }
}