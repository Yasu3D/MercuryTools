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
    
    public void UpdateTreeView(List<StructPropertyData>? tableData)
    {
        if (tableData == null) return;
        if (TreeViewElementList == null) return;

        try
        {
            // Save selected data.
            object? selectedData = null;
            if (TreeViewElementList.SelectedItem is TreeViewItem selectedItem)
            {
                selectedData = selectedItem.Tag;
            }

            // Update TreeViewItems.
            for (int i = 0; i < tableData.Count; i++)
            {
                if (i < TreeViewElementList.Items.Count)
                {
                    // Modify existing TreeViewItem.
                    if (TreeViewElementList.Items[i] is not TreeViewItem item) continue;
                    
                    item.Header = tableData[i].Name;
                    item.Tag = tableData[i];
                }
                else
                {
                    // Create new TreeViewItem.
                    TreeViewItem item = new()
                    {
                        Header = tableData[i].Name,
                        Tag = tableData[i],
                    };

                    TreeViewElementList.Items.Add(item);
                }
            }
            
            // Delete redundant TreeViewItems.
            for (int i = tableData.Count; i < TreeViewElementList.Items.Count; i++)
            {
                TreeViewElementList.Items.RemoveAt(i);
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