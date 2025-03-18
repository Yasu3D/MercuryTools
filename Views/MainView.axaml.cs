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
        bossStageTableView = new(this);
        infernoUnlockTableView = new(this);
        musicUnlockTableView = new(this);
        itemUnlockTableView = new(this);
        conditionTableView = new(this);
        gateTableView = new(this);
        gateStepTableView = new(this);
        messageTableView = new(this);

        ViewContainer.Content = iconTableView;
        
        KeyDownEvent.AddClassHandler<TopLevel>(OnKeyDown, RoutingStrategies.Tunnel, handledEventsToo: true);
        KeyUpEvent.AddClassHandler<TopLevel>(OnKeyUp, RoutingStrategies.Tunnel, handledEventsToo: true);
    }

    private readonly IconTableView iconTableView;
    private readonly PlateTableView plateTableView;
    private readonly GradeTableView gradeTableView;
    private readonly GradePartsTableView gradePartsTableView;
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
            "TabIconTable"          => iconTableView,
            "TabPlateTable"         => plateTableView,
            "TabGradeTable"         => gradeTableView,
            "TabGradePartsTable"    => gradePartsTableView,
            "TabBossStageTable"     => bossStageTableView,
            "TabInfernoUnlockTable" => infernoUnlockTableView,
            "TabMusicUnlockTable"   => musicUnlockTableView,
            "TabItemUnlockTable"    => itemUnlockTableView,
            "TabConditionTable"     => conditionTableView,
            "TabGateTable"          => gateTableView,
            "TabGateStepTable"      => gateStepTableView,
            "TabMessageTable"       => messageTableView,
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
        if (TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement() is TextBox) return;
        if (args.Key is Key.LeftAlt or Key.RightAlt or Key.LeftShift or Key.RightShift or Key.LeftCtrl or Key.RightCtrl)
        {
            args.Handled = true;
            return;
        }
        
        // Very very janky.
        if (ViewContainer.Content is not Tab tab) return;
        
        if (args.Key is Key.O && args.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            tab.ButtonOpen_OnClick(this, null!);
        }
        
        if (args.Key is Key.S && args.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            tab.ButtonSave_OnClick(this, null!);
        }
        
        if (args.Key is Key.Z && args.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            tab.ButtonUndo_OnClick(this, null!);
        }
        
        if (args.Key is Key.Y && args.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            tab.ButtonRedo_OnClick(this, null!);
        }
        
        if (args.Key is Key.Up && args.KeyModifiers.HasFlag(KeyModifiers.Alt))
        {
            tab.ButtonMoveElementUp_OnClick(this, null!);
        }
        
        if (args.Key is Key.Down && args.KeyModifiers.HasFlag(KeyModifiers.Alt))
        {
            tab.ButtonMoveElementDown_OnClick(this, null!);
        }
    }
    
    private static void OnKeyUp(object sender, KeyEventArgs e) => e.Handled = true;

    public static void ShowWarningMessage(string title, string? text = null)
    {
        ContentDialog dialog = new()
        {
            Title = title,
            Content = text,
            PrimaryButtonText = "Ok",
        };

        dialog.ShowAsync();
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
}