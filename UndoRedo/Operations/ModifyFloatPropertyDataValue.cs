using UAssetAPI.PropertyTypes.Objects;

namespace MercuryTools.UndoRedo.Operations;

public class ModifyFloatPropertyDataValue(object? parentStruct, FloatPropertyData floatPropertyData, float oldValue, float newValue) : IOperation
{
    public readonly object? ParentStruct = parentStruct;
    public readonly FloatPropertyData FloatPropertyData = floatPropertyData;
    public readonly float OldValue = oldValue;
    public readonly float NewValue = newValue;
    
    public void Undo()
    {
        FloatPropertyData.Value = OldValue;
    }

    public void Redo()
    {
        FloatPropertyData.Value = NewValue;
    }
}