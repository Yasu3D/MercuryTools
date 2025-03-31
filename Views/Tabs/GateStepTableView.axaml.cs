using System;
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

        ArrayTargetPoint.Content = new ArrayEditor(undoRedoManager, "TargetPoint", "IntProperty");
        ArrayGetItemVariety.Content = new ArrayEditor(undoRedoManager, "GetItemVariety", "StrProperty");
        ArrayGetItemValue.Content = new ArrayEditor(undoRedoManager, "GetItemValue", "IntProperty");
        ArrayTaskMusic01.Content = new ArrayEditor(undoRedoManager, "TaskMusic01", "IntProperty");
        ArrayTaskMusic02.Content = new ArrayEditor(undoRedoManager, "TaskMusic02", "IntProperty");
        ArrayTaskMusic03.Content = new ArrayEditor(undoRedoManager, "TaskMusic03", "IntProperty");

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
        StructType = new(asset, "SugorokuStageParameterTableData"),
        Value =
        [
            new Int16PropertyData(new(asset, "SugorokuID")),
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
        string sugorokuId = ((Int16PropertyData)data.Value[0]).Value.ToString();
        if (Utils.Filter(sugorokuId, "SugorokuId", SearchQuery, comparison)) return true;
        
        string pageNumber = ((Int16PropertyData)data.Value[1]).Value.ToString();
        if (Utils.Filter(pageNumber, "PageNumber", SearchQuery, comparison)) return true;
        
        string taskGenre01 = ((IntPropertyData)data.Value[6]).Value.ToString();
        if (Utils.Filter(taskGenre01, "TaskGenre01", SearchQuery, comparison)) return true;
        
        string taskGenre02 = ((IntPropertyData)data.Value[8]).Value.ToString();
        if (Utils.Filter(taskGenre02, "TaskGenre02", SearchQuery, comparison)) return true;
        
        string taskGenre03 = ((IntPropertyData)data.Value[10]).Value.ToString();
        if (Utils.Filter(taskGenre03, "TaskGenre03", SearchQuery, comparison)) return true;
        
        string missionMusicID = ((IntPropertyData)data.Value[11]).Value.ToString();
        if (Utils.Filter(missionMusicID, "MissionMusicID", SearchQuery, comparison)) return true;
        
        string condition = ((IntPropertyData)data.Value[12]).Value.ToString();
        if (Utils.Filter(condition, "Condition", SearchQuery, comparison)) return true;
        
        string difficulty = ((IntPropertyData)data.Value[13]).Value.ToString();
        if (Utils.Filter(difficulty, "Difficulty", SearchQuery, comparison)) return true;
        
        string clearRate = ((IntPropertyData)data.Value[14]).Value.ToString();
        if (Utils.Filter(clearRate, "ClearRate", SearchQuery, comparison)) return true;
        
        string rewardGatePoint = ((IntPropertyData)data.Value[15]).Value.ToString();
        if (Utils.Filter(rewardGatePoint, "RewardGatePoint", SearchQuery, comparison)) return true;
        
        string easingDay = ((Int64PropertyData)data.Value[16]).Value.ToString();
        if (Utils.Filter(easingDay, "EasingDay", SearchQuery, comparison)) return true;

        ArrayPropertyData targetPoint = (ArrayPropertyData)data.Value[2];
        if (Utils.FilterArray(targetPoint, "TargetPoint", SearchQuery, comparison)) return true;
        
        ArrayPropertyData getItemVariety = (ArrayPropertyData)data.Value[2];
        if (Utils.FilterArray(getItemVariety, "GetItemVariety", SearchQuery, comparison)) return true;
        
        ArrayPropertyData getItemValue = (ArrayPropertyData)data.Value[2];
        if (Utils.FilterArray(getItemValue, "GetItemValue", SearchQuery, comparison)) return true;
        
        ArrayPropertyData taskMusic01 = (ArrayPropertyData)data.Value[2];
        if (Utils.FilterArray(taskMusic01, "TaskMusic01", SearchQuery, comparison)) return true;
        
        ArrayPropertyData taskMusic02 = (ArrayPropertyData)data.Value[2];
        if (Utils.FilterArray(taskMusic02, "TaskMusic02", SearchQuery, comparison)) return true;
        
        ArrayPropertyData taskMusic03 = (ArrayPropertyData)data.Value[2];
        if (Utils.FilterArray(taskMusic03, "TaskMusic03", SearchQuery, comparison)) return true;
        
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
            Int16PropertyData sugorokuId = (Int16PropertyData)data.Value[0];
            Int16PropertyData pageNumber = (Int16PropertyData)data.Value[1];
            ArrayPropertyData targetPoint = (ArrayPropertyData)data.Value[2];
            ArrayPropertyData getItemVariety = (ArrayPropertyData)data.Value[3];
            ArrayPropertyData getItemValue = (ArrayPropertyData)data.Value[4];
            ArrayPropertyData taskMusic01 = (ArrayPropertyData)data.Value[5];
            IntPropertyData taskGenre01 = (IntPropertyData)data.Value[6];
            ArrayPropertyData taskMusic02 = (ArrayPropertyData)data.Value[7];
            IntPropertyData taskGenre02 = (IntPropertyData)data.Value[8];
            ArrayPropertyData taskMusic03 = (ArrayPropertyData)data.Value[9];
            IntPropertyData taskGenre03 = (IntPropertyData)data.Value[10];
            IntPropertyData missionMusicID = (IntPropertyData)data.Value[11];
            IntPropertyData condition = (IntPropertyData)data.Value[12];
            IntPropertyData difficulty = (IntPropertyData)data.Value[13];
            IntPropertyData clearRate = (IntPropertyData)data.Value[14];
            IntPropertyData rewardGatePoint = (IntPropertyData)data.Value[15];
            Int64PropertyData easingDay = (Int64PropertyData)data.Value[16];
            
            if (ignoreChange) ignoreDataChange = true;
            
            // Set Content to StructPropertyData contents
            ContentGroup.IsVisible = true;
            TextBoxName.Text = data.Name.Value?.Value ?? "NO_NAME";

            FName dummyName = FName.DefineDummy(asset, "0");
            ((ArrayEditor)ArrayTargetPoint.Content!).SetTable(asset, data, targetPoint, new IntPropertyData(dummyName)); 
            ((ArrayEditor)ArrayGetItemVariety.Content!).SetTable(asset, data, getItemVariety, new StrPropertyData(dummyName)); 
            ((ArrayEditor)ArrayGetItemValue.Content!).SetTable(asset, data, getItemValue, new IntPropertyData(dummyName)); 
            ((ArrayEditor)ArrayTaskMusic01.Content!).SetTable(asset, data, taskMusic01, new IntPropertyData(dummyName)); 
            ((ArrayEditor)ArrayTaskMusic02.Content!).SetTable(asset, data, taskMusic02, new IntPropertyData(dummyName)); 
            ((ArrayEditor)ArrayTaskMusic03.Content!).SetTable(asset, data, taskMusic03, new IntPropertyData(dummyName)); 
            
            TextBoxSugorokuId.Text = sugorokuId.Value.ToString(); 
            TextBoxPageNumber.Text = pageNumber.Value.ToString(); 
            TextBoxTaskGenre01.Text = taskGenre01.Value.ToString(); 
            TextBoxTaskGenre02.Text = taskGenre02.Value.ToString(); 
            TextBoxTaskGenre03.Text = taskGenre03.Value.ToString(); 
            TextBoxMissionMusicId.Text = missionMusicID.Value.ToString(); 
            TextBoxCondition.Text = condition.Value.ToString(); 
            TextBoxDifficulty.Text = difficulty.Value.ToString(); 
            TextBoxClearRate.Text = clearRate.Value.ToString(); 
            TextBoxRewardGatePoint.Text = rewardGatePoint.Value.ToString(); 
            TextBoxEasingDay.Text = easingDay.Value.ToString(); 
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
                
                case "TextBoxPageNumber":
                {
                    Int16PropertyData int16PropertyData = (Int16PropertyData)data.Value[1];
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
                
                case "TextBoxTaskGenre01":
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[6];
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
                
                case "TextBoxTaskGenre02":
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
                
                case "TextBoxTaskGenre03":
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[10];
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
                
                case "TextBoxMissionMusicId":
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[11];
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
                
                case "TextBoxCondition":
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[12];
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
                
                case "TextBoxDifficulty":
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[13];
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
                
                case "TextBoxClearRate":
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[14];
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
                
                case "TextBoxRewardGatePoint":
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
                
                case "TextBoxEasingDay":
                {
                    Int64PropertyData int64PropertyData = (Int64PropertyData)data.Value[16];
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
                
                // Int16Property
                case "TextBoxSugorokuId":
                case "TextBoxPageNumber":
                {
                    _ = Convert.ToInt16(textBox.Text);
                    break;
                }
                
                // IntProperty
                case "TextBoxTaskGenre01":
                case "TextBoxTaskGenre02":
                case "TextBoxTaskGenre03":
                case "TextBoxMissionMusicId":
                case "TextBoxCondition":
                case "TextBoxDifficulty":
                case "TextBoxClearRate":
                case "TextBoxRewardGatePoint":
                {
                    _ = Convert.ToInt32(textBox.Text);
                    break;
                }
                    
                // Int64Property
                case "TextBoxEasingDay":
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