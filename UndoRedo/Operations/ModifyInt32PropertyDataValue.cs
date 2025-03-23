using UAssetAPI.PropertyTypes.Objects;

namespace MercuryTools.UndoRedo.Operations;

public class ModifyInt32PropertyDataValue : Operation
{
    public ModifyInt32PropertyDataValue(object? parent, IntPropertyData intPropertyData, int oldValue, int newValue)
    {
        Parent = parent;
        IntPropertyData = intPropertyData;
        OldValue = oldValue;
        NewValue = newValue;
    }
    
    public readonly IntPropertyData IntPropertyData;
    public readonly int OldValue;
    public readonly int NewValue;

    public override void Undo()
    {
        IntPropertyData.Value = OldValue;
    }

    public override void Redo()
    {
        IntPropertyData.Value = NewValue;
    }
}