using UAssetAPI.PropertyTypes.Objects;

namespace MercuryTools.UndoRedo.Operations;

public class ModifyInt64PropertyDataValue : Operation
{
    public ModifyInt64PropertyDataValue(object? parent, Int64PropertyData int64PropertyData, long oldValue, long newValue)
    {
        Parent = parent;
        Int64PropertyData = int64PropertyData;
        OldValue = oldValue;
        NewValue = newValue;
    }
    
    public readonly Int64PropertyData Int64PropertyData;
    public readonly long OldValue;
    public readonly long NewValue;

    public override void Undo()
    {
        Int64PropertyData.Value = OldValue;
    }

    public override void Redo()
    {
        Int64PropertyData.Value = NewValue;
    }
}