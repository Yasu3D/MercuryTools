using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MercuryTools.UndoRedo.Operations;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace MercuryTools.Views.Tabs;

public partial class BossStageTableView : TableTab
{
    public BossStageTableView(MainView main)
    {
        InitializeComponent();
        mainView = main;
        explorerView = Explorer;
        undoRedoManager = new();

        ArrayAppearConditionArray.Content = new ArrayEditor(undoRedoManager, "Appear Condition Array", "StrProperty");
        ArrayUnlockConditionArray.Content = new ArrayEditor(undoRedoManager, "Unlock Condition Array", "StrProperty");

        undoRedoManager.OperationHistoryChanged += UpdateUndoRedoButtons;

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
        
        explorerView.ListBoxElementList.SelectionChanged += ListBox_OnSelectionChanged;
    }

    protected override StructPropertyData NewData => new()
    {
        Name = new(asset, "NO_NAME"),
        StructType = new(asset, "SugorokuStageParameterTableData"),
        Value =
        [
            new Int16PropertyData(new(asset, "MusicId")),
            new BoolPropertyData(new(asset, "bVipPreOpen")),
            new UInt64PropertyData(new(asset, "StartDate")),
            new UInt64PropertyData(new(asset, "EndDate")),
            new ArrayPropertyData(new(asset, "AppearConditionArray")),
            new ArrayPropertyData(new(asset, "UnlockConditionArray")),
        ],
    };

    protected override bool FormatCheck()
    {
        return table.Count != 0 && table[0].Value[0].Name.ToString() == "MusicId" && table[0].Value[4].Name.ToString() == "AppearConditionArray";
    }
    
    protected override bool ContentContainsQuery(StructPropertyData data)
    {
        StringComparison comparison = SearchMatchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        
        // Check Name
        string? name = data.Name.Value?.Value;
        if (Utils.Filter(name, "Name", SearchQuery, comparison)) return true;
        
        // Check Data
        string sugorokuId = ((IntPropertyData)data.Value[0]).Value.ToString();
        if (Utils.Filter(sugorokuId, "SugorokuId", SearchQuery, comparison)) return true;
        
        string? sugorokuStageName = ((StrPropertyData)data.Value[1]).Value?.Value;
        if (Utils.Filter(sugorokuStageName, "SugorokuStageName", SearchQuery, comparison)) return true;
        
        // TODO: ContentContainsQuery
        
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
            IntPropertyData musicId = (IntPropertyData)data.Value[0];
            BoolPropertyData bVipPreOpen = (BoolPropertyData)data.Value[1];
            UInt64PropertyData startDate = (UInt64PropertyData)data.Value[2];
            UInt64PropertyData endDate = (UInt64PropertyData)data.Value[3];
            ArrayPropertyData appearConditionArray = (ArrayPropertyData)data.Value[4];
            ArrayPropertyData unlockConditionArray = (ArrayPropertyData)data.Value[5];
            
            if (ignoreChange) ignoreDataChange = true;
            
            // Set Content to StructPropertyData contents
            ContentGroup.IsVisible = true;
            TextBoxName.Text = data.Name.Value?.Value ?? "0";

            FName dummyName = FName.DefineDummy(asset, "0");
            ((ArrayEditor)ArrayAppearConditionArray.Content!).SetTable(asset, data, appearConditionArray, new StrPropertyData(dummyName)); 
            ((ArrayEditor)ArrayUnlockConditionArray.Content!).SetTable(asset, data, unlockConditionArray, new StrPropertyData(dummyName)); 
            
            TextBoxMusicId.Text = musicId.Value.ToString();
            CheckBoxVipPreOpen.IsChecked = bVipPreOpen.Value;
            TextBoxStartDate.Text = startDate.Value.ToString();
            TextBoxEndDate.Text = endDate.Value.ToString();
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
            ListBoxItem item = explorerView.SelectedItem;
            if (item.Tag is not StructPropertyData data) return;
            
            switch (textBox.Name)
            {
                case "TextBoxName":
                {
                    FName oldName = data.Name;
                    FName newName = new(asset, TextBoxName.Text);

                    ModifyStructPropertyName operation = new(data, oldName, newName);
                    undoRedoManager.RedoAndPush(operation);
                    
                    UpdateListBox(true);
                    break;
                }
                
                case "TextBoxMusicId":
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
                
                case "TextBoxStartDate":
                {
                    UInt64PropertyData uInt64PropertyData = (UInt64PropertyData)data.Value[2];
                    ulong oldValue = uInt64PropertyData.Value;
                    ulong newValue;
                    
                    try
                    {
                        newValue = Convert.ToUInt64(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyUInt64PropertyDataValue operation = new(data, uInt64PropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxEndDate":
                {
                    UInt64PropertyData uInt64PropertyData = (UInt64PropertyData)data.Value[3];
                    ulong oldValue = uInt64PropertyData.Value;
                    ulong newValue;
                    
                    try
                    {
                        newValue = Convert.ToUInt64(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyUInt64PropertyDataValue operation = new(data, uInt64PropertyData, oldValue, newValue);
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
                // StrProperty
                case "TextBoxName":
                {
                    return;
                }
                
                // IntProperty
                case "TextBoxMusicId":
                {
                    _ = Convert.ToInt32(textBox.Text);
                    break;
                }
                    
                // UInt64Property
                case "TextBoxStartDate":
                case "TextBoxEndDate":
                {
                    _ = Convert.ToUInt64(textBox.Text);
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
            ListBoxItem item = explorerView.SelectedItem;
            if (item.Tag is not StructPropertyData data) return;
            
            switch (checkBox.Name)
            {
                case "CheckBoxVipPreOpen":
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
    
    private void ListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs args) => UpdateContent(true);

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