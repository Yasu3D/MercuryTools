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

    public ListBoxItem? SelectedItem => (ListBoxItem?)ListBoxElementList?.SelectedItem;
    
    public void UpdateListBox(List<StructPropertyData>? tableData)
    {
        if (tableData == null) return;
        if (ListBoxElementList == null) return;

        try
        {
            // Save selected data.
            object? selectedData = null;
            if (ListBoxElementList.SelectedItem is ListBoxItem selectedItem)
            {
                selectedData = selectedItem.Tag;
            }

            // Update ListBoxItems.
            for (int i = 0; i < tableData.Count; i++)
            {
                if (i < ListBoxElementList.Items.Count)
                {
                    // Modify existing ListBoxItem.
                    if (ListBoxElementList.Items[i] is not ListBoxItem item) continue;
                    
                    item.Content = tableData[i].Name;
                    item.Tag = tableData[i];
                }
                else
                {
                    // Create new ListBoxItem.
                    ListBoxItem item = new()
                    {
                        Content = tableData[i].Name,
                        Tag = tableData[i],
                    };

                    ListBoxElementList.Items.Add(item);
                }
            }
            
            // Delete redundant ListBoxItems.
            for (int i = ListBoxElementList.Items.Count - 1; i >= tableData.Count ; i--)
            {
                ListBoxElementList.Items.RemoveAt(i);
            }
            
            // Reassign selection based on selected data.
            ListBoxElementList.SelectedItem = ListBoxElementList.Items.FirstOrDefault(x => x is ListBoxItem item && item.Tag == selectedData);
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