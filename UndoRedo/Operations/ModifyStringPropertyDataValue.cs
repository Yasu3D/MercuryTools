using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.UnrealTypes;

namespace MercuryTools.UndoRedo.Operations;

public class ModifyStringPropertyDataValue : Operation
{
    public ModifyStringPropertyDataValue(object? parent, StrPropertyData strPropertyData, FString oldValue, FString newValue)
    {
        Parent = parent;
        StrPropertyData = strPropertyData;
        OldValue = oldValue;
        NewValue = newValue;
    }
    
    public readonly StrPropertyData StrPropertyData;
    public readonly FString OldValue;
    public readonly FString NewValue;

    public override void Undo()
    {
        StrPropertyData.Value = OldValue;
    }

    public override void Redo()
    {
        StrPropertyData.Value = NewValue;
    }
}