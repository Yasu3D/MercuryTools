using UAssetAPI.PropertyTypes.Objects;

namespace MercuryTools.UndoRedo.Operations;

public class ModifyInt16PropertyDataValue(object? parentStruct, Int16PropertyData int16PropertyData, short oldValue, short newValue) : IOperation
{
    public readonly object? ParentStruct = parentStruct;
    public readonly Int16PropertyData Int16PropertyData = int16PropertyData;
    public readonly short OldValue = oldValue;
    public readonly short NewValue = newValue;
    
    public void Undo()
    {
        Int16PropertyData.Value = OldValue;
    }

    public void Redo()
    {
        Int16PropertyData.Value = NewValue;
    }
}