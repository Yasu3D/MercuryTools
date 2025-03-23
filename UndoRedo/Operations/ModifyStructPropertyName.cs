using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace MercuryTools.UndoRedo.Operations;

public class ModifyStructPropertyName(StructPropertyData structPropertyData, FName oldName, FName newName) : Operation
{
    public readonly StructPropertyData StructPropertyData = structPropertyData;
    public readonly FName OldName = oldName;
    public readonly FName NewName = newName;
    
    public override void Undo()
    {
        StructPropertyData.Name = OldName;
    }

    public override void Redo()
    {
        StructPropertyData.Name = NewName;
    }
}