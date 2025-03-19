using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace MercuryTools.UndoRedo.Operations;

public class ModifyStructPropertyName(StructPropertyData structPropertyData, FName oldName, FName newName) : IOperation
{
    public readonly StructPropertyData StructPropertyData = structPropertyData;
    public readonly FName OldName = oldName;
    public readonly FName NewName = newName;
    
    public void Undo()
    {
        StructPropertyData.Name = OldName;
    }

    public void Redo()
    {
        StructPropertyData.Name = NewName;
    }
}