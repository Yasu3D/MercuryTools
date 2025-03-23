using UAssetAPI.PropertyTypes.Objects;

namespace MercuryTools.UndoRedo.Operations;

public class ModifyBoolPropertyDataValue : Operation
{
    public ModifyBoolPropertyDataValue(object? parent, BoolPropertyData boolPropertyData, bool oldValue, bool newValue)
    {
        Parent = parent;
        BoolPropertyData = boolPropertyData;
        OldValue = oldValue;
        NewValue = newValue;
    }
    
    public readonly BoolPropertyData BoolPropertyData;
    public readonly bool OldValue;
    public readonly bool NewValue;

    public override void Undo()
    {
        BoolPropertyData.Value = OldValue;
    }

    public override void Redo()
    {
        BoolPropertyData.Value = NewValue;
    }
}