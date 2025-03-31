using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
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
    
    protected List<StructPropertyData>? table => ((DataTableExport?)asset?.Exports[0])?.Table?.Data;
    protected abstract StructPropertyData NewData { get; }

    protected string SearchQuery => explorerView?.TextBoxSearch.Text ?? "";
    protected bool SearchMatchCase => explorerView?.ToggleMatchCase.IsChecked ?? false;
    
    protected bool ignoreDataChange;

    public FileSaveState fileSaveState { get; private set; } = FileSaveState.NoFile;
    public enum FileSaveState
    {
        NoFile = 0,
        Saved = 1,
        Unsaved = 2,
    }

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
        
    protected void OnOperationHistoryChanged(object? sender, EventArgs args)
    {
        SetSaved(FileSaveState.Unsaved);
        
        if (explorerView == null) return;
        
        explorerView.ButtonUndo.IsEnabled = undoRedoManager?.CanUndo ?? false;
        explorerView.ButtonRedo.IsEnabled = undoRedoManager?.CanRedo ?? false;
    }

    protected void SetSaved(FileSaveState state)
    {
        if (explorerView == null) return;
        
        fileSaveState = state;
        explorerView.TextBlockSaveStatus.Text = state switch
        {
            FileSaveState.NoFile => "No File",
            FileSaveState.Saved => "File Saved",
            FileSaveState.Unsaved => "File Not Saved",
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
        };

        explorerView.TextBlockSaveStatus.Foreground = state switch
        {
            FileSaveState.NoFile => Brushes.Gray,
            FileSaveState.Saved => Brushes.MediumSeaGreen,
            FileSaveState.Unsaved => Brushes.DarkRed,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
        };
    }
    
    public void Save()
    {
        if (asset == null) return;
        
        try
        {
            string uassetPath = asset.FilePath;
            string uassetPathBak = uassetPath + ".bak";

            string uexpPath = Path.ChangeExtension(asset.FilePath, ".uexp");
            string uexpPathBak = uexpPath + ".bak";
            
            if (File.Exists(uassetPath)) File.Copy(uassetPath, uassetPathBak, true);
            if (File.Exists(uexpPath)) File.Copy(uexpPath, uexpPathBak, true);
            
            asset?.Write(uassetPath);
            SetSaved(FileSaveState.Saved);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
    
    public async void OpenFilePicker()
    {
        if (mainView == null) return;
        if (explorerView == null) return;
        if (undoRedoManager == null) return;

        try
        {
            if (fileSaveState == FileSaveState.Unsaved)
            {
                bool open = await MainView.ShowChoiceMessage("Warning.", "The currently open file has not been saved yet.\nDo you really want to open a new file?", "Open New File", "Cancel");
                if (!open) return;
            }
            
            IStorageFile? file = await mainView.OpenUAssetFile();
            if (file == null) return;
            
            await Open(file.Path.LocalPath);
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

    public async void OpenDragDrop(string path)
    {
        if (mainView == null) return;
        if (explorerView == null) return;
        if (undoRedoManager == null) return;

        try
        {
            if (fileSaveState == FileSaveState.Unsaved)
            {
                bool open = await MainView.ShowChoiceMessage("Warning.", "The currently open file has not been saved yet.\nDo you really want to open a new file?", "Open New File", "Cancel");
                if (!open) return;
            }
            
            await Open(path);
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

    public async Task Open(string path)
    {
        if (undoRedoManager == null) return;
        
        asset = new(path, EngineVersion.VER_UE4_19);

        if (!FormatCheck())
        {
            bool open = await MainView.ShowChoiceMessage("Warning.", "The provided .uasset file does not match the expected table format.\nOpening it may cause errors.", "Open Anyways", "Cancel");

            if (!open)
            {
                Close();
                return;
            }
        }

        undoRedoManager.Clear();
        SetSaved(FileSaveState.Saved);
        
        UpdateTreeView(true);
        UpdateContent(true);
        explorerView?.ScrollViewerElementList.ScrollToHome();
        
        if (table?.Count == 0) throw new ArgumentException("Provided .uasset file is empty.");
    }
    
    public async void Close()
    {
        if (undoRedoManager == null) return;
        
        if (fileSaveState == FileSaveState.Unsaved)
        {
            bool open = await MainView.ShowChoiceMessage("Warning.", "The currently open file has not been saved yet.\nDo you really want to open a new file?", "Open New File", "Cancel");
            if (!open) return;
        }

        asset = null;
        
        undoRedoManager.Clear();
        SetSaved(FileSaveState.NoFile);
        
        UpdateTreeView(true);
        UpdateContent(true);
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
        if (table == null) return;
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
        if (table == null) return;
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
        if (table == null) return;
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
        if (table == null) return;
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