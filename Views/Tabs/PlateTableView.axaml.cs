using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MercuryTools.Views.Tabs;

public partial class PlateTableView : UserControl
{
    public PlateTableView()
    {
        InitializeComponent();

        Explorer.ButtonExport.Click += ButtonExport_OnClick;
        Explorer.ButtonImport.Click += ButtonImport_OnClick;
        Explorer.ButtonAddElement.Click += ButtonAddElement_OnClick;
        Explorer.ButtonDeleteElement.Click += ButtonDeleteElement_OnClick;
    }

    private void ButtonExport_OnClick(object? sender, RoutedEventArgs args)
    {
        
    }
    
    private void ButtonImport_OnClick(object? sender, RoutedEventArgs args)
    {
        
    }
    
    private void ButtonAddElement_OnClick(object? sender, RoutedEventArgs args)
    {
        
    }
    
    private void ButtonDeleteElement_OnClick(object? sender, RoutedEventArgs args)
    {
        
    }
}