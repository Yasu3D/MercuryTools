using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MercuryTools.UndoRedo.Operations;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace MercuryTools.Views.Tabs;

public partial class GateStepTableView : TableTab
{
    public GateStepTableView(MainView main)
    {
        InitializeComponent();
        mainView = main;
        explorerView = Explorer;
        undoRedoManager = new();

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
        
        explorerView.TreeViewElementList.SelectionChanged += TreeView_OnSelectionChanged;
    }

    protected override StructPropertyData NewData => new()
    {
        Name = new(asset, "NO_NAME"),
        StructType = new(asset, "SugorokuStageParameterTableData"),
        Value =
        [
            new Int16PropertyData(new(asset, "SugorokuId")),
            new Int16PropertyData(new(asset, "PageNumber")),
            new ArrayPropertyData(new(asset, "TargetPoint")),
            new ArrayPropertyData(new(asset, "GetItemVariety")),
            new ArrayPropertyData(new(asset, "GetItemValue")),
            new ArrayPropertyData(new(asset, "TaskMusic01")),
            new IntPropertyData(new(asset, "TaskGenre01")),
            new ArrayPropertyData(new(asset, "TaskMusic02")),
            new IntPropertyData(new(asset, "TaskGenre02")),
            new ArrayPropertyData(new(asset, "TaskMusic03")),
            new IntPropertyData(new(asset, "TaskGenre03")),
            new IntPropertyData(new(asset, "MissionMusicID")),
            new IntPropertyData(new(asset, "Condition")),
            new IntPropertyData(new(asset, "Difficulty")),
            new IntPropertyData(new(asset, "ClearRate")),
            new IntPropertyData(new(asset, "RewardGatePoint")),
            new Int64PropertyData(new(asset, "EasingDay")),
        ],
    };

    protected override bool FormatCheck()
    {
        return table.Count != 0 && table[0].Value[0].Name.ToString() == "SugorokuID" && table[0].Value[1].Name.ToString() == "PageNumber";
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
        
        return false;
    }
    
    protected override void UpdateContent(bool ignoreChange)
    {
        ContentGroup.IsVisible = true;
        return;
        
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
            Int16PropertyData sugorokuId = (Int16PropertyData)data.Value[0];
            StrPropertyData sugorokuStageName = (StrPropertyData)data.Value[1];
            StrPropertyData sugorokuSelectCenterImage = (StrPropertyData)data.Value[2];
            StrPropertyData sugorokuCenterImage = (StrPropertyData)data.Value[3];
            BoolPropertyData hasLoopPage = (BoolPropertyData)data.Value[4];
            IntPropertyData endContentsStartUserLevel = (IntPropertyData)data.Value[5];
            Int8PropertyData firstPlayBonus = (Int8PropertyData)data.Value[6];
            FloatPropertyData baseScoreNormal = (FloatPropertyData)data.Value[7];
            FloatPropertyData baseScoreVip = (FloatPropertyData)data.Value[8];
            Int16PropertyData misslessBonus = (Int16PropertyData)data.Value[9];
            Int16PropertyData fullComboBonus = (Int16PropertyData)data.Value[10];
            FloatPropertyData multiBonus = (FloatPropertyData)data.Value[11];
            FloatPropertyData taskMusicBonus01 = (FloatPropertyData)data.Value[12];
            FloatPropertyData taskMusicBonus02 = (FloatPropertyData)data.Value[13];
            FloatPropertyData taskMusicBonus03 = (FloatPropertyData)data.Value[14];
            IntPropertyData priority = (IntPropertyData)data.Value[15];

            if (ignoreChange) ignoreDataChange = true;
            
            // Set Content to StructPropertyData contents
            ContentGroup.IsVisible = true;
            TextBoxName.Text = data.Name.Value?.Value ?? "0";

            TextBoxSugorokuId.Text = sugorokuId.Value.ToString();
            TextBoxSugorokuStageName.Text = sugorokuStageName.Value?.Value ?? "";
            TextBoxSugorokuSelectCenterImage.Text = sugorokuSelectCenterImage.Value?.Value ?? "";
            TextBoxSugorokuCenterImage.Text = sugorokuCenterImage.Value?.Value ?? "";
            CheckBoxHasLoopPage.IsChecked = hasLoopPage.Value;
            TextBoxEndContentsStartUserLevel.Text = endContentsStartUserLevel.Value.ToString();
            TextBoxFirstPlayBonus.Text = firstPlayBonus.Value.ToString();
            TextBoxBaseScoreNormal.Text = baseScoreNormal.Value.ToString(CultureInfo.InvariantCulture);
            TextBoxBaseScoreVip.Text = baseScoreVip.Value.ToString(CultureInfo.InvariantCulture);
            TextBoxMisslessBonus.Text = misslessBonus.Value.ToString();
            TextBoxFullComboBonus.Text = fullComboBonus.Value.ToString();
            TextBoxMultiBonus.Text = multiBonus.Value.ToString(CultureInfo.InvariantCulture);
            TextBoxTaskMusicBonus01.Text = taskMusicBonus01.Value.ToString(CultureInfo.InvariantCulture);
            TextBoxTaskMusicBonus02.Text = taskMusicBonus02.Value.ToString(CultureInfo.InvariantCulture);
            TextBoxTaskMusicBonus03.Text = taskMusicBonus03.Value.ToString(CultureInfo.InvariantCulture);
            TextBoxPriority.Text = priority.Value.ToString();
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
        return;
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
                    FName oldName = data.Name;
                    FName newName = new(asset, TextBoxName.Text);

                    ModifyStructPropertyName operation = new(data, oldName, newName);
                    undoRedoManager.RedoAndPush(operation);
                    
                    UpdateTreeView(true);
                    break;
                }
                
                case "TextBoxSugorokuId": 
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
                
                case "TextBoxSugorokuStageName": 
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[1];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxSugorokuSelectCenterImage": 
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[2];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxSugorokuCenterImage": 
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[3];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxEndContentsStartUserLevel": 
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[5];
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
                
                case "TextBoxFirstPlayBonus": 
                {
                    Int8PropertyData int8PropertyData = (Int8PropertyData)data.Value[6];
                    sbyte oldValue = int8PropertyData.Value;
                    sbyte newValue;
                    
                    try
                    {
                        newValue = Convert.ToSByte(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyInt8PropertyDataValue operation = new(data, int8PropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxBaseScoreNormal": 
                {
                    FloatPropertyData intPropertyData = (FloatPropertyData)data.Value[7];
                    float oldValue = intPropertyData.Value;
                    float newValue;
                    
                    try
                    {
                        newValue = Convert.ToSingle(textBox.Text, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyFloatPropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxBaseScoreVip": 
                {
                    FloatPropertyData intPropertyData = (FloatPropertyData)data.Value[8];
                    float oldValue = intPropertyData.Value;
                    float newValue;
                    
                    try
                    {
                        newValue = Convert.ToSingle(textBox.Text, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyFloatPropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxMisslessBonus": 
                {
                    Int16PropertyData int16PropertyData = (Int16PropertyData)data.Value[9];
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
                
                case "TextBoxFullComboBonus": 
                {
                    Int16PropertyData int16PropertyData = (Int16PropertyData)data.Value[10];
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
                
                case "TextBoxMultiBonus": 
                {
                    FloatPropertyData intPropertyData = (FloatPropertyData)data.Value[11];
                    float oldValue = intPropertyData.Value;
                    float newValue;
                    
                    try
                    {
                        newValue = Convert.ToSingle(textBox.Text, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyFloatPropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxTaskMusicBonus01": 
                {
                    FloatPropertyData intPropertyData = (FloatPropertyData)data.Value[12];
                    float oldValue = intPropertyData.Value;
                    float newValue;
                    
                    try
                    {
                        newValue = Convert.ToSingle(textBox.Text, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyFloatPropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxTaskMusicBonus02": 
                {
                    FloatPropertyData intPropertyData = (FloatPropertyData)data.Value[13];
                    float oldValue = intPropertyData.Value;
                    float newValue;
                    
                    try
                    {
                        newValue = Convert.ToSingle(textBox.Text, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyFloatPropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxTaskMusicBonus03": 
                {
                    FloatPropertyData intPropertyData = (FloatPropertyData)data.Value[14];
                    float oldValue = intPropertyData.Value;
                    float newValue;
                    
                    try
                    {
                        newValue = Convert.ToSingle(textBox.Text, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyFloatPropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxPriority": 
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[15];
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
        return;
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
                case "TextBoxSugorokuStageName":
                case "TextBoxSugorokuSelectCenterImage":
                case "TextBoxSugorokuCenterImage":
                {
                    return;
                }

                // FloatProperty
                case "TextBoxBaseScoreNormal":
                case "TextBoxBaseScoreVip":
                case "TextBoxMultiBonus":
                case "TextBoxTaskMusicBonus01":
                case "TextBoxTaskMusicBonus02":
                case "TextBoxTaskMusicBonus03":
                {
                    _ = Convert.ToSingle(textBox.Text, CultureInfo.InvariantCulture);
                    break;
                }
                
                // IntProperty
                case "TextBoxEndContentsStartUserLevel":
                case "TextBoxPriority":
                {
                    _ = Convert.ToInt32(textBox.Text);
                    break;
                }
                    
                // Int8Property
                case "TextBoxFirstPlayBonus":
                {
                    _ = Convert.ToSByte(textBox.Text);
                    break;
                }

                // Int16Property
                case "TextBoxSugorokuId":
                case "TextBoxMisslessBonus":
                case "TextBoxFullComboBonus":
                {
                    _ = Convert.ToInt16(textBox.Text);
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
        return;
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
                case "CheckBoxHasLoopPage":
                {
                    BoolPropertyData boolPropertyData = (BoolPropertyData)data.Value[4];
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

    public override void Save()
    {
        // Data validation
        try
        {
            _ = Convert.ToInt16(TextBoxSugorokuId.Text);
        }
        catch(FormatException)
        {
            TextBoxSugorokuId.Text = "0";
        }
        
        try
        {
            _ = Convert.ToInt32(TextBoxEndContentsStartUserLevel.Text);
        }
        catch(FormatException)
        {
            TextBoxEndContentsStartUserLevel.Text = "0";
        }
        
        try
        {
            _ = Convert.ToSByte(TextBoxFirstPlayBonus.Text);
        }
        catch(FormatException)
        {
            TextBoxFirstPlayBonus.Text = "0";
        }
        
        try
        {
            _ = Convert.ToSingle(TextBoxBaseScoreNormal.Text, CultureInfo.InvariantCulture);
        }
        catch(FormatException)
        {
            TextBoxBaseScoreNormal.Text = "0";
        }
        
        try
        {
            _ = Convert.ToSingle(TextBoxBaseScoreVip.Text, CultureInfo.InvariantCulture);
        }
        catch(FormatException)
        {
            TextBoxBaseScoreVip.Text = "0";
        }
        
        try
        {
            _ = Convert.ToInt16(TextBoxMisslessBonus.Text);
        }
        catch(FormatException)
        {
            TextBoxMisslessBonus.Text = "0";
        }
        
        try
        {
            _ = Convert.ToInt16(TextBoxFullComboBonus.Text);
        }
        catch(FormatException)
        {
            TextBoxFullComboBonus.Text = "0";
        }
        
        try
        {
            _ = Convert.ToSingle(TextBoxMultiBonus.Text, CultureInfo.InvariantCulture);
        }
        catch(FormatException)
        {
            TextBoxMultiBonus.Text = "0";
        }
        
        try
        {
            _ = Convert.ToSingle(TextBoxTaskMusicBonus01.Text, CultureInfo.InvariantCulture);
        }
        catch(FormatException)
        {
            TextBoxTaskMusicBonus01.Text = "0";
        }
        
        try
        {
            _ = Convert.ToSingle(TextBoxTaskMusicBonus02.Text, CultureInfo.InvariantCulture);
        }
        catch(FormatException)
        {
            TextBoxTaskMusicBonus02.Text = "0";
        }
        
        try
        {
            _ = Convert.ToSingle(TextBoxTaskMusicBonus03.Text, CultureInfo.InvariantCulture);
        }
        catch(FormatException)
        {
            TextBoxTaskMusicBonus03.Text = "0";
        }
        
        try
        {
            _ = Convert.ToInt32(TextBoxPriority.Text);
        }
        catch(FormatException)
        {
            TextBoxPriority.Text = "0";
        }
        
        base.Save();
    }
}