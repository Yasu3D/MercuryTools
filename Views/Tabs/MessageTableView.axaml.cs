using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace MercuryTools.Views.Tabs;

public partial class MessageTableView : UserControl
{
    public MessageTableView(MainView main)
    {
        InitializeComponent();
        mainView = main;
        
        Explorer.ButtonSave.Click += ButtonSave_OnClick;
        Explorer.ButtonOpen.Click += ButtonOpen_OnClick;
        Explorer.ButtonAddElement.Click += ButtonAddElement_OnClick;
        Explorer.ButtonDuplicateElement.Click += ButtonDuplicateElement_OnClick;
        Explorer.ButtonDeleteElement.Click += ButtonDeleteElement_OnClick;
        Explorer.TreeViewElementList.SelectionChanged += TreeView_OnSelectionChanged;
    }

    private readonly MainView mainView;
    private UAsset? asset;
    
    private void ButtonSave_OnClick(object? sender, RoutedEventArgs args) => asset?.Write(asset.FilePath);

    private async void ButtonOpen_OnClick(object? sender, RoutedEventArgs args)
    {
        IStorageFile? file = await mainView.OpenUAssetFile();
        if (file == null) return;

        try
        {
            asset = new(file.Path.AbsolutePath, EngineVersion.VER_UE4_19);
            BuildAssetList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            mainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
    
    private void ButtonAddElement_OnClick(object? sender, RoutedEventArgs args)
    {
        if (asset == null) return;
        if (Explorer.TreeViewElementList?.SelectedItem == null) return;

        try
        {
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            mainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
    
    private void ButtonDuplicateElement_OnClick(object? sender, RoutedEventArgs args)
    {
        if (asset == null) return;
        if (Explorer.TreeViewElementList?.SelectedItem == null) return;

        try
        {
            TreeViewItem item = (TreeViewItem)Explorer.TreeViewElementList.SelectedItem;
            if (item.Tag is not StructPropertyData data) return;

            StructPropertyData duplicateData = (StructPropertyData)data.Clone();
            TreeViewItem duplicateItem = new()
            {
                Header = duplicateData.Name,
                Tag = duplicateData,
            };

            // Add item to TreeView
            Explorer.TreeViewElementList.Items.Add(duplicateItem);
            
            // Add data to Table
            DataTableExport export = (DataTableExport)asset.Exports[0];
            export.Table.Data.Add(duplicateData);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            mainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
    
    private void ButtonDeleteElement_OnClick(object? sender, RoutedEventArgs args)
    {
        if (asset == null) return;
        if (Explorer.TreeViewElementList?.SelectedItem == null) return;

        try
        {
            // Get Selected Item and connected Data
            TreeViewItem item = (TreeViewItem)Explorer.TreeViewElementList.SelectedItem;
            if (item.Tag is not StructPropertyData data) return;

            // Remove item from TreeView
            Explorer.TreeViewElementList.Items.Remove(item);
            
            // Remove data from Table
            DataTableExport export = (DataTableExport)asset.Exports[0];
            export.Table.Data.Remove(data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            mainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }

    private void BuildAssetList()
    {
        if (asset == null) return;

        // Clear TreeView
        Explorer.TreeViewElementList.Items.Clear();
        
        // Loop over Table contents and create a new TreeViewItem for each one.
        DataTableExport export = (DataTableExport)asset.Exports[0];
        foreach (StructPropertyData data in export.Table.Data)
        {
            // Crude format check. This code makes A LOT of assumptions :P
            if (data.Value[0].Name.ToString() != "JapaneseMessage")
            {
                throw new FormatException("Provided .uasset file is not a valid Message Table.");
            }
            
            TreeViewItem item = new()
            {
                Header = data.Name,
                Tag = data,
            };
            
            Explorer.TreeViewElementList.Items.Add(item);
        }
    }

    private void TreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        // Get Selected Item
        if (Explorer?.TreeViewElementList?.SelectedItem is not TreeViewItem item)
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
            mainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
    
    private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs args)
    {
        if (asset == null) return;
        if (Explorer.TreeViewElementList?.SelectedItem == null) return;
        if (sender is not TextBox textBox) return;
        
        try
        {
            TreeViewItem item = (TreeViewItem)Explorer.TreeViewElementList.SelectedItem;
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
            mainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
}