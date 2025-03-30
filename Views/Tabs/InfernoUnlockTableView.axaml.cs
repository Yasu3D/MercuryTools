using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MercuryTools.UndoRedo.Operations;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace MercuryTools.Views.Tabs;

public partial class InfernoUnlockTableView : TableTab
{
    public InfernoUnlockTableView(MainView main)
    {
        InitializeComponent();
        mainView = main;
        explorerView = Explorer;
        undoRedoManager = new();

        undoRedoManager.OperationHistoryChanged += OnOperationHistoryChanged;

        explorerView.TextBoxSearch.TextChanging += TextBoxSearch_OnTextChanging;
        explorerView.ToggleSearch.IsCheckedChanged += ToggleSearch_OnIsCheckedChanged;
        explorerView.ToggleMatchCase.IsCheckedChanged += ToggleMatchCase_OnIsCheckedChanged;
        explorerView.ToggleInvertQuery.IsCheckedChanged += ToggleInvertQuery_OnIsCheckedChanged;
        
        explorerView.ButtonSave.Click += ButtonSave_OnClick;
        explorerView.ButtonOpen.Click += ButtonOpen_OnClick;
        
        explorerView.ButtonUndo.Click += ButtonUndo_OnClick;
        explorerView.ButtonRedo.Click += ButtonRedo_OnClick;
        
        explorerView.ButtonMoveElementUp.Click += ButtonMoveElementUp_OnClick;
        explorerView.ButtonMoveElementDown.Click += ButtonMoveElementDown_OnClick;
        
        explorerView.ButtonAddElement.Click += ButtonAddElement_OnClick;
        explorerView.ButtonDuplicateElement.Click += ButtonDuplicateElement_OnClick;
        explorerView.ButtonDeleteElement.Click += ButtonDeleteElement_OnClick;
        
        explorerView.TreeViewElementList.SelectionChanged += TreeView_OnSelectionChanged;
    }

    protected override StructPropertyData NewData => new()
    {
        Name = new(asset, "NO_NAME"),
        StructType = new(asset, "UnlockInfernoTableData"),
        Value =
        [
            new IntPropertyData(new(asset, "MusicId")),
            new BoolPropertyData(new(asset, "bRequirePurchase")),
            new IntPropertyData(new(asset, "RequiredInfernoOpenWaccaPoint")),
            new BoolPropertyData(new(asset, "bVipPreOpen")),
            new StrPropertyData(new(asset, "NameTag")),
            new StrPropertyData(new(asset, "ExplanationTextTag")),
            new Int64PropertyData(new(asset, "ItemActivateStartTime")),
            new Int64PropertyData(new(asset, "ItemActivateEndTime")),
            new BoolPropertyData(new(asset, "bIsInitItem")),
            new IntPropertyData(new(asset, "GainWaccaPoint")),
        ],
    };

    protected override bool FormatCheck()
    {
        return table.Count != 0 && table[0].Value[0].Name.ToString() == "MusicId" && table[0].Value[2].Name.ToString() == "RequiredInfernoOpenWaccaPoint";
    }
    
    protected override bool ContentContainsQuery(StructPropertyData data)
    {
        StringComparison comparison = SearchMatchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        
        // Check Name
        string? name = data.Name.Value?.Value;
        if (Utils.Filter(name, "Name", SearchQuery, comparison)) return true;
        
        // Check Data
        string musicId = ((IntPropertyData)data.Value[0]).Value.ToString();
        if (Utils.Filter(musicId, "MusicId", SearchQuery, comparison)) return true;
        
        string bRequirePurchase = ((BoolPropertyData)data.Value[1]).Value.ToString();
        if (Utils.Filter(bRequirePurchase, "bRequirePurchase", SearchQuery, comparison)) return true;
        
        string requiredInfernoOpenWaccaPoint = ((IntPropertyData)data.Value[2]).Value.ToString();
        if (Utils.Filter(requiredInfernoOpenWaccaPoint, "RequiredInfernoOpenWaccaPoint", SearchQuery, comparison)) return true;

        string bVipPreOpen = ((BoolPropertyData)data.Value[3]).Value.ToString();
        if (Utils.Filter(bVipPreOpen, "bVipPreOpen", SearchQuery, comparison)) return true;
        
        string? nameTag = ((StrPropertyData)data.Value[4]).Value?.Value;
        if (Utils.Filter(nameTag, "NameTag", SearchQuery, comparison)) return true;
        
        string? explanationTextTag = ((StrPropertyData)data.Value[5]).Value?.Value;
        if (Utils.Filter(explanationTextTag, "ExplanationTextTag", SearchQuery, comparison)) return true;
        
        string itemActivateStartTime = ((Int64PropertyData)data.Value[6]).Value.ToString();
        if (Utils.Filter(itemActivateStartTime, "ItemActivateStartTime", SearchQuery, comparison)) return true;
        
        string itemActivateEndTime = ((Int64PropertyData)data.Value[7]).Value.ToString();
        if (Utils.Filter(itemActivateEndTime, "ItemActivateEndTime", SearchQuery, comparison)) return true;
        
        string bIsInitItem = ((BoolPropertyData)data.Value[8]).Value.ToString();
        if (Utils.Filter(bIsInitItem, "bIsInitItem", SearchQuery, comparison)) return true;
        
        string gainWaccaPoint = ((IntPropertyData)data.Value[9]).Value.ToString();
        if (Utils.Filter(gainWaccaPoint, "GainWaccaPoint", SearchQuery, comparison)) return true;
        
        return false;
    }
    
    protected override void UpdateContent(bool ignoreChange)
    {
        if (explorerView?.SelectedItem == null)
        {
            ContentGroup.IsVisible = false;
            return;
        }

        // Get selected item and connected Data
        if (explorerView.SelectedItem.Tag is not StructPropertyData data)
        {
            ContentGroup.IsVisible = false;
            return;
        }

        try
        {
            // Get Properties
            IntPropertyData musicId = (IntPropertyData)data.Value[0];
            BoolPropertyData bRequirePurchase = (BoolPropertyData)data.Value[1];
            IntPropertyData requiredInfernoOpenWaccaPoint = (IntPropertyData)data.Value[2];
            BoolPropertyData bVipPreOpen = (BoolPropertyData)data.Value[3];
            StrPropertyData nameTag = (StrPropertyData)data.Value[4];
            StrPropertyData explanationTextTag = (StrPropertyData)data.Value[5];
            Int64PropertyData itemActivateStartTime = (Int64PropertyData)data.Value[6];
            Int64PropertyData itemActivateEndTime = (Int64PropertyData)data.Value[7];
            BoolPropertyData bIsInitItem = (BoolPropertyData)data.Value[8];
            IntPropertyData gainWaccaPoint = (IntPropertyData)data.Value[9];

            if (ignoreChange) ignoreDataChange = true;
            
            // Set Content to StructPropertyData contents
            ContentGroup.IsVisible = true;
            TextBoxName.Text = data.Name.Value?.Value ?? "NO_NAME";

            TextBoxMusicId.Text = musicId.Value.ToString();
            CheckBoxRequirePurchase.IsChecked = bRequirePurchase.Value;
            TextBoxRequiredInfernoOpenWaccaPoint.Text = requiredInfernoOpenWaccaPoint.Value.ToString();
            CheckBoxVipPreOpen.IsChecked = bVipPreOpen.Value;
            TextBoxNameTag.Text = nameTag.Value?.Value ?? "";
            TextBoxExplanationTextTag.Text = explanationTextTag.Value?.Value ?? "";
            TextBoxItemActivateStartTime.Text = itemActivateStartTime.Value.ToString();
            TextBoxItemActivateEndTime.Text = itemActivateEndTime.Value.ToString();
            CheckBoxIsInitItem.IsChecked = bIsInitItem.Value;
            TextBoxGainWaccaPoint.Text = gainWaccaPoint.Value.ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
        finally
        {
            if (ignoreChange) ignoreDataChange = false;
        }
    }
    
    private void TextBox_OnTextChanging(object? sender, TextChangingEventArgs args)
    {
        if (ignoreDataChange) return;
        
        if (asset == null) return;
        if (undoRedoManager == null) return;
        if (explorerView?.SelectedItem == null) return;
        if (sender is not TextBox textBox) return;
        
        try
        {
            TreeViewItem item = explorerView.SelectedItem;
            if (item.Tag is not StructPropertyData data) return;
            
            switch (textBox.Name)
            {
                case "TextBoxName":
                {
                    string name = string.IsNullOrEmpty(TextBoxName.Text) ? "NO_NAME" : TextBoxName.Text;
                    
                    FName oldName = data.Name;
                    FName newName = new(asset, name);

                    ModifyStructPropertyName operation = new(data, oldName, newName);
                    undoRedoManager.RedoAndPush(operation);
                    
                    UpdateTreeView(true);
                    break;
                }
                
                case "MusicId":
                { 
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[0];
                    int oldValue = intPropertyData.Value;
                    int newValue;
                    
                    try
                    {
                        newValue = Convert.ToInt32(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyInt32PropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "RequiredInfernoOpenWaccaPoint":
                { 
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[2];
                    int oldValue = intPropertyData.Value;
                    int newValue;
                    
                    try
                    {
                        newValue = Convert.ToInt32(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyInt32PropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxNameTag":
                { 
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[4];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(TextBoxNameTag.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxExplanationTextTag":
                { 
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[5];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(TextBoxExplanationTextTag.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxItemActivateStartTime":
                { 
                    Int64PropertyData int64PropertyData = (Int64PropertyData)data.Value[6];
                    long oldValue = int64PropertyData.Value;
                    long newValue;
                    
                    try
                    {
                        newValue = Convert.ToInt64(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }
                    
                    ModifyInt64PropertyDataValue operation = new(data, int64PropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxItemActivateEndTime":
                {
                    Int64PropertyData int64PropertyData = (Int64PropertyData)data.Value[7];
                    long oldValue = int64PropertyData.Value;
                    long newValue;
                    
                    try
                    {
                        newValue = Convert.ToInt64(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }
                    
                    ModifyInt64PropertyDataValue operation = new(data, int64PropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxGainWaccaPoint":
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[8];
                    int oldValue = intPropertyData.Value;
                    int newValue;
                    
                    try
                    {
                        newValue = Convert.ToInt32(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyInt32PropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }

    private void TextBox_OnLostFocus(object? sender, RoutedEventArgs args)
    {
        if (asset == null) return;
        if (undoRedoManager == null) return;
        if (explorerView?.SelectedItem == null) return;
        if (sender is not TextBox textBox) return;

        try
        {
            switch (textBox.Name)
            {
                // Name
                case "TextBoxName":
                {
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        textBox.Text = "NO_NAME";
                    }

                    break;
                }
                    
                // StrProperty
                case "TextBoxNameTag":
                case "TextBoxExplanationTextTag":
                {
                    return;
                }
                
                // IntProperty
                case "TextBoxMusicId":
                case "TextBoxRequiredInfernoOpenWaccaPoint":
                case "TextBoxGainWaccaPoint":
                {
                    _ = Convert.ToInt32(textBox.Text);
                    break;
                }
                
                // Int64Property
                case "TextBoxItemActivateStartTime":
                case "TextBoxItemActivateEndTime":
                {
                    _ = Convert.ToInt64(textBox.Text);
                    break;
                }
            }
        }
        catch (FormatException)
        {
            textBox.Text = "0";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }

    private void CheckBox_OnIsCheckedChanged(object? sender, RoutedEventArgs args)
    {
        if (ignoreDataChange) return;
        
        if (asset == null) return;
        if (undoRedoManager == null) return;
        if (explorerView?.SelectedItem == null) return;
        if (sender is not CheckBox checkBox) return;
        
        try
        {
            TreeViewItem item = explorerView.SelectedItem;
            if (item.Tag is not StructPropertyData data) return;
            
            switch (checkBox.Name)
            {
                case "CheckBoxRequirePurchase":
                {
                    BoolPropertyData boolPropertyData = (BoolPropertyData)data.Value[1];
                    bool oldValue = boolPropertyData.Value;
                    bool newValue = checkBox.IsChecked ?? false;

                    ModifyBoolPropertyDataValue operation = new(data, boolPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "CheckBoxVipPreOpen":
                {
                    BoolPropertyData boolPropertyData = (BoolPropertyData)data.Value[3];
                    bool oldValue = boolPropertyData.Value;
                    bool newValue = checkBox.IsChecked ?? false;

                    ModifyBoolPropertyDataValue operation = new(data, boolPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "CheckBoxIsInitItem":
                {
                    BoolPropertyData boolPropertyData = (BoolPropertyData)data.Value[8];
                    bool oldValue = boolPropertyData.Value;
                    bool newValue = checkBox.IsChecked ?? false;

                    ModifyBoolPropertyDataValue operation = new(data, boolPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
    
    private void TreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs args) => UpdateContent(true);

    private void TextBoxSearch_OnTextChanging(object? sender, TextChangingEventArgs args) => SearchContent();
    private void ToggleSearch_OnIsCheckedChanged(object? sender, RoutedEventArgs args) => SearchContent();
    private void ToggleMatchCase_OnIsCheckedChanged(object? sender, RoutedEventArgs args) => SearchContent();
    private void ToggleInvertQuery_OnIsCheckedChanged(object? sender, RoutedEventArgs args) => SearchContent();
    
    private void ButtonSave_OnClick(object? sender, RoutedEventArgs args) => Save();
    private void ButtonOpen_OnClick(object? sender, RoutedEventArgs args) => Open();
    private void ButtonUndo_OnClick(object? sender, RoutedEventArgs args) => Undo();
    private void ButtonRedo_OnClick(object? sender, RoutedEventArgs args) => Redo();
    private void ButtonMoveElementUp_OnClick(object? sender, RoutedEventArgs args) => MoveElement(ElementMoveDirection.Up);
    private void ButtonMoveElementDown_OnClick(object? sender, RoutedEventArgs args) => MoveElement(ElementMoveDirection.Down);
    private void ButtonAddElement_OnClick(object? sender, RoutedEventArgs args) => AddElement();
    private void ButtonDuplicateElement_OnClick(object? sender, RoutedEventArgs args) => DuplicateElement();
    private void ButtonDeleteElement_OnClick(object? sender, RoutedEventArgs args) => DeleteElement();
}