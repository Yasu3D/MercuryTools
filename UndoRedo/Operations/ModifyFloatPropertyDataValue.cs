using UAssetAPI.PropertyTypes.Objects;

namespace MercuryTools.UndoRedo.Operations;

public class ModifyFloatPropertyDataValue : Operation
{
    public ModifyFloatPropertyDataValue(object? parent, FloatPropertyData floatPropertyData, float oldValue, float newValue)
    {
        Parent = parent;
        FloatPropertyData = floatPropertyData;
        OldValue = oldValue;
        NewValue = newValue;
    }
    
    public readonly FloatPropertyData FloatPropertyData;
    public readonly float OldValue;
    public readonly float NewValue;

    public override void Undo()
    {
        FloatPropertyData.Value = OldValue;
    }

    public override void Redo()
    {
        FloatPropertyData.Value = NewValue;
    }
}