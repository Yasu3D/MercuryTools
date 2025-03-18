using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MercuryTools.Views.Tabs;

public abstract class Tab : UserControl
{
    public abstract void ButtonSave_OnClick(object? sender, RoutedEventArgs args);
    public abstract void ButtonOpen_OnClick(object? sender, RoutedEventArgs args);
    public abstract void ButtonUndo_OnClick(object? sender, RoutedEventArgs args);
    public abstract void ButtonRedo_OnClick(object? sender, RoutedEventArgs args);
    public abstract void ButtonMoveElementUp_OnClick(object? sender, RoutedEventArgs args);
    public abstract void ButtonMoveElementDown_OnClick(object? sender, RoutedEventArgs args);
    public abstract void ButtonAddElement_OnClick(object? sender, RoutedEventArgs args);
    public abstract void ButtonDuplicateElement_OnClick(object? sender, RoutedEventArgs args);
    public abstract void ButtonDeleteElement_OnClick(object? sender, RoutedEventArgs args);
}