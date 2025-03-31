using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MercuryTools.UndoRedo.Operations;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace MercuryTools.Views.Tabs;

public partial class ConditionTableView : TableTab
{
    public ConditionTableView(MainView main)
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
        StructType = new(asset, "ConditionTableData"),
        Value =
        [
            new IntPropertyData(new(asset, "ConditionId")),
            new BoolPropertyData(new(asset, "bConditionLimitNowSeason")),
            new IntPropertyData(new(asset, "ConditionType")),
            new StrPropertyData(new(asset, "Value1")),
            new StrPropertyData(new(asset, "Value2")),
            new StrPropertyData(new(asset, "Value3")),
            new StrPropertyData(new(asset, "Value4")),
            new StrPropertyData(new(asset, "Value5")),
        ],
    };

    protected override bool FormatCheck()
    {
        return table.Count != 0 && table[0].Value[0].Name.ToString() == "ConditionId" && table[0].Value[3].Name.ToString() == "Value1";
    }
    
    protected override bool ContentContainsQuery(StructPropertyData data)
    {
        StringComparison comparison = SearchMatchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        
        // Check Name
        string? name = data.Name.Value?.Value;
        if (Utils.Filter(name, "Name", SearchQuery, comparison)) return true;
        
        // Check Data
        string conditionId = ((IntPropertyData)data.Value[0]).Value.ToString();
        if (Utils.Filter(conditionId, "ConditionId", SearchQuery, comparison)) return true;
        
        string bConditionLimitNowSeason = ((BoolPropertyData)data.Value[1]).Value.ToString();
        if (Utils.Filter(bConditionLimitNowSeason, "bConditionLimitNowSeason", SearchQuery, comparison)) return true;
        
        string conditionType = ((IntPropertyData)data.Value[2]).Value.ToString();
        if (Utils.Filter(conditionType, "ConditionType", SearchQuery, comparison)) return true;
        
        string? value1 = ((StrPropertyData)data.Value[3]).Value?.Value;
        if (Utils.Filter(value1, "Value1", SearchQuery, comparison)) return true;
        
        string? value2 = ((StrPropertyData)data.Value[4]).Value?.Value;
        if (Utils.Filter(value2, "Value2", SearchQuery, comparison)) return true;
        
        string? value3 = ((StrPropertyData)data.Value[5]).Value?.Value;
        if (Utils.Filter(value3, "Value3", SearchQuery, comparison)) return true;
        
        string? value4 = ((StrPropertyData)data.Value[6]).Value?.Value;
        if (Utils.Filter(value4, "Value4", SearchQuery, comparison)) return true;
        
        string? value5 = ((StrPropertyData)data.Value[7]).Value?.Value;
        if (Utils.Filter(value5, "Value5", SearchQuery, comparison)) return true;
        
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
            IntPropertyData conditionId = (IntPropertyData)data.Value[0];
            BoolPropertyData bConditionLimitNowSeason = (BoolPropertyData)data.Value[1];
            IntPropertyData conditionType = (IntPropertyData)data.Value[2];
            StrPropertyData value1 = (StrPropertyData)data.Value[3];
            StrPropertyData value2 = (StrPropertyData)data.Value[4];
            StrPropertyData value3 = (StrPropertyData)data.Value[5];
            StrPropertyData value4 = (StrPropertyData)data.Value[6];
            StrPropertyData value5 = (StrPropertyData)data.Value[7];

            if (ignoreChange) ignoreDataChange = true;
            
            // Set Content to StructPropertyData contents
            ContentGroup.IsVisible = true;
            TextBoxName.Text = data.Name.Value?.Value ?? "NO_NAME";

            TextBoxConditionId.Text = conditionId.Value.ToString();
            CheckBoxConditionLimitNowSeason.IsChecked = bConditionLimitNowSeason.Value;
            ComboBoxConditionType.SelectedIndex = conditionType.Value;
            TextBoxValue1.Text = value1.Value?.Value ?? "";
            TextBoxValue2.Text = value2.Value?.Value ?? "";
            TextBoxValue3.Text = value3.Value?.Value ?? "";
            TextBoxValue4.Text = value4.Value?.Value ?? "";
            TextBoxValue5.Text = value5.Value?.Value ?? "";
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
                
                case "TextBoxConditionId":
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
                
                case "TextBoxConditionType":
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
                
                case "TextBoxValue1":
                { 
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[3];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxValue2":
                { 
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[4];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxValue3":
                { 
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[5];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxValue4":
                { 
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[6];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxValue5":
                { 
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[7];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
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
                case "Value1":
                case "Value2":
                case "Value3":
                case "Value4":
                case "Value5":
                {
                    return;
                }

                // IntProperty
                case "TextBoxConditionId":
                case "TextBoxConditionType":
                {
                    _ = Convert.ToInt32(textBox.Text);
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
                case "CheckBoxConditionLimitNowSeason":
                {
                    BoolPropertyData boolPropertyData = (BoolPropertyData)data.Value[1];
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
    
    private void ComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (ignoreDataChange) return;
        
        if (asset == null) return;
        if (undoRedoManager == null) return;
        if (explorerView?.SelectedItem == null) return;
        if (sender is not ComboBox comboBox) return;
        
        try
        {
            TreeViewItem item = explorerView.SelectedItem;
            if (item.Tag is not StructPropertyData data) return;
            
            switch (comboBox.Name)
            {
                case "ComboBoxConditionType":
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[2];
                    int oldValue = intPropertyData.Value;
                    int newValue = comboBox.SelectedIndex;

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