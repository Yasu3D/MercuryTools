using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MercuryTools.UndoRedo.Operations;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace MercuryTools.Views.Tabs;

public partial class MessageTableView : TableTab
{
    public MessageTableView(MainView main)
    {
        InitializeComponent();
        mainView = main;
        explorerView = Explorer;
        undoRedoManager = new();
        
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
        StructType = new(asset, "MessageData"),
        Value =
        [
            new StrPropertyData(new(asset, "JapaneseMessage")),
            new StrPropertyData(new(asset, "EnglishMessageUSA")),
            new StrPropertyData(new(asset, "EnglishMessageSG")),
            new StrPropertyData(new(asset, "TraditionalChineseMessageTW")),
            new StrPropertyData(new(asset, "TraditionalChineseMessageHK")),
            new StrPropertyData(new(asset, "SimplifiedChineseMessage")),
            new StrPropertyData(new(asset, "KoreanMessage")),
        ],
    };

    protected override void ReloadContent()
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
            // Get String Properties
            StrPropertyData? japaneseMessage = (StrPropertyData?)data.Value[0];
            StrPropertyData? englishMessageUSA = (StrPropertyData?)data.Value[1];
            StrPropertyData? englishMessageSG = (StrPropertyData?)data.Value[2];
            StrPropertyData? traditionalChineseMessageTW = (StrPropertyData?)data.Value[3];
            StrPropertyData? traditionalChineseMessageHK = (StrPropertyData?)data.Value[4];
            StrPropertyData? simplifiedChineseMessage = (StrPropertyData?)data.Value[5];
            StrPropertyData? koreanMessage = (StrPropertyData?)data.Value[6];

            ignoreDataChange = true;
            
            // Set TextBoxes to StructPropertyData contents
            ContentGroup.IsVisible = true;
            TextBoxName.Text = data.Name.Value.Value;
            
            TextBoxJapaneseMessage.Text = japaneseMessage?.Value?.Value ?? "";
            TextBoxEnglishMessageUSA.Text = englishMessageUSA?.Value?.Value ?? "";
            TextBoxEnglishMessageSG.Text = englishMessageSG?.Value?.Value ?? "";
            TextBoxTraditionalChineseMessageTW.Text = traditionalChineseMessageTW?.Value?.Value ?? "";
            TextBoxTraditionalChineseMessageHK.Text = traditionalChineseMessageHK?.Value?.Value ?? "";
            TextBoxSimplifiedChineseMessage.Text = simplifiedChineseMessage?.Value?.Value ?? "";
            TextBoxKoreanMessage.Text = koreanMessage?.Value?.Value ?? "";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
        finally
        {
            ignoreDataChange = false;
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
                    FName oldName = data.Name;
                    FName newName = new(asset, TextBoxName.Text);

                    ModifyStructPropertyName operation = new(data, oldName, newName);
                    undoRedoManager.InvokeAndPush(operation);
                    break;
                }
                
                case "TextBoxJapaneseMessage":
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[0];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(TextBoxJapaneseMessage.Text);

                    ModifyStringPropertyDataValue operation = new(strPropertyData, oldValue, newValue);
                    undoRedoManager.InvokeAndPush(operation);
                    break;
                }
                
                case "TextBoxEnglishMessageUSA":
                { 
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[1];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(TextBoxEnglishMessageUSA.Text);

                    ModifyStringPropertyDataValue operation = new(strPropertyData, oldValue, newValue);
                    undoRedoManager.InvokeAndPush(operation);
                    break; 
                }
                
                case "TextBoxEnglishMessageSG":
                { 
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[2];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(TextBoxEnglishMessageSG.Text);

                    ModifyStringPropertyDataValue operation = new(strPropertyData, oldValue, newValue);
                    undoRedoManager.InvokeAndPush(operation);
                    break; 
                }
                
                case "TextBoxTraditionalChineseMessageTW":
                { 
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[3];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(TextBoxTraditionalChineseMessageTW.Text);

                    ModifyStringPropertyDataValue operation = new(strPropertyData, oldValue, newValue);
                    undoRedoManager.InvokeAndPush(operation);
                    break; 
                }
                
                case "TextBoxTraditionalChineseMessageHK":
                { 
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[4];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(TextBoxTraditionalChineseMessageHK.Text);

                    ModifyStringPropertyDataValue operation = new(strPropertyData, oldValue, newValue);
                    undoRedoManager.InvokeAndPush(operation);
                    break; 
                }
                
                case "TextBoxSimplifiedChineseMessage":
                { 
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[5];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(TextBoxSimplifiedChineseMessage.Text);

                    ModifyStringPropertyDataValue operation = new(strPropertyData, oldValue, newValue);
                    undoRedoManager.InvokeAndPush(operation);
                    break; 
                }
                
                case "TextBoxKoreanMessage":
                { 
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[6];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(TextBoxKoreanMessage.Text);

                    ModifyStringPropertyDataValue operation = new(strPropertyData, oldValue, newValue);
                    undoRedoManager.InvokeAndPush(operation);
                    break; 
                }
            }

            RebuildTreeView();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
    
    private void TreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs args) => ReloadContent();

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