using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;

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

    private bool ignoreTextChanged;

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

    private void TreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (explorerView == null) return;
        
        // Get Selected Item
        if (explorerView.TreeViewElementList?.SelectedItem is not TreeViewItem item)
        {
            ContentGroup.IsVisible = false;
            return;
        }

        // Get connected Data
        if (item.Tag is not StructPropertyData data)
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
            
            ContentGroup.IsVisible = true;
            TextBoxName.Text = data.Name.Value.Value;
            
            // Set TextBoxes to StringProperty contents
            ignoreTextChanged = true;
            
            TextBoxJapaneseMessage.Text = japaneseMessage?.Value?.Value ?? "";
            TextBoxEnglishMessageUSA.Text = englishMessageUSA?.Value?.Value ?? "";
            TextBoxEnglishMessageSG.Text = englishMessageSG?.Value?.Value ?? "";
            TextBoxTraditionalChineseMessageTW.Text = traditionalChineseMessageTW?.Value?.Value ?? "";
            TextBoxTraditionalChineseMessageHK.Text = traditionalChineseMessageHK?.Value?.Value ?? "";
            TextBoxSimplifiedChineseMessage.Text = simplifiedChineseMessage?.Value?.Value ?? "";
            TextBoxKoreanMessage.Text = koreanMessage?.Value?.Value ?? "";

            ignoreTextChanged = false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
    
    private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs args)
    {
        if (asset == null) return;
        if (ignoreTextChanged) return;
        if (explorerView?.TreeViewElementList?.SelectedItem == null) return;
        if (sender is not TextBox textBox) return;
        
        try
        {
            TreeViewItem item = (TreeViewItem)explorerView.TreeViewElementList.SelectedItem;
            if (item.Tag is not StructPropertyData data) return;
            
            switch (textBox.Name)
            {
                case "TextBoxName":
                {
                    data.Name = new(asset, Utils.InputOrDefault(TextBoxName.Text));
                    item.Header = TextBoxName.Text;
                    break;
                }
                
                case "TextBoxJapaneseMessage":
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[0];
                    strPropertyData.Value = Utils.InputOrDefault(TextBoxJapaneseMessage.Text);
                    break; 
                }
                
                case "TextBoxEnglishMessageUSA":
                { 
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[1];
                    strPropertyData.Value = Utils.InputOrDefault(TextBoxEnglishMessageUSA.Text);
                    break; 
                }
                
                case "TextBoxEnglishMessageSG":
                { 
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[2];
                    strPropertyData.Value = Utils.InputOrDefault(TextBoxEnglishMessageSG.Text);
                    break; 
                }
                
                case "TextBoxTraditionalChineseMessageTW":
                { 
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[3];
                    strPropertyData.Value = Utils.InputOrDefault(TextBoxTraditionalChineseMessageTW.Text);
                    break; 
                }
                
                case "TextBoxTraditionalChineseMessageHK":
                { 
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[4];
                    strPropertyData.Value = Utils.InputOrDefault(TextBoxTraditionalChineseMessageHK.Text);
                    break; 
                }
                
                case "TextBoxSimplifiedChineseMessage":
                { 
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[5];
                    strPropertyData.Value = Utils.InputOrDefault(TextBoxSimplifiedChineseMessage.Text);
                    break; 
                }
                
                case "TextBoxKoreanMessage":
                { 
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[6];
                    strPropertyData.Value = Utils.InputOrDefault(TextBoxKoreanMessage.Text);
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