using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MercuryTools.Views.Tabs;

public partial class ItemUnlockTableView : UserControl
{
    public ItemUnlockTableView()
    {
        InitializeComponent();

        Explorer.ButtonSave.Click += ButtonSave_OnClick;
        Explorer.ButtonOpen.Click += ButtonOpen_OnClick;
        Explorer.ButtonAddElement.Click += ButtonAddElement_OnClick;
        Explorer.ButtonDeleteElement.Click += ButtonDeleteElement_OnClick;
    }

    private void ButtonSave_OnClick(object? sender, RoutedEventArgs args)
    {
        
    }
    
    private void ButtonOpen_OnClick(object? sender, RoutedEventArgs args)
    {
        
    }
    
    private void ButtonAddElement_OnClick(object? sender, RoutedEventArgs args)
    {
        
    }
    
    private void ButtonDeleteElement_OnClick(object? sender, RoutedEventArgs args)
    {
        
    }
}