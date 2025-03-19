using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using UAssetAPI.PropertyTypes.Structs;

namespace MercuryTools.Views;

public enum ElementMoveDirection
{
    Up = -1,
    Down = 1,
}

public partial class ExplorerView : UserControl
{
    public ExplorerView()
    {
        InitializeComponent();

        ToggleSearch.IsCheckedChanged += ToggleSearch_OnIsCheckedChanged;
    }

    public TreeViewItem? SelectedItem => (TreeViewItem?)TreeViewElementList?.SelectedItem;
    
    public void RebuildTreeView(List<StructPropertyData>? tableData)
    {
        if (tableData == null) return;
        if (TreeViewElementList == null) return;

        try
        {
            // Save selected Data
            object? selectedData = null;
            if (TreeViewElementList.SelectedItem is TreeViewItem selectedItem)
            {
                selectedData = selectedItem.Tag;
            }

            // Clear TreeView
            TreeViewElementList.Items.Clear();

            // Loop over Table contents and create a new TreeViewItem for each one.
            foreach (StructPropertyData data in tableData)
            {
                TreeViewItem item = new()
                {
                    Header = data.Name,
                    Tag = data,
                };

                TreeViewElementList.Items.Add(item);
            }
            
            // Reassign selection based on selected data.
            TreeViewElementList.SelectedItem = TreeViewElementList.Items.FirstOrDefault(x => x is TreeViewItem item && item.Tag == selectedData);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
    
    private void ToggleSearch_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        SearchGroup.IsVisible = ToggleSearch.IsChecked ?? false;
    }
}