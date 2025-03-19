using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.UnrealTypes;

namespace MercuryTools.UndoRedo.Operations;

public class ModifyStringPropertyDataValue(StrPropertyData strPropertyData, FString oldValue, FString newValue) : IOperation
{
    public readonly StrPropertyData StrPropertyData = strPropertyData;
    public readonly FString OldValue = oldValue;
    public readonly FString NewValue = newValue;
    
    public void Undo()
    {
        StrPropertyData.Value = OldValue;
    }

    public void Redo()
    {
        StrPropertyData.Value = NewValue;
    }
}