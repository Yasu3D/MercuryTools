using UAssetAPI.PropertyTypes.Objects;

namespace MercuryTools.UndoRedo.Operations;

public class ModifyArrayPropertyData : Operation
{
    public ModifyArrayPropertyData(object? parent, ArrayPropertyData arrayPropertyData, PropertyData[] oldArray, PropertyData[] newArray)
    {
        Parent = parent;
        this.arrayPropertyData = arrayPropertyData;
        this.oldArray = oldArray;
        this.newArray = newArray;
    }
    
    private readonly ArrayPropertyData arrayPropertyData;
    private readonly PropertyData[] oldArray;
    private readonly PropertyData[] newArray;

    public override void Undo()
    {
        arrayPropertyData.Value = oldArray;
    }

    public override void Redo()
    {
        arrayPropertyData.Value = newArray;
    }
}