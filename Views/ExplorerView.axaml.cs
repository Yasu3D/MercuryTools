using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MercuryTools.Views;

public partial class ExplorerView : UserControl
{
    public ExplorerView()
    {
        InitializeComponent();

        ToggleSearch.IsCheckedChanged += ToggleSearch_OnIsCheckedChanged;
    }

    private void ToggleSearch_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        SearchGroup.IsVisible = ToggleSearch.IsChecked ?? false;
    }
}