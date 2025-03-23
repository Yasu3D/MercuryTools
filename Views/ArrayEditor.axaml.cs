using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MercuryTools.UndoRedo;
using MercuryTools.UndoRedo.Operations;
using UAssetAPI;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace MercuryTools.Views;

public partial class ArrayEditor : UserControl
{
    public ArrayEditor(UndoRedoManager undoRedoManager, string arrayName, string arrayType)
    {
        InitializeComponent();
        this.undoRedoManager = undoRedoManager;
        
        TextBlockArrayName.Text = arrayName;
        TextBlockArrayType.Text = $"ArrayProperty<{arrayType}>";

        ButtonMoveElementDown.Click += ButtonMoveElementDown_OnClick;
        ButtonMoveElementUp.Click += ButtonMoveElementUp_OnClick;
        ButtonAddElement.Click += ButtonAddElement_OnClick;
        ButtonDeleteElement.Click += ButtonDeleteElementOnClick;
    }

    private UAsset? asset;
    private StructPropertyData? parentStructPropertyData;
    
    private ArrayPropertyData? arrayPropertyData;
    private PropertyData? propertyDataTemplate;
    
    private readonly UndoRedoManager undoRedoManager;
    private bool ignoreDataChange;

    public void SetTable(UAsset newAsset, StructPropertyData newStructPropertyData, ArrayPropertyData newArrayPropertyData, PropertyData newPropertyDataTemplate)
    {
        asset = newAsset;
        parentStructPropertyData = newStructPropertyData;
        
        arrayPropertyData = newArrayPropertyData;
        propertyDataTemplate = newPropertyDataTemplate;
        UpdateListBox();
    }
    
    private void UpdateListBox()
    {
        if (asset == null) return;
        if (arrayPropertyData == null) return;
        if (ListBoxElementList == null) return;

        ignoreDataChange = true;

        try
        {
            // Update Table names.
            for (int i = 0; i < arrayPropertyData.Value.Length; i++)
            {
                arrayPropertyData.Value[i].Name = FName.DefineDummy(asset, (i + 1).ToString().Trim());
            }
            
            // Save selected data.
            object? selectedData = null;
            if (ListBoxElementList.SelectedItem is ArrayItem selectedItem)
            {
                selectedData = selectedItem.Tag;
            }

            // Update ListBoxItems.
            for (int i = 0; i < arrayPropertyData.Value.Length; i++)
            {
                if (i < ListBoxElementList.Items.Count)
                {
                    // Modify existing ArrayItem.
                    if (ListBoxElementList.Items[i] is not ArrayItem item) continue;

                    item.TextBlockName.Text = arrayPropertyData.Value[i].Name.ToString();
                    item.TextBoxValue.Text = arrayPropertyData.Value[i].RawValue.ToString();
                    item.Tag = arrayPropertyData.Value[i];
                }
                else
                {
                    // Create new ArrayItem.
                    ArrayItem item = new()
                    {
                        TextBlockName = { Text = arrayPropertyData.Value[i].Name.ToString() },
                        TextBoxValue = { Text = arrayPropertyData.Value[i].RawValue?.ToString() },
                        Tag = arrayPropertyData.Value[i],
                    };

                    item.TextBoxValue.TextChanging += TextBox_OnTextChanging;
                    item.TextBoxValue.LostFocus += TextBox_OnLostFocus;

                    ListBoxElementList.Items.Add(item);
                }
            }

            // Delete redundant ListBoxItems.
            for (int i = ListBoxElementList.Items.Count - 1; i >= arrayPropertyData.Value.Length; i--)
            {
                if (ListBoxElementList.Items[i] is not ArrayItem item) continue;
                
                item.TextBoxValue.TextChanging -= TextBox_OnTextChanging;
                item.TextBoxValue.LostFocus -= TextBox_OnLostFocus;
                ListBoxElementList.Items.Remove(item);
            }

            // Reassign selection based on selected data.
            ListBoxElementList.SelectedItem = ListBoxElementList.Items.FirstOrDefault(x => x is ArrayItem item && item.Tag == selectedData);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
        finally
        {
            ignoreDataChange = false;
        }
    }
    
    private void MoveElement(ElementMoveDirection direction)
    {
        if (arrayPropertyData == null) return;
        if (ListBoxElementList.SelectedItem is not ArrayItem item) return;
        if (item.Tag is not PropertyData dataA) return;
        
        try
        {
            // Get Indices
            int indexA = Array.IndexOf(arrayPropertyData.Value, dataA);
            if (indexA == -1) return;
            
            int indexB = indexA + (int)direction;
            
            if (indexB < 0) return; // Trying to move first element up, skip.
            if (indexB >= arrayPropertyData.Value.Length) return; // Trying to move last element down, skip.
            
            // Create new Arrays
            PropertyData[] oldArray = (PropertyData[])arrayPropertyData.Value.Clone();
            PropertyData[] newArray = (PropertyData[])arrayPropertyData.Value.Clone();

            // Switch Data
            newArray[indexA] = oldArray[indexB];
            newArray[indexB] = oldArray[indexA];

            ModifyArrayPropertyData operation = new(parentStructPropertyData, arrayPropertyData, oldArray, newArray);
            undoRedoManager.RedoAndPush(operation);
            
            UpdateListBox();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
    
    private void AddElement()
    {
        if (arrayPropertyData == null) return;
        if (propertyDataTemplate == null) return;

        try
        {
            // Create new Data
            PropertyData newData = (PropertyData)propertyDataTemplate.Clone();
            
            // Create new Arrays
            PropertyData[] oldArray = (PropertyData[]) arrayPropertyData.Value.Clone();
            PropertyData[] newArray = oldArray.Append(newData).ToArray();

            ModifyArrayPropertyData operation = new(parentStructPropertyData, arrayPropertyData, oldArray, newArray);
            undoRedoManager.RedoAndPush(operation);
            
            UpdateListBox();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
    
    private void DeleteElement()
    {
        if (arrayPropertyData == null) return;
        if (ListBoxElementList.SelectedItem is not ArrayItem item) return;
        if (item.Tag is not PropertyData data) return;

        try
        {
            // Get Index
            int index = Array.IndexOf(arrayPropertyData.Value, data);
            if (index == -1) return;
            
            // Create new Arrays
            PropertyData[] oldArray = (PropertyData[]) arrayPropertyData.Value.Clone();
            PropertyData[] newArray = oldArray.Where((_, i) => i != index).ToArray();

            ModifyArrayPropertyData operation = new(parentStructPropertyData, arrayPropertyData, oldArray, newArray);
            undoRedoManager.RedoAndPush(operation);
            
            UpdateListBox();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }

    private void TextBox_OnTextChanging(object? sender, TextChangingEventArgs args)
    {
        if (ignoreDataChange) return;
        if (sender is not TextBox textBox) return;
        if (textBox.Parent?.Parent is not ArrayItem item) return;
        if (item.Tag is not PropertyData propertyData) return;
        
        if (propertyData is StrPropertyData strPropertyData)
        {
            FString oldValue = strPropertyData.Value;
            FString newValue = new(textBox.Text);

            ModifyStringPropertyDataValue operation = new(parentStructPropertyData, strPropertyData, oldValue, newValue);
            undoRedoManager.RedoAndPush(operation);
            
            return;
        }
        
        if (propertyData is IntPropertyData intPropertyData)
        {
            int oldValue = intPropertyData.Value;
            int newValue;

            try
            {
                newValue = Convert.ToInt16(textBox.Text);
            }
            catch (FormatException)
            {
                newValue = 0;
            }

            ModifyInt32PropertyDataValue operation = new(parentStructPropertyData, intPropertyData, oldValue, newValue);
            undoRedoManager.RedoAndPush(operation);
            
            return;
        }
    }

    private void TextBox_OnLostFocus(object? sender, RoutedEventArgs args)
    {
        if (sender is not TextBox textBox) return;
        
        if (propertyDataTemplate is StrPropertyData)
        {
            return;
        }
        
        if (propertyDataTemplate is IntPropertyData)
        {
            try
            {
                _ = Convert.ToInt32(textBox.Text);
            }
            catch (FormatException)
            {
                textBox.Text = "0";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MainView.ShowWarningMessage("An Error has occurred.", e.Message);
            }
            
            return;
        }
    }
    
    private void ButtonMoveElementDown_OnClick(object? sender, RoutedEventArgs args) => MoveElement(ElementMoveDirection.Down);
    private void ButtonMoveElementUp_OnClick(object? sender, RoutedEventArgs args) => MoveElement(ElementMoveDirection.Up);
    private void ButtonAddElement_OnClick(object? sender, RoutedEventArgs args) => AddElement();
    private void ButtonDeleteElementOnClick(object? sender, RoutedEventArgs args) => DeleteElement();
}