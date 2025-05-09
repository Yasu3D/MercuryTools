using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using FluentAvalonia.UI.Controls;
using MercuryTools.Views.Tabs;

namespace MercuryTools.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        iconTableView = new(this);
        plateTableView = new(this);
        gradeTableView = new(this);
        gradePartsTableView = new(this);
        musicParameterTableView = new(this);
        bossStageTableView = new(this);
        infernoUnlockTableView = new(this);
        musicUnlockTableView = new(this);
        itemUnlockTableView = new(this);
        conditionTableView = new(this);
        gateTableView = new(this);
        gateStepTableView = new(this);
        messageTableView = new(this);

        ViewContainer.Content = iconTableView;
        TextBlockVersion.Text = VersionMessage;
        
        KeyDownEvent.AddClassHandler<TopLevel>(OnKeyDown, RoutingStrategies.Tunnel, handledEventsToo: true);
        KeyUpEvent.AddClassHandler<TopLevel>(OnKeyUp, RoutingStrategies.Tunnel, handledEventsToo: true);
    }

    private const string VersionMessage = "MercuryTools v0.0.2 by yasu3d";

    private readonly IconTableView iconTableView;
    private readonly PlateTableView plateTableView;
    private readonly GradeTableView gradeTableView;
    private readonly GradePartsTableView gradePartsTableView;
    private readonly MusicParameterTableView musicParameterTableView;
    private readonly BossStageTableView bossStageTableView;
    private readonly InfernoUnlockTableView infernoUnlockTableView;
    private readonly MusicUnlockTableView musicUnlockTableView;
    private readonly ItemUnlockTableView itemUnlockTableView;
    private readonly ConditionTableView conditionTableView;
    private readonly GateTableView gateTableView;
    private readonly GateStepTableView gateStepTableView;
    private readonly MessageTableView messageTableView;

    private void AssetTypeTabs_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (ViewContainer == null) return;
        if (sender is not RadioButton selected) return;
        if (selected.IsChecked == false) return;

        ViewContainer.Content = selected.Name switch
        {
            "TabIconTable"           => iconTableView,
            "TabPlateTable"          => plateTableView,
            "TabGradeTable"          => gradeTableView,
            "TabGradePartsTable"     => gradePartsTableView,
            "TabMusicParameterTable" => musicParameterTableView,
            "TabBossStageTable"      => bossStageTableView,
            "TabInfernoUnlockTable"  => infernoUnlockTableView,
            "TabMusicUnlockTable"    => musicUnlockTableView,
            "TabItemUnlockTable"     => itemUnlockTableView,
            "TabConditionTable"      => conditionTableView,
            "TabGateTable"           => gateTableView,
            "TabGateStepTable"       => gateStepTableView,
            "TabMessageTable"        => messageTableView,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    private IStorageProvider GetStorageProvider()
    {
        if (VisualRoot is TopLevel top) return top.StorageProvider;
        throw new("Oops!");
    }

    private void OnKeyDown(object sender, KeyEventArgs args)
    {
        if (args.Key is Key.LeftAlt or Key.RightAlt or Key.LeftShift or Key.RightShift or Key.LeftCtrl or Key.RightCtrl)
        {
            args.Handled = true;
            return;
        }
        
        // Very very janky.
        if (ViewContainer.Content is not TableTab tab) return;
        
        if (args.Key is Key.Z && args.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            tab.Undo();
        }
        
        if (args.Key is Key.Y && args.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            tab.Redo();
        }
        
        if (args.Key is Key.S && args.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            tab.Save();
        }
        
        if (TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement() is TextBox) return;

        if (args.Key is Key.F4 && args.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            tab.Close();
        }
        
        if (args.Key is Key.O && args.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            tab.OpenFilePicker();
        }
        
        if (args.Key is Key.Insert)
        {
            tab.AddElement();
        }
        
        if (args.Key is Key.D && args.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            tab.DuplicateElement();
        }
        
        if (args.Key is Key.Delete)
        {
            tab.DeleteElement();
        }
        
        if (args.Key is Key.Up && args.KeyModifiers.HasFlag(KeyModifiers.Alt))
        {
            tab.MoveElement(ElementMoveDirection.Up);
        }
        
        if (args.Key is Key.Down && args.KeyModifiers.HasFlag(KeyModifiers.Alt))
        {
            tab.MoveElement(ElementMoveDirection.Down);
        }
    }
    
    private static void OnKeyUp(object sender, KeyEventArgs e) => e.Handled = true;

    public static void ShowWarningMessage(string title, string? content = null)
    {
        ContentDialog dialog = new()
        {
            Title = title,
            Content = content,
            PrimaryButtonText = "Ok",
        };

        dialog.ShowAsync();
    }

    public static async Task<bool> ShowChoiceMessage(string title, string content, string primary, string secondary)
    {
        ContentDialog dialog = new()
        {
            Title = title,
            Content = content,
            PrimaryButtonText = primary,
            SecondaryButtonText = secondary,
        };

        ContentDialogResult result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }
    
    public async Task<IStorageFile?> OpenUAssetFile()
    {
        IReadOnlyList<IStorageFile?> result = await GetStorageProvider().OpenFilePickerAsync(new()
        {
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new("UAsset files")
                {
                    Patterns = new[] {"*.uasset"},
                    AppleUniformTypeIdentifiers = new[] {"public.item"},
                },
            },
        });

        return result.Count != 1 ? null : result[0];
    }

    public bool IsEverythingSaved()
    {
        if (iconTableView.fileSaveState == TableTab.FileSaveState.Unsaved) return false;
        if (plateTableView.fileSaveState == TableTab.FileSaveState.Unsaved) return false;
        if (gradeTableView.fileSaveState == TableTab.FileSaveState.Unsaved) return false;
        if (gradePartsTableView.fileSaveState == TableTab.FileSaveState.Unsaved) return false;
        if (musicParameterTableView.fileSaveState == TableTab.FileSaveState.Unsaved) return false;
        if (bossStageTableView.fileSaveState == TableTab.FileSaveState.Unsaved) return false;
        if (infernoUnlockTableView.fileSaveState == TableTab.FileSaveState.Unsaved) return false;
        if (musicUnlockTableView.fileSaveState == TableTab.FileSaveState.Unsaved) return false;
        if (itemUnlockTableView.fileSaveState == TableTab.FileSaveState.Unsaved) return false;
        if (conditionTableView.fileSaveState == TableTab.FileSaveState.Unsaved) return false;
        if (gateTableView.fileSaveState == TableTab.FileSaveState.Unsaved) return false;
        if (gateStepTableView.fileSaveState == TableTab.FileSaveState.Unsaved) return false;
        if (messageTableView.fileSaveState == TableTab.FileSaveState.Unsaved) return false;

        return true;
    }

    public void DragDrop(string path)
    {
        if (ViewContainer.Content is not TableTab tab) return;
        tab.OpenDragDrop(path);
    }
}