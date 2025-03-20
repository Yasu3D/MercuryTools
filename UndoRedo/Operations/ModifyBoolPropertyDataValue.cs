using UAssetAPI.PropertyTypes.Objects;

namespace MercuryTools.UndoRedo.Operations;

public class ModifyBoolPropertyDataValue(object? parentStruct, BoolPropertyData boolPropertyData, bool oldValue, bool newValue) : IOperation
{
    public readonly object? ParentStruct = parentStruct;
    public readonly BoolPropertyData BoolPropertyData = boolPropertyData;
    public readonly bool OldValue = oldValue;
    public readonly bool NewValue = newValue;
    
    public void Undo()
    {
        BoolPropertyData.Value = OldValue;
    }

    public void Redo()
    {
        BoolPropertyData.Value = NewValue;
    }
}