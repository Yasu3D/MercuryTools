using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MercuryTools.Views.Tabs;

namespace MercuryTools.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        iconTableView = new();
        plateTableView = new();
        gradeTableView = new();
        gradePartsTableView = new();
        bossStageTableView = new();
        infernoUnlockTableView = new();
        musicUnlockTableView = new();
        itemUnlockTableView = new();
        conditionTableView = new();
        gateTableView = new();
        gateStepTableView = new();
        messageTableView = new();

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
}