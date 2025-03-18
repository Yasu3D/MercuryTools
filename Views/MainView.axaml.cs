using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
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

    public void ShowWarningMessage(string title, string? text = null)
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