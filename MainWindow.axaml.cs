using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using MercuryTools.Views;

namespace MercuryTools;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        AddHandler(DragDrop.DropEvent, Window_Drop);
    }

    private async void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (!MainView.IsEverythingSaved())
        {
            e.Cancel = true;
            
            bool close = await MainView.ShowChoiceMessage("Warning.", "Not all currently open files have been saved yet.\nDo you really want to close MercuryTools?", "Close MercuryTools", "Cancel");
            
            if (close) Environment.Exit(0);
        }
    }
    
    private void Window_Drop(object? sender, DragEventArgs e)
    {
        Uri? path = e.Data.GetFiles()?.First().Path;
        
        if (path == null || !File.Exists(path.LocalPath) || Path.GetExtension(path.LocalPath) is not (".uasset")) return;
        
        MainView.DragDrop(path.LocalPath);
        e.Handled = true;
    }
}