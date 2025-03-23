using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using MercuryTools.UndoRedo;
using MercuryTools.UndoRedo.Operations;
using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace MercuryTools.Views.Tabs;

public abstract class TableTab : UserControl
{
    protected MainView? mainView;
    protected ExplorerView? explorerView;
    protected UndoRedoManager? undoRedoManager;
        
    protected UAsset? asset;
    protected UAsset? assetBackup;
    
    protected List<StructPropertyData> table => ((DataTableExport)asset!.Exports[0]).Table.Data;
    protected abstract StructPropertyData NewData { get; }

    protected string SearchQuery => explorerView?.TextBoxSearch.Text ?? "";
    protected bool SearchMatchCase => explorerView?.ToggleMatchCase.IsChecked ?? false;
    
    protected bool ignoreDataChange;

    protected abstract bool FormatCheck();
    
    protected abstract bool ContentContainsQuery(StructPropertyData data);
    
    protected abstract void UpdateContent(bool ignoreChange);

    protected void UpdateTreeView(bool ignoreChange)
    {
        if (ignoreChange) ignoreDataChange = true;
        
        explorerView?.UpdateTreeView(table);
        SearchContent();
        
        if (ignoreChange) ignoreDataChange = false;
    }

    protected void SearchContent()
    {
        if (explorerView == null) return;
        
        // Set TreeViewItems to default state if not actively searching for anything.
        if (!explorerView.ToggleSearch.IsChecked ?? string.IsNullOrEmpty(SearchQuery))
        {
            foreach (TreeViewItem? item in explorerView.TreeViewElementList.Items)
            {
                if (item == null) continue;
                item.IsVisible = true;
            }

            return;
        }

        // Loop through TreeViewItems to check if they match the query.
        foreach (TreeViewItem? item in explorerView.TreeViewElementList.Items)
        {
            if (item?.Tag is not StructPropertyData data) continue;

            item.IsVisible = ContentContainsQuery(data) ^ explorerView.ToggleInvertQuery.IsChecked ?? false;
        }
    }
        
    protected void UpdateUndoRedoButtons(object? sender, EventArgs args)
    {
        if (explorerView == null) return;
        
        explorerView.ButtonUndo.IsEnabled = undoRedoManager?.CanUndo ?? false;
        explorerView.ButtonRedo.IsEnabled = undoRedoManager?.CanRedo ?? false;
    }
    
    public void Save()
    {
        asset?.Write(asset.FilePath);
        assetBackup?.Write($"{assetBackup.FilePath}.bak");
    }
    
    public async void Open()
    {
        if (mainView == null) return;
        if (explorerView == null) return;
        if (undoRedoManager == null) return;

        try
        {
            IStorageFile? file = await mainView.OpenUAssetFile();
            if (file == null) return;

            undoRedoManager.Clear();

            asset = new(file.Path.AbsolutePath, EngineVersion.VER_UE4_19);
            assetBackup = new(file.Path.AbsolutePath, EngineVersion.VER_UE4_19);

            UpdateTreeView(true);
            UpdateContent(true);

            if (table.Count == 0) throw new ArgumentException("Provided .uasset file is empty.");
            if (!FormatCheck()) throw new FormatException("Provided .uasset file does not follow the correct Table Format.");
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("Warning.", e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
    
    public void Undo()
    {
        if (explorerView == null) return;
        if (undoRedoManager == null) return;
        if (!undoRedoManager.CanUndo) return;

        ignoreDataChange = true;
        Operation operation = undoRedoManager.Undo();

        UpdateTreeView(false);
        UpdateContent(false);
        
        // Try to highlight modified element.
        if (operation.Parent != null)
        {
            explorerView.TreeViewElementList.SelectedItem = explorerView.TreeViewElementList.Items.FirstOrDefault(x => x is TreeViewItem item && item.Tag == operation.Parent);
        }


        ignoreDataChange = false;
    }
    
    public void Redo()
    {
        if (explorerView == null) return;
        if (undoRedoManager == null) return;
        if (!undoRedoManager.CanRedo) return;

        ignoreDataChange = true;
        Operation operation = undoRedoManager.Redo();
        
        UpdateTreeView(false);
        UpdateContent(false);
        
        // Try to highlight modified element.
        if (operation.Parent != null)
        {
            explorerView.TreeViewElementList.SelectedItem = explorerView.TreeViewElementList.Items.FirstOrDefault(x => x is TreeViewItem item && item.Tag == operation.Parent);
        }

        
        ignoreDataChange = false;
    }
    
    public void MoveElement(ElementMoveDirection direction)
    {
        if (asset == null) return;
        if (undoRedoManager == null) return;
        if (explorerView?.SelectedItem == null) return;
        
        try
        {
            // Get Selected Item and connected Data
            TreeViewItem item = explorerView.SelectedItem;
            if (item.Tag is not StructPropertyData dataA) return;
            
            // Get Index
            int indexA = table.IndexOf(dataA);
            if (indexA == -1) return;
            
            int indexB = indexA + (int)direction;
            
            if (indexB < 0) return; // Trying to move first element up, skip.
            if (indexB >= table.Count) return; // Trying to move last element down, skip.
            
            // Switch Data
            StructPropertyData dataB = table[indexB];
            SwapItems<StructPropertyData> operation = new(table, dataA, dataB, indexA, indexB);
            undoRedoManager.RedoAndPush(operation);

            UpdateTreeView(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
    
    public void AddElement()
    {
        if (asset == null) return;
        if (undoRedoManager == null) return;

        try
        {
            // Create and add new Data
            // Index is table.Count because the item is added at the very end of the list.
            AddItem<StructPropertyData> operation = new(table, NewData, table.Count);
            undoRedoManager.RedoAndPush(operation);
            
            UpdateTreeView(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
    
    public void DuplicateElement()
    {
        if (asset == null) return;
        if (undoRedoManager == null) return;
        if (explorerView?.SelectedItem == null) return;

        try
        {
            TreeViewItem item = explorerView.SelectedItem;
            if (item.Tag is not StructPropertyData data) return;

            // Add new Data
            StructPropertyData duplicateData = (StructPropertyData)data.Clone();
            int index = table.IndexOf(data);
            
            AddItem<StructPropertyData> operation = new(table, duplicateData, index);
            undoRedoManager.RedoAndPush(operation);
            
            UpdateTreeView(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
    
    public void DeleteElement()
    {
        if (asset == null) return;
        if (undoRedoManager == null) return;
        if (explorerView?.SelectedItem == null) return;

        try
        {
            // Get Selected Item and connected Data
            TreeViewItem item = explorerView.SelectedItem;
            if (item.Tag is not StructPropertyData data) return;
            
            // Remove data from Table
            int index = table.IndexOf(data);
            if (index == -1) return;
            
            RemoveItem<StructPropertyData> operation = new(table, data, index);
            undoRedoManager.RedoAndPush(operation);

            UpdateTreeView(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
}