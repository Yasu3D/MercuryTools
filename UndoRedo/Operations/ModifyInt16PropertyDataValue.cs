using UAssetAPI.PropertyTypes.Objects;

namespace MercuryTools.UndoRedo.Operations;

public class ModifyInt16PropertyDataValue : Operation
{
    public ModifyInt16PropertyDataValue(object? parent, Int16PropertyData int16PropertyData, short oldValue, short newValue)
    {
        Parent = parent;
        Int16PropertyData = int16PropertyData;
        OldValue = oldValue;
        NewValue = newValue;
    }
    
    public readonly Int16PropertyData Int16PropertyData;
    public readonly short OldValue;
    public readonly short NewValue;

    public override void Undo()
    {
        Int16PropertyData.Value = OldValue;
    }

    public override void Redo()
    {
        Int16PropertyData.Value = NewValue;
    }
}