using System;
using Avalonia.Controls;
using MercuryTools.Views;

namespace MercuryTools;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
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
}