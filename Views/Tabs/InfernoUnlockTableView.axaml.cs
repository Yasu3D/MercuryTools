using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using UAssetAPI;
using UAssetAPI.UnrealTypes;

namespace MercuryTools.Views.Tabs;

public partial class InfernoUnlockTableView : UserControl
{
    public InfernoUnlockTableView(MainView main)
    {
        InitializeComponent();
        mainView = main;

        Explorer.ButtonSave.Click += ButtonSave_OnClick;
        Explorer.ButtonOpen.Click += ButtonOpen_OnClick;
        Explorer.ButtonAddElement.Click += ButtonAddElement_OnClick;
        Explorer.ButtonDuplicateElement.Click += ButtonDuplicateElement_OnClick;
        Explorer.ButtonDeleteElement.Click += ButtonDeleteElement_OnClick;
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
        
    }
    
    private void ButtonDuplicateElement_OnClick(object? sender, RoutedEventArgs args)
    {
        
    }
    
    private void ButtonDeleteElement_OnClick(object? sender, RoutedEventArgs args)
    {
        
    }

    private void BuildAssetList()
    {
        if (asset == null) return;
    }
}