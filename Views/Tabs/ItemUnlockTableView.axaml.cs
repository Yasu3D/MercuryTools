using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MercuryTools.UndoRedo.Operations;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace MercuryTools.Views.Tabs;

public partial class ItemUnlockTableView : TableTab
{
    public ItemUnlockTableView(MainView main)
    {
        InitializeComponent();
        mainView = main;
        explorerView = Explorer;
        undoRedoManager = new();

        ArrayConditionKeys.Content = new ArrayEditor(undoRedoManager, "ConditionKeys", "StrProperty");

        undoRedoManager.OperationHistoryChanged += OnOperationHistoryChanged;

        explorerView.TextBoxSearch.TextChanging += TextBoxSearch_OnTextChanging;
        explorerView.ToggleSearch.IsCheckedChanged += ToggleSearch_OnIsCheckedChanged;
        explorerView.ToggleMatchCase.IsCheckedChanged += ToggleMatchCase_OnIsCheckedChanged;
        explorerView.ToggleInvertQuery.IsCheckedChanged += ToggleInvertQuery_OnIsCheckedChanged;
        
        explorerView.ButtonSave.Click += ButtonSave_OnClick;
        explorerView.ButtonOpen.Click += ButtonOpen_OnClick;
        explorerView.ButtonClose.Click += ButtonClose_OnClick;
        
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
        StructType = new(asset, "TotalResultItemJudgementTableData"),
        Value =
        [
            new IntPropertyData(new(asset, "ItemId")),
            new Int64PropertyData(new(asset, "ConditionGetableStartTime")),
            new Int64PropertyData(new(asset, "ConditionGetableEndTime")),
            new ArrayPropertyData(new(asset, "ConditionKeys")),
        ],
    };

    protected override bool FormatCheck()
    {
        return table.Count != 0 && table[0].Value[0].Name.ToString() == "ItemId" && table[0].Value[3].Name.ToString() == "ConditionKeys";
    }
    
    protected override bool ContentContainsQuery(StructPropertyData data)
    {
        StringComparison comparison = SearchMatchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        
        // Check Name
        string? name = data.Name.Value?.Value;
        if (Utils.Filter(name, "Name", SearchQuery, comparison)) return true;
        
        // Check Data
        string itemId = ((IntPropertyData)data.Value[0]).Value.ToString();
        if (Utils.Filter(itemId, "ItemId", SearchQuery, comparison)) return true;
        
        string conditionGetableStartTime = ((Int64PropertyData)data.Value[1]).Value.ToString();
        if (Utils.Filter(conditionGetableStartTime, "ConditionGetableStartTime", SearchQuery, comparison)) return true;
        
        string conditionGetableEndTime = ((Int64PropertyData)data.Value[2]).Value.ToString();
        if (Utils.Filter(conditionGetableEndTime, "ConditionGetableEndTime", SearchQuery, comparison)) return true;

        ArrayPropertyData conditionKeys = (ArrayPropertyData)data.Value[3];
        if (Utils.FilterArray(conditionKeys, "ConditionKeys", SearchQuery, comparison)) return true;
        
        return false;
    }
    
    protected override void UpdateContent(bool ignoreChange)
    {
        if (asset == null) return;
        
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
            IntPropertyData itemId = (IntPropertyData)data.Value[0];
            Int64PropertyData conditionGetableStartTime = (Int64PropertyData)data.Value[1];
            Int64PropertyData conditionGetableEndTime = (Int64PropertyData)data.Value[2];
            ArrayPropertyData conditionKeys = (ArrayPropertyData)data.Value[3];
            
            if (ignoreChange) ignoreDataChange = true;
            
            // Set Content to StructPropertyData contents
            ContentGroup.IsVisible = true;
            TextBoxName.Text = data.Name.Value?.Value ?? "NO_NAME";

            FName dummyName = FName.DefineDummy(asset, "0");
            ((ArrayEditor)ArrayConditionKeys.Content!).SetTable(asset, data, conditionKeys, new IntPropertyData(dummyName)); 
            
            TextBoxItemId.Text = itemId.Value.ToString(); 
            TextBoxConditionGetableStartTime.Text = conditionGetableStartTime.Value.ToString(); 
            TextBoxConditionGetableEndTime.Text = conditionGetableEndTime.Value.ToString();
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
                
                case "TextBoxItemId": 
                {
                    Int16PropertyData int16PropertyData = (Int16PropertyData)data.Value[0];
                    short oldValue = int16PropertyData.Value;
                    short newValue;
                    
                    try
                    {
                        newValue = Convert.ToInt16(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyInt16PropertyDataValue operation = new(data, int16PropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxConditionGetableStartTime":
                {
                    Int64PropertyData int64PropertyData = (Int64PropertyData)data.Value[1];
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
                
                case "TextBoxConditionGetableEndTime":
                {
                    Int64PropertyData int64PropertyData = (Int64PropertyData)data.Value[2];
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
                {
                    return;
                }
                
                // IntProperty
                case "TextBoxItemId":
                {
                    _ = Convert.ToInt32(textBox.Text);
                    break;
                }
                    
                // Int64Property
                case "TextBoxConditionGetableStartTime":
                case "TextBoxConditionGetableEndTime":
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
    
    private void TreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs args) => UpdateContent(true);

    private void TextBoxSearch_OnTextChanging(object? sender, TextChangingEventArgs args) => SearchContent();
    private void ToggleSearch_OnIsCheckedChanged(object? sender, RoutedEventArgs args) => SearchContent();
    private void ToggleMatchCase_OnIsCheckedChanged(object? sender, RoutedEventArgs args) => SearchContent();
    private void ToggleInvertQuery_OnIsCheckedChanged(object? sender, RoutedEventArgs args) => SearchContent();
    
    private void ButtonSave_OnClick(object? sender, RoutedEventArgs args) => Save();
    private void ButtonOpen_OnClick(object? sender, RoutedEventArgs args) => OpenFilePicker();
    private void ButtonClose_OnClick(object? sender, RoutedEventArgs args) => Close();
    private void ButtonUndo_OnClick(object? sender, RoutedEventArgs args) => Undo();
    private void ButtonRedo_OnClick(object? sender, RoutedEventArgs args) => Redo();
    private void ButtonMoveElementUp_OnClick(object? sender, RoutedEventArgs args) => MoveElement(ElementMoveDirection.Up);
    private void ButtonMoveElementDown_OnClick(object? sender, RoutedEventArgs args) => MoveElement(ElementMoveDirection.Down);
    private void ButtonAddElement_OnClick(object? sender, RoutedEventArgs args) => AddElement();
    private void ButtonDuplicateElement_OnClick(object? sender, RoutedEventArgs args) => DuplicateElement();
    private void ButtonDeleteElement_OnClick(object? sender, RoutedEventArgs args) => DeleteElement();
}