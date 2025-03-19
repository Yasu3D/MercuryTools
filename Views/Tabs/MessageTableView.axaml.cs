using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MercuryTools.UndoRedo;
using MercuryTools.UndoRedo.Operations;
using UAssetAPI;
using UAssetAPI.ExportTypes;
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
        explorerView = Explorer; // a bit of a hacky workaround but.. whatever.
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

    private readonly MainView mainView;
    private readonly ExplorerView explorerView;
    private readonly UndoRedoManager undoRedoManager;
        
    private UAsset? asset;
    private UAsset? assetBackup;
    
    private List<StructPropertyData> table => ((DataTableExport)asset!.Exports[0]).Table.Data;
    private StructPropertyData NewData => new()
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

    private bool ignoreTextChanged;

    private void TreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
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
        if (explorerView.TreeViewElementList?.SelectedItem == null) return;
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
    
    private void MoveElement(ElementMoveDirection direction)
    {
        if (asset == null) return;
        if (explorerView.TreeViewElementList?.SelectedItem == null) return;
        
        try
        {
            // Get Selected Item and connected Data
            TreeViewItem item = (TreeViewItem)explorerView.TreeViewElementList.SelectedItem;
            if (item.Tag is not StructPropertyData dataA) return;
            
            // Get Index
            int indexA = table.IndexOf(dataA);
            int indexB = indexA + (int)direction;
            
            if (indexB < 0) return; // Trying to move first element up, skip.
            if (indexB >= table.Count) return; // Trying to move last element down, skip.
            
            // Switch Data
            StructPropertyData dataB = table[indexB];
            SwapStructProperty operation = new(table, dataA, dataB, indexA, indexB);
            undoRedoManager.InvokeAndPush(operation);
            
            explorerView.RebuildTreeView(table);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
    
    public override void ButtonSave_OnClick(object? sender, RoutedEventArgs args)
    {
        asset?.Write(asset.FilePath);
        assetBackup?.Write($"{assetBackup.FilePath}.bak");
    }

    public override async void ButtonOpen_OnClick(object? sender, RoutedEventArgs args)
    {
        IStorageFile? file = await mainView.OpenUAssetFile();
        if (file == null) return;

        try
        {
            asset = new(file.Path.AbsolutePath, EngineVersion.VER_UE4_19);
            assetBackup = new(file.Path.AbsolutePath, EngineVersion.VER_UE4_19);
            
            explorerView.RebuildTreeView(table);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }

    public override void ButtonUndo_OnClick(object? sender, RoutedEventArgs args)
    {
        undoRedoManager.Undo();
        explorerView.RebuildTreeView(table);
    }

    public override void ButtonRedo_OnClick(object? sender, RoutedEventArgs args)
    {
        undoRedoManager.Redo();
        explorerView.RebuildTreeView(table);
    }

    public override void ButtonMoveElementUp_OnClick(object? sender, RoutedEventArgs args)
    {
        MoveElement(ElementMoveDirection.Up);
    }

    public override void ButtonMoveElementDown_OnClick(object? sender, RoutedEventArgs args)
    {
        MoveElement(ElementMoveDirection.Down);
    }

    public override void ButtonAddElement_OnClick(object? sender, RoutedEventArgs args)
    {
        if (asset == null) return;
        if (explorerView.TreeViewElementList?.SelectedItem == null) return;

        try
        {
            // Create and add new Data
            // Index is table.Count because the item is added at the very end of the list.
            AddStructProperty operation = new(table, NewData, table.Count);
            undoRedoManager.InvokeAndPush(operation);
            
            explorerView.RebuildTreeView(table);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
    
    public override void ButtonDuplicateElement_OnClick(object? sender, RoutedEventArgs args)
    {
        if (asset == null) return;
        if (explorerView.TreeViewElementList?.SelectedItem == null) return;

        try
        {
            TreeViewItem item = (TreeViewItem)explorerView.TreeViewElementList.SelectedItem;
            if (item.Tag is not StructPropertyData data) return;

            StructPropertyData duplicateData = (StructPropertyData)data.Clone();
            
            // Get Index
            DataTableExport export = (DataTableExport)asset.Exports[0];
            int index = export.Table.Data.IndexOf(data);
            
            // Add data to Table
            export.Table.Data.Insert(index, duplicateData);
            
            explorerView.RebuildTreeView(table);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
    
    public override void ButtonDeleteElement_OnClick(object? sender, RoutedEventArgs args)
    {
        if (asset == null) return;
        if (explorerView.TreeViewElementList?.SelectedItem == null) return;

        try
        {
            // Get Selected Item and connected Data
            TreeViewItem item = (TreeViewItem)explorerView.TreeViewElementList.SelectedItem;
            if (item.Tag is not StructPropertyData data) return;
            
            // Remove data from Table
            int index = table.IndexOf(data);
            if (index == -1) return;
            
            RemoveStructProperty operation = new(table, data, index);
            undoRedoManager.InvokeAndPush(operation);

            explorerView.RebuildTreeView(table);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
}