using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MercuryTools.UndoRedo.Operations;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace MercuryTools.Views.Tabs;

public partial class MusicParameterTableView : TableTab
{
    public MusicParameterTableView(MainView main)
    {
        InitializeComponent();
        mainView = main;
        explorerView = Explorer;
        undoRedoManager = new();

        undoRedoManager.OperationHistoryChanged += OnOperationHistoryChanged;

        explorerView.TextBoxSearch.TextChanging += TextBoxSearch_OnTextChanging;
        explorerView.ToggleSearch.IsCheckedChanged += ToggleSearch_OnIsCheckedChanged;
        explorerView.ToggleMatchCase.IsCheckedChanged += ToggleMatchCase_OnIsCheckedChanged;
        explorerView.ToggleInvertQuery.IsCheckedChanged += ToggleInvertQuery_OnIsCheckedChanged;
        
        explorerView.ButtonSave.Click += ButtonSave_OnClick;
        explorerView.ButtonOpen.Click += ButtonOpen_OnClick;
        
        explorerView.ButtonUndo.Click += ButtonUndo_OnClick;
        explorerView.ButtonRedo.Click += ButtonRedo_OnClick;
        
        explorerView.ButtonMoveElementUp.Click += ButtonMoveElementUp_OnClick;
        explorerView.ButtonMoveElementDown.Click += ButtonMoveElementDown_OnClick;
        
        explorerView.ButtonAddElement.Click += ButtonAddElement_OnClick;
        explorerView.ButtonDuplicateElement.Click += ButtonDuplicateElement_OnClick;
        explorerView.ButtonDeleteElement.Click += ButtonDeleteElement_OnClick;
        
        explorerView.TreeViewElementList.SelectionChanged += TreeView_OnSelectionChanged;
    }

    protected override StructPropertyData NewData => new()
    {
        Name = new(asset, "NO_NAME"),
        StructType = new(asset, "MusicParameterTableData"),
        Value =
        [
            new UInt32PropertyData(new(asset, "UniqueId")),
            new StrPropertyData(new(asset, "MusicMessage")),
            new StrPropertyData(new(asset, "ArtistMessage")),
            new StrPropertyData(new(asset, "CopyrightMessage")),
            new UInt32PropertyData(new(asset, "VersionNo")),
            new StrPropertyData(new(asset, "AssetDirectory")),
            new StrPropertyData(new(asset, "MovieAssetName")),
            new StrPropertyData(new(asset, "MovieAssetNameHard")),
            new StrPropertyData(new(asset, "MovieAssetNameExpert")),
            new StrPropertyData(new(asset, "MovieAssetNameInferno")),
            new StrPropertyData(new(asset, "JacketAssetName")),
            new StrPropertyData(new(asset, "Rubi")),
            new BoolPropertyData(new(asset, "bValidCulture_ja_JP")),
            new BoolPropertyData(new(asset, "bValidCulture_en_US")),
            new BoolPropertyData(new(asset, "bValidCulture_zh_Hant_TW")),
            new BoolPropertyData(new(asset, "bValidCulture_en_HK")),
            new BoolPropertyData(new(asset, "bValidCulture_en_SG")),
            new BoolPropertyData(new(asset, "bValidCulture_ko_KR")),
            new BoolPropertyData(new(asset, "bValidCulture_h_Hans_CN_Guest")),
            new BoolPropertyData(new(asset, "bValidCulture_h_Hans_CN_GeneralMember")),
            new BoolPropertyData(new(asset, "bValidCulture_h_Hans_CN_VipMember")),
            new BoolPropertyData(new(asset, "bValidCulture_Offline")),
            new BoolPropertyData(new(asset, "bValidCulture_NoneActive")),
            new BoolPropertyData(new(asset, "bRecommend")),
            new IntPropertyData(new(asset, "WaccaPointCost")),
            new BytePropertyData(new(asset, "bCollaboration")),
            new BytePropertyData(new(asset, "bWaccaOriginal")),
            new BytePropertyData(new(asset, "TrainingLevel")),
            new BytePropertyData(new(asset, "Reserved")),
            new StrPropertyData(new(asset, "Bpm")),
            new StrPropertyData(new(asset, "HashTag")),
            new StrPropertyData(new(asset, "NotesDesignerNormal")),
            new StrPropertyData(new(asset, "NotesDesignerHard")),
            new StrPropertyData(new(asset, "NotesDesignerExpert")),
            new StrPropertyData(new(asset, "NotesDesignerInferno")),
            new FloatPropertyData(new(asset, "DifficultyNormalLv")),
            new FloatPropertyData(new(asset, "DifficultyHardLv")),
            new FloatPropertyData(new(asset, "DifficultyExtremeLv")),
            new FloatPropertyData(new(asset, "DifficultyInfernoLv")),
            new FloatPropertyData(new(asset, "ClearNormaRateNormal")),
            new FloatPropertyData(new(asset, "ClearNormaRateHard")),
            new FloatPropertyData(new(asset, "ClearNormaRateExtreme")),
            new FloatPropertyData(new(asset, "ClearNormaRateInferno")),
            new FloatPropertyData(new(asset, "PreviewBeginTime")),
            new FloatPropertyData(new(asset, "PreviewSeconds")),
            new IntPropertyData(new(asset, "ScoreGenre")),
            new IntPropertyData(new(asset, "MusicTagForUnlock0")),
            new IntPropertyData(new(asset, "MusicTagForUnlock1")),
            new IntPropertyData(new(asset, "MusicTagForUnlock2")),
            new IntPropertyData(new(asset, "MusicTagForUnlock3")),
            new IntPropertyData(new(asset, "MusicTagForUnlock4")),
            new IntPropertyData(new(asset, "MusicTagForUnlock5")),
            new IntPropertyData(new(asset, "MusicTagForUnlock6")),
            new IntPropertyData(new(asset, "MusicTagForUnlock7")),
            new IntPropertyData(new(asset, "MusicTagForUnlock8")),
            new IntPropertyData(new(asset, "MusicTagForUnlock9")),
            new UInt64PropertyData(new(asset, "WorkBuffer")),
            new StrPropertyData(new(asset, "AssetFullPath")),
        ],
    };

    protected override bool FormatCheck()
    {
        return table.Count != 0 && table[0].Value[0].Name.ToString() == "UniqueID" && table[0].Value[1].Name.ToString() == "MusicMessage";
    }
    
    protected override bool ContentContainsQuery(StructPropertyData data)
    {
        StringComparison comparison = SearchMatchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        
        // Check Name
        string? name = data.Name.Value?.Value;
        if (Utils.Filter(name, "Name", SearchQuery, comparison)) return true;
        
        // Check Data
        
        string uniqueId = ((UInt32PropertyData)data.Value[0]).Value.ToString();
        if (Utils.Filter(uniqueId, "UniqueId", SearchQuery, comparison)) return true;
        
        string? musicMessage = ((StrPropertyData)data.Value[1]).Value?.Value;
        if (Utils.Filter(musicMessage, "MusicMessage", SearchQuery, comparison)) return true;
        
        string? artistMessage = ((StrPropertyData)data.Value[2]).Value?.Value;
        if (Utils.Filter(artistMessage, "ArtistMessage", SearchQuery, comparison)) return true;
        
        string? copyrightMessage = ((StrPropertyData)data.Value[3]).Value?.Value;
        if (Utils.Filter(copyrightMessage, "CopyrightMessage", SearchQuery, comparison)) return true;
        
        string versionNo = ((UInt32PropertyData)data.Value[4]).Value.ToString();
        if (Utils.Filter(versionNo, "VersionNo", SearchQuery, comparison)) return true;
        
        string? assetDirectory = ((StrPropertyData)data.Value[5]).Value?.Value;
        if (Utils.Filter(assetDirectory, "AssetDirectory", SearchQuery, comparison)) return true;
        
        string? movieAssetName = ((StrPropertyData)data.Value[6]).Value?.Value;
        if (Utils.Filter(movieAssetName, "MovieAssetName", SearchQuery, comparison)) return true;
        
        string? movieAssetNameHard = ((StrPropertyData)data.Value[7]).Value?.Value;
        if (Utils.Filter(movieAssetNameHard, "MovieAssetNameHard", SearchQuery, comparison)) return true;
        
        string? movieAssetNameExpert = ((StrPropertyData)data.Value[8]).Value?.Value;
        if (Utils.Filter(movieAssetNameExpert, "MovieAssetNameExpert", SearchQuery, comparison)) return true;
        
        string? movieAssetNameInferno = ((StrPropertyData)data.Value[9]).Value?.Value;
        if (Utils.Filter(movieAssetNameInferno, "MovieAssetNameInferno", SearchQuery, comparison)) return true;
        
        string? jacketAssetName = ((StrPropertyData)data.Value[10]).Value?.Value;
        if (Utils.Filter(jacketAssetName, "JacketAssetName", SearchQuery, comparison)) return true;
        
        string? rubi = ((StrPropertyData)data.Value[11]).Value?.Value;
        if (Utils.Filter(rubi, "Rubi", SearchQuery, comparison)) return true;
        
        string bValidCulture_ja_JP = ((BoolPropertyData)data.Value[12]).Value.ToString();
        if (Utils.Filter(bValidCulture_ja_JP, "bValidCulture_ja_JP", SearchQuery, comparison)) return true;
        
        string bValidCulture_en_US = ((BoolPropertyData)data.Value[13]).Value.ToString();
        if (Utils.Filter(bValidCulture_en_US, "bValidCulture_en_US", SearchQuery, comparison)) return true;
        
        string bValidCulture_zh_Hant_TW = ((BoolPropertyData)data.Value[14]).Value.ToString();
        if (Utils.Filter(bValidCulture_zh_Hant_TW, "bValidCulture_zh_Hant_TW", SearchQuery, comparison)) return true;
        
        string bValidCulture_en_HK = ((BoolPropertyData)data.Value[15]).Value.ToString();
        if (Utils.Filter(bValidCulture_en_HK, "bValidCulture_en_HK", SearchQuery, comparison)) return true;
        
        string bValidCulture_en_SG = ((BoolPropertyData)data.Value[16]).Value.ToString();
        if (Utils.Filter(bValidCulture_en_SG, "bValidCulture_en_SG", SearchQuery, comparison)) return true;
        
        string bValidCulture_ko_KR = ((BoolPropertyData)data.Value[17]).Value.ToString();
        if (Utils.Filter(bValidCulture_ko_KR, "bValidCulture_ko_KR", SearchQuery, comparison)) return true;
        
        string bValidCulture_h_Hans_CN_Guest = ((BoolPropertyData)data.Value[18]).Value.ToString();
        if (Utils.Filter(bValidCulture_h_Hans_CN_Guest, "bValidCulture_h_Hans_CN_Guest", SearchQuery, comparison)) return true;
        
        string bValidCulture_h_Hans_CN_GeneralMember = ((BoolPropertyData)data.Value[19]).Value.ToString();
        if (Utils.Filter(bValidCulture_h_Hans_CN_GeneralMember, "bValidCulture_h_Hans_CN_GeneralMember", SearchQuery, comparison)) return true;
        
        string bValidCulture_h_Hans_CN_VipMember = ((BoolPropertyData)data.Value[20]).Value.ToString();
        if (Utils.Filter(bValidCulture_h_Hans_CN_VipMember, "bValidCulture_h_Hans_CN_VipMember", SearchQuery, comparison)) return true;
        
        string bValidCulture_Offline = ((BoolPropertyData)data.Value[21]).Value.ToString();
        if (Utils.Filter(bValidCulture_Offline, "bValidCulture_Offline", SearchQuery, comparison)) return true;
        
        string bValidCulture_NoneActive = ((BoolPropertyData)data.Value[22]).Value.ToString();
        if (Utils.Filter(bValidCulture_NoneActive, "bValidCulture_NoneActive", SearchQuery, comparison)) return true;
        
        string bRecommend = ((BoolPropertyData)data.Value[23]).Value.ToString();
        if (Utils.Filter(bRecommend, "bRecommend", SearchQuery, comparison)) return true;
        
        string waccaPointCost = ((IntPropertyData)data.Value[24]).Value.ToString();
        if (Utils.Filter(waccaPointCost, "WaccaPointCost", SearchQuery, comparison)) return true;
        
        string bCollaboration = ((BytePropertyData)data.Value[25]).Value.ToString();
        if (Utils.Filter(bCollaboration, "bCollaboration", SearchQuery, comparison)) return true;
        
        string bWaccaOriginal = ((BytePropertyData)data.Value[26]).Value.ToString();
        if (Utils.Filter(bWaccaOriginal, "bWaccaOriginal", SearchQuery, comparison)) return true;
        
        string trainingLevel = ((BytePropertyData)data.Value[27]).Value.ToString();
        if (Utils.Filter(trainingLevel, "TrainingLevel", SearchQuery, comparison)) return true;
        
        string reserved = ((BytePropertyData)data.Value[28]).Value.ToString();
        if (Utils.Filter(reserved, "Reserved", SearchQuery, comparison)) return true;
        
        string? bpm = ((StrPropertyData)data.Value[29]).Value?.Value;
        if (Utils.Filter(bpm, "Bpm", SearchQuery, comparison)) return true;
        
        string? hashTag = ((StrPropertyData)data.Value[30]).Value?.Value;
        if (Utils.Filter(hashTag, "HashTag", SearchQuery, comparison)) return true;
        
        string? notesDesignerNormal = ((StrPropertyData)data.Value[31]).Value?.Value;
        if (Utils.Filter(notesDesignerNormal, "NotesDesignerNormal", SearchQuery, comparison)) return true;
        
        string? notesDesignerHard = ((StrPropertyData)data.Value[32]).Value?.Value;
        if (Utils.Filter(notesDesignerHard, "NotesDesignerHard", SearchQuery, comparison)) return true;
        
        string? notesDesignerExpert = ((StrPropertyData)data.Value[33]).Value?.Value;
        if (Utils.Filter(notesDesignerExpert, "NotesDesignerExpert", SearchQuery, comparison)) return true;
        
        string? notesDesignerInferno = ((StrPropertyData)data.Value[34]).Value?.Value;
        if (Utils.Filter(notesDesignerInferno, "NotesDesignerInferno", SearchQuery, comparison)) return true;
        
        string difficultyNormalLv = ((FloatPropertyData)data.Value[35]).Value.ToString(CultureInfo.InvariantCulture);
        if (Utils.Filter(difficultyNormalLv, "DifficultyNormalLv", SearchQuery, comparison)) return true;
        
        string difficultyHardLv = ((FloatPropertyData)data.Value[36]).Value.ToString(CultureInfo.InvariantCulture);
        if (Utils.Filter(difficultyHardLv, "DifficultyHardLv", SearchQuery, comparison)) return true;
        
        string difficultyExtremeLv = ((FloatPropertyData)data.Value[37]).Value.ToString(CultureInfo.InvariantCulture);
        if (Utils.Filter(difficultyExtremeLv, "DifficultyExtremeLv", SearchQuery, comparison)) return true;
        
        string difficultyInfernoLv = ((FloatPropertyData)data.Value[38]).Value.ToString(CultureInfo.InvariantCulture);
        if (Utils.Filter(difficultyInfernoLv, "DifficultyInfernoLv", SearchQuery, comparison)) return true;
        
        string clearNormaRateNormal = ((FloatPropertyData)data.Value[39]).Value.ToString(CultureInfo.InvariantCulture);
        if (Utils.Filter(clearNormaRateNormal, "ClearNormaRateNormal", SearchQuery, comparison)) return true;
        
        string clearNormaRateHard = ((FloatPropertyData)data.Value[40]).Value.ToString(CultureInfo.InvariantCulture);
        if (Utils.Filter(clearNormaRateHard, "ClearNormaRateHard", SearchQuery, comparison)) return true;
        
        string clearNormaRateExtreme = ((FloatPropertyData)data.Value[41]).Value.ToString(CultureInfo.InvariantCulture);
        if (Utils.Filter(clearNormaRateExtreme, "ClearNormaRateExtreme", SearchQuery, comparison)) return true;
        
        string clearNormaRateInferno = ((FloatPropertyData)data.Value[42]).Value.ToString(CultureInfo.InvariantCulture);
        if (Utils.Filter(clearNormaRateInferno, "ClearNormaRateInferno", SearchQuery, comparison)) return true;
        
        string previewBeginTime = ((FloatPropertyData)data.Value[43]).Value.ToString(CultureInfo.InvariantCulture);
        if (Utils.Filter(previewBeginTime, "PreviewBeginTime", SearchQuery, comparison)) return true;
        
        string previewSeconds = ((FloatPropertyData)data.Value[44]).Value.ToString(CultureInfo.InvariantCulture);
        if (Utils.Filter(previewSeconds, "PreviewSeconds", SearchQuery, comparison)) return true;
        
        string scoreGenre = ((IntPropertyData)data.Value[45]).Value.ToString();
        if (Utils.Filter(scoreGenre, "ScoreGenre", SearchQuery, comparison)) return true;
        
        string musicTagForUnlock0 = ((IntPropertyData)data.Value[46]).Value.ToString();
        if (Utils.Filter(musicTagForUnlock0, "MusicTagForUnlock0", SearchQuery, comparison)) return true;
        
        string musicTagForUnlock1 = ((IntPropertyData)data.Value[47]).Value.ToString();
        if (Utils.Filter(musicTagForUnlock1, "MusicTagForUnlock1", SearchQuery, comparison)) return true;
        
        string musicTagForUnlock2 = ((IntPropertyData)data.Value[48]).Value.ToString();
        if (Utils.Filter(musicTagForUnlock2, "MusicTagForUnlock2", SearchQuery, comparison)) return true;
        
        string musicTagForUnlock3 = ((IntPropertyData)data.Value[49]).Value.ToString();
        if (Utils.Filter(musicTagForUnlock3, "MusicTagForUnlock3", SearchQuery, comparison)) return true;
        
        string musicTagForUnlock4 = ((IntPropertyData)data.Value[50]).Value.ToString();
        if (Utils.Filter(musicTagForUnlock4, "MusicTagForUnlock4", SearchQuery, comparison)) return true;
        
        string musicTagForUnlock5 = ((IntPropertyData)data.Value[51]).Value.ToString();
        if (Utils.Filter(musicTagForUnlock5, "MusicTagForUnlock5", SearchQuery, comparison)) return true;
        
        string musicTagForUnlock6 = ((IntPropertyData)data.Value[52]).Value.ToString();
        if (Utils.Filter(musicTagForUnlock6, "MusicTagForUnlock6", SearchQuery, comparison)) return true;
        
        string musicTagForUnlock7 = ((IntPropertyData)data.Value[53]).Value.ToString();
        if (Utils.Filter(musicTagForUnlock7, "MusicTagForUnlock7", SearchQuery, comparison)) return true;
        
        string musicTagForUnlock8 = ((IntPropertyData)data.Value[54]).Value.ToString();
        if (Utils.Filter(musicTagForUnlock8, "MusicTagForUnlock8", SearchQuery, comparison)) return true;
        
        string musicTagForUnlock9 = ((IntPropertyData)data.Value[55]).Value.ToString();
        if (Utils.Filter(musicTagForUnlock9, "MusicTagForUnlock9", SearchQuery, comparison)) return true;
        
        string workBuffer = ((UInt64PropertyData)data.Value[56]).Value.ToString();
        if (Utils.Filter(workBuffer, "WorkBuffer", SearchQuery, comparison)) return true;
        
        string? assetFullPath = ((StrPropertyData)data.Value[57]).Value?.Value;
        if (Utils.Filter(assetFullPath, "AssetFullPath", SearchQuery, comparison)) return true;
        
        return false;
    }
    
    protected override void UpdateContent(bool ignoreChange)
    {
        if (explorerView?.SelectedItem == null)
        {
            ContentGroup.IsVisible = false;
            return;
        }

        // Get selected item and connected Data
        if (explorerView.SelectedItem.Tag is not StructPropertyData data)
        {
            ContentGroup.IsVisible = false;
            return;
        }

        try
        {
            // Get Properties
            UInt32PropertyData uniqueId = (UInt32PropertyData)data.Value[0];
            StrPropertyData musicMessage = (StrPropertyData)data.Value[1];
            StrPropertyData artistMessage = (StrPropertyData)data.Value[2];
            StrPropertyData copyrightMessage = (StrPropertyData)data.Value[3];
            UInt32PropertyData versionNo = (UInt32PropertyData)data.Value[4];
            StrPropertyData assetDirectory = (StrPropertyData)data.Value[5];
            StrPropertyData movieAssetName = (StrPropertyData)data.Value[6];
            StrPropertyData movieAssetNameHard = (StrPropertyData)data.Value[7];
            StrPropertyData movieAssetNameExpert = (StrPropertyData)data.Value[8];
            StrPropertyData movieAssetNameInferno = (StrPropertyData)data.Value[9];
            StrPropertyData jacketAssetName = (StrPropertyData)data.Value[10];
            StrPropertyData rubi = (StrPropertyData)data.Value[11];
            BoolPropertyData bValidCulture_ja_JP = (BoolPropertyData)data.Value[12];
            BoolPropertyData bValidCulture_en_US = (BoolPropertyData)data.Value[13];
            BoolPropertyData bValidCulture_zh_Hant_TW = (BoolPropertyData)data.Value[14];
            BoolPropertyData bValidCulture_en_HK = (BoolPropertyData)data.Value[15];
            BoolPropertyData bValidCulture_en_SG = (BoolPropertyData)data.Value[16];
            BoolPropertyData bValidCulture_ko_KR = (BoolPropertyData)data.Value[17];
            BoolPropertyData bValidCulture_h_Hans_CN_Guest = (BoolPropertyData)data.Value[18];
            BoolPropertyData bValidCulture_h_Hans_CN_GeneralMember = (BoolPropertyData)data.Value[19];
            BoolPropertyData bValidCulture_h_Hans_CN_VipMember = (BoolPropertyData)data.Value[20];
            BoolPropertyData bValidCulture_Offline = (BoolPropertyData)data.Value[21];
            BoolPropertyData bValidCulture_NoneActive = (BoolPropertyData)data.Value[22];
            BoolPropertyData bRecommend = (BoolPropertyData)data.Value[23];
            IntPropertyData waccaPointCost = (IntPropertyData)data.Value[24];
            BytePropertyData bCollaboration = (BytePropertyData)data.Value[25];
            BytePropertyData bWaccaOriginal = (BytePropertyData)data.Value[26];
            BytePropertyData trainingLevel = (BytePropertyData)data.Value[27];
            BytePropertyData reserved = (BytePropertyData)data.Value[28];
            StrPropertyData bpm = (StrPropertyData)data.Value[29];
            StrPropertyData hashTag = (StrPropertyData)data.Value[30];
            StrPropertyData notesDesignerNormal = (StrPropertyData)data.Value[31];
            StrPropertyData notesDesignerHard = (StrPropertyData)data.Value[32];
            StrPropertyData notesDesignerExpert = (StrPropertyData)data.Value[33];
            StrPropertyData notesDesignerInferno = (StrPropertyData)data.Value[34];
            FloatPropertyData difficultyNormalLv = (FloatPropertyData)data.Value[35];
            FloatPropertyData difficultyHardLv = (FloatPropertyData)data.Value[36];
            FloatPropertyData difficultyExtremeLv = (FloatPropertyData)data.Value[37];
            FloatPropertyData difficultyInfernoLv = (FloatPropertyData)data.Value[38];
            FloatPropertyData clearNormaRateNormal = (FloatPropertyData)data.Value[39];
            FloatPropertyData clearNormaRateHard = (FloatPropertyData)data.Value[40];
            FloatPropertyData clearNormaRateExtreme = (FloatPropertyData)data.Value[41];
            FloatPropertyData clearNormaRateInferno = (FloatPropertyData)data.Value[42];
            FloatPropertyData previewBeginTime = (FloatPropertyData)data.Value[43];
            FloatPropertyData previewSeconds = (FloatPropertyData)data.Value[44];
            IntPropertyData scoreGenre = (IntPropertyData)data.Value[45];
            IntPropertyData musicTagForUnlock0 = (IntPropertyData)data.Value[46];
            IntPropertyData musicTagForUnlock1 = (IntPropertyData)data.Value[47];
            IntPropertyData musicTagForUnlock2 = (IntPropertyData)data.Value[48];
            IntPropertyData musicTagForUnlock3 = (IntPropertyData)data.Value[49];
            IntPropertyData musicTagForUnlock4 = (IntPropertyData)data.Value[50];
            IntPropertyData musicTagForUnlock5 = (IntPropertyData)data.Value[51];
            IntPropertyData musicTagForUnlock6 = (IntPropertyData)data.Value[52];
            IntPropertyData musicTagForUnlock7 = (IntPropertyData)data.Value[53];
            IntPropertyData musicTagForUnlock8 = (IntPropertyData)data.Value[54];
            IntPropertyData musicTagForUnlock9 = (IntPropertyData)data.Value[55];
            UInt64PropertyData workBuffer = (UInt64PropertyData)data.Value[56];
            StrPropertyData assetFullPath = (StrPropertyData)data.Value[57];

            if (ignoreChange) ignoreDataChange = true;
            
            // Set Content to StructPropertyData contents
            ContentGroup.IsVisible = true;
            TextBoxName.Text = data.Name.Value?.Value ?? "NO_NAME";
            
            TextBoxUniqueId.Text = uniqueId.Value.ToString();
            TextBoxMusicMessage.Text = musicMessage.Value?.Value ?? "";
            TextBoxArtistMessage.Text = artistMessage.Value?.Value ?? "";
            TextBoxCopyrightMessage.Text = copyrightMessage.Value?.Value ?? "";
            TextBoxVersionNo.Text = versionNo.Value.ToString();
            TextBoxAssetDirectory.Text = assetDirectory.Value?.Value ?? "";
            TextBoxMovieAssetName.Text = movieAssetName.Value?.Value ?? "";
            TextBoxMovieAssetNameHard.Text = movieAssetNameHard.Value?.Value ?? "";
            TextBoxMovieAssetNameExpert.Text = movieAssetNameExpert.Value?.Value ?? "";
            TextBoxMovieAssetNameInferno.Text = movieAssetNameInferno.Value?.Value ?? "";
            TextBoxJacketAssetName.Text = jacketAssetName.Value?.Value ?? "";
            TextBoxRubi.Text = rubi.Value?.Value ?? "";
            CheckBoxValidCulture_ja_JP.IsChecked = bValidCulture_ja_JP.Value;
            CheckBoxValidCulture_en_US.IsChecked = bValidCulture_en_US.Value;
            CheckBoxValidCulture_zh_Hant_TW.IsChecked = bValidCulture_zh_Hant_TW.Value;
            CheckBoxValidCulture_en_HK.IsChecked = bValidCulture_en_HK.Value;
            CheckBoxValidCulture_en_SG.IsChecked = bValidCulture_en_SG.Value;
            CheckBoxValidCulture_ko_KR.IsChecked = bValidCulture_ko_KR.Value;
            CheckBoxValidCulture_h_Hans_CN_Guest.IsChecked = bValidCulture_h_Hans_CN_Guest.Value;
            CheckBoxValidCulture_h_Hans_CN_GeneralMember.IsChecked = bValidCulture_h_Hans_CN_GeneralMember.Value;
            CheckBoxValidCulture_h_Hans_CN_VipMember.IsChecked = bValidCulture_h_Hans_CN_VipMember.Value;
            CheckBoxValidCulture_Offline.IsChecked = bValidCulture_Offline.Value;
            CheckBoxValidCulture_NoneActive.IsChecked = bValidCulture_NoneActive.Value;
            CheckBoxRecommend.IsChecked = bRecommend.Value;
            TextBoxWaccaPointCost.Text = waccaPointCost.Value.ToString();
            TextBoxCollaboration.Text = bCollaboration.Value.ToString();
            TextBoxWaccaOriginal.Text = bWaccaOriginal.Value.ToString();
            TextBoxTrainingLevel.Text = trainingLevel.Value.ToString();
            TextBoxReserved.Text = reserved.Value.ToString();
            TextBoxBpm.Text = bpm.Value?.Value ?? "";
            TextBoxHashTag.Text = hashTag.Value?.Value ?? "";
            TextBoxNotesDesignerNormal.Text = notesDesignerNormal.Value?.Value ?? "";
            TextBoxNotesDesignerHard.Text = notesDesignerHard.Value?.Value ?? "";
            TextBoxNotesDesignerExpert.Text = notesDesignerExpert.Value?.Value ?? "";
            TextBoxNotesDesignerInferno.Text = notesDesignerInferno.Value?.Value ?? "";
            TextBoxDifficultyNormalLv.Text = difficultyNormalLv.Value.ToString(CultureInfo.InvariantCulture);
            TextBoxDifficultyHardLv.Text = difficultyHardLv.Value.ToString(CultureInfo.InvariantCulture);
            TextBoxDifficultyExtremeLv.Text = difficultyExtremeLv.Value.ToString(CultureInfo.InvariantCulture);
            TextBoxDifficultyInfernoLv.Text = difficultyInfernoLv.Value.ToString(CultureInfo.InvariantCulture);
            TextBoxClearNormaRateNormal.Text = clearNormaRateNormal.Value.ToString(CultureInfo.InvariantCulture);
            TextBoxClearNormaRateHard.Text = clearNormaRateHard.Value.ToString(CultureInfo.InvariantCulture);
            TextBoxClearNormaRateExtreme.Text = clearNormaRateExtreme.Value.ToString(CultureInfo.InvariantCulture);
            TextBoxClearNormaRateInferno.Text = clearNormaRateInferno.Value.ToString(CultureInfo.InvariantCulture);
            TextBoxPreviewBeginTime.Text = previewBeginTime.Value.ToString(CultureInfo.InvariantCulture);
            TextBoxPreviewSeconds.Text = previewSeconds.Value.ToString(CultureInfo.InvariantCulture);
            TextBoxScoreGenre.Text = scoreGenre.Value.ToString();
            TextBoxMusicTagForUnlock0.Text = musicTagForUnlock0.Value.ToString();
            TextBoxMusicTagForUnlock1.Text = musicTagForUnlock1.Value.ToString();
            TextBoxMusicTagForUnlock2.Text = musicTagForUnlock2.Value.ToString();
            TextBoxMusicTagForUnlock3.Text = musicTagForUnlock3.Value.ToString();
            TextBoxMusicTagForUnlock4.Text = musicTagForUnlock4.Value.ToString();
            TextBoxMusicTagForUnlock5.Text = musicTagForUnlock5.Value.ToString();
            TextBoxMusicTagForUnlock6.Text = musicTagForUnlock6.Value.ToString();
            TextBoxMusicTagForUnlock7.Text = musicTagForUnlock7.Value.ToString();
            TextBoxMusicTagForUnlock8.Text = musicTagForUnlock8.Value.ToString();
            TextBoxMusicTagForUnlock9.Text = musicTagForUnlock9.Value.ToString();
            TextBoxWorkBuffer.Text = workBuffer.Value.ToString();
            TextBoxAssetFullPath.Text = assetFullPath.Value?.Value ?? "";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
        finally
        {
            if (ignoreChange) ignoreDataChange = false;
        }
    }
    
    private void TextBox_OnTextChanging(object? sender, TextChangingEventArgs args)
    {
        if (ignoreDataChange) return;
        
        if (asset == null) return;
        if (undoRedoManager == null) return;
        if (explorerView?.SelectedItem == null) return;
        if (sender is not TextBox textBox) return;
        
        try
        {
            TreeViewItem item = explorerView.SelectedItem;
            if (item.Tag is not StructPropertyData data) return;
            
            switch (textBox.Name)
            {
                case "TextBoxName":
                {
                    string name = string.IsNullOrEmpty(TextBoxName.Text) ? "NO_NAME" : TextBoxName.Text;
                    
                    FName oldName = data.Name;
                    FName newName = new(asset, name);

                    ModifyStructPropertyName operation = new(data, oldName, newName);
                    undoRedoManager.RedoAndPush(operation);
                    
                    UpdateTreeView(true);
                    break;
                }
                
                case "TextBoxUniqueId":
                {
                    UInt32PropertyData uint32PropertyData = (UInt32PropertyData)data.Value[0];
                    uint oldValue = uint32PropertyData.Value;
                    uint newValue;
                    
                    try
                    {
                        newValue = Convert.ToUInt32(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyUInt32PropertyDataValue operation = new(data, uint32PropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxMusicMessage":
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[1];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxArtistMessage":
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[2];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxCopyrightMessage":
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[3];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxVersionNo":
                {
                    UInt32PropertyData uint32PropertyData = (UInt32PropertyData)data.Value[4];
                    uint oldValue = uint32PropertyData.Value;
                    uint newValue;
                    
                    try
                    {
                        newValue = Convert.ToUInt32(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyUInt32PropertyDataValue operation = new(data, uint32PropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxAssetDirectory":
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[5];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxMovieAssetName":
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[6];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxMovieAssetNameHard":
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[7];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxMovieAssetNameExpert":
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[8];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxMovieAssetNameInferno":
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[9];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxJacketAssetName":
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[10];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxRubi":
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[11];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxWaccaPointCost":
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[24];
                    int oldValue = intPropertyData.Value;
                    int newValue;
                    
                    try
                    {
                        newValue = Convert.ToInt32(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyInt32PropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxCollaboration":
                {
                    BytePropertyData bytePropertyData = (BytePropertyData)data.Value[25];
                    byte oldValue = bytePropertyData.Value;
                    byte newValue;
                    
                    try
                    {
                        newValue = Convert.ToByte(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyBytePropertyDataValue operation = new(data, bytePropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxWaccaOriginal":
                {
                    BytePropertyData bytePropertyData = (BytePropertyData)data.Value[26];
                    byte oldValue = bytePropertyData.Value;
                    byte newValue;
                    
                    try
                    {
                        newValue = Convert.ToByte(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyBytePropertyDataValue operation = new(data, bytePropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxTrainingLevel":
                {
                    BytePropertyData bytePropertyData = (BytePropertyData)data.Value[27];
                    byte oldValue = bytePropertyData.Value;
                    byte newValue;
                    
                    try
                    {
                        newValue = Convert.ToByte(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyBytePropertyDataValue operation = new(data, bytePropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxReserved":
                {
                    BytePropertyData bytePropertyData = (BytePropertyData)data.Value[28];
                    byte oldValue = bytePropertyData.Value;
                    byte newValue;
                    
                    try
                    {
                        newValue = Convert.ToByte(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyBytePropertyDataValue operation = new(data, bytePropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxBpm":
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[29];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxHashTag":
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[30];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxNotesDesignerNormal":
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[31];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxNotesDesignerHard":
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[32];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxNotesDesignerExpert":
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[33];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxNotesDesignerInferno":
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[34];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
                
                case "TextBoxDifficultyNormalLv":
                {
                    FloatPropertyData intPropertyData = (FloatPropertyData)data.Value[35];
                    float oldValue = intPropertyData.Value;
                    float newValue;
                    
                    try
                    {
                        newValue = Convert.ToSingle(textBox.Text, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyFloatPropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxDifficultyHardLv":
                {
                    FloatPropertyData intPropertyData = (FloatPropertyData)data.Value[36];
                    float oldValue = intPropertyData.Value;
                    float newValue;
                    
                    try
                    {
                        newValue = Convert.ToSingle(textBox.Text, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyFloatPropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxDifficultyExtremeLv":
                {
                    FloatPropertyData intPropertyData = (FloatPropertyData)data.Value[37];
                    float oldValue = intPropertyData.Value;
                    float newValue;
                    
                    try
                    {
                        newValue = Convert.ToSingle(textBox.Text, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyFloatPropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxDifficultyInfernoLv":
                {
                    FloatPropertyData intPropertyData = (FloatPropertyData)data.Value[38];
                    float oldValue = intPropertyData.Value;
                    float newValue;
                    
                    try
                    {
                        newValue = Convert.ToSingle(textBox.Text, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyFloatPropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxClearNormaRateNormal":
                {
                    FloatPropertyData intPropertyData = (FloatPropertyData)data.Value[39];
                    float oldValue = intPropertyData.Value;
                    float newValue;
                    
                    try
                    {
                        newValue = Convert.ToSingle(textBox.Text, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyFloatPropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxClearNormaRateHard":
                {
                    FloatPropertyData intPropertyData = (FloatPropertyData)data.Value[40];
                    float oldValue = intPropertyData.Value;
                    float newValue;
                    
                    try
                    {
                        newValue = Convert.ToSingle(textBox.Text, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyFloatPropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxClearNormaRateExtreme":
                {
                    FloatPropertyData intPropertyData = (FloatPropertyData)data.Value[41];
                    float oldValue = intPropertyData.Value;
                    float newValue;
                    
                    try
                    {
                        newValue = Convert.ToSingle(textBox.Text, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyFloatPropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxClearNormaRateInferno":
                {
                    FloatPropertyData intPropertyData = (FloatPropertyData)data.Value[42];
                    float oldValue = intPropertyData.Value;
                    float newValue;
                    
                    try
                    {
                        newValue = Convert.ToSingle(textBox.Text, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyFloatPropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxPreviewBeginTime":
                {
                    FloatPropertyData intPropertyData = (FloatPropertyData)data.Value[43];
                    float oldValue = intPropertyData.Value;
                    float newValue;
                    
                    try
                    {
                        newValue = Convert.ToSingle(textBox.Text, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyFloatPropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxPreviewSeconds":
                {
                    FloatPropertyData intPropertyData = (FloatPropertyData)data.Value[44];
                    float oldValue = intPropertyData.Value;
                    float newValue;
                    
                    try
                    {
                        newValue = Convert.ToSingle(textBox.Text, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyFloatPropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxScoreGenre":
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[45];
                    int oldValue = intPropertyData.Value;
                    int newValue;
                    
                    try
                    {
                        newValue = Convert.ToInt32(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyInt32PropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxMusicTagForUnlock0":
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[46];
                    int oldValue = intPropertyData.Value;
                    int newValue;
                    
                    try
                    {
                        newValue = Convert.ToInt32(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyInt32PropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxMusicTagForUnlock1":
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[47];
                    int oldValue = intPropertyData.Value;
                    int newValue;
                    
                    try
                    {
                        newValue = Convert.ToInt32(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyInt32PropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxMusicTagForUnlock2":
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[48];
                    int oldValue = intPropertyData.Value;
                    int newValue;
                    
                    try
                    {
                        newValue = Convert.ToInt32(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyInt32PropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxMusicTagForUnlock3":
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[49];
                    int oldValue = intPropertyData.Value;
                    int newValue;
                    
                    try
                    {
                        newValue = Convert.ToInt32(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyInt32PropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxMusicTagForUnlock4":
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[50];
                    int oldValue = intPropertyData.Value;
                    int newValue;
                    
                    try
                    {
                        newValue = Convert.ToInt32(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyInt32PropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxMusicTagForUnlock5":
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[51];
                    int oldValue = intPropertyData.Value;
                    int newValue;
                    
                    try
                    {
                        newValue = Convert.ToInt32(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyInt32PropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxMusicTagForUnlock6":
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[52];
                    int oldValue = intPropertyData.Value;
                    int newValue;
                    
                    try
                    {
                        newValue = Convert.ToInt32(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyInt32PropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxMusicTagForUnlock7":
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[53];
                    int oldValue = intPropertyData.Value;
                    int newValue;
                    
                    try
                    {
                        newValue = Convert.ToInt32(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyInt32PropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxMusicTagForUnlock8":
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[54];
                    int oldValue = intPropertyData.Value;
                    int newValue;
                    
                    try
                    {
                        newValue = Convert.ToInt32(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyInt32PropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxMusicTagForUnlock9":
                {
                    IntPropertyData intPropertyData = (IntPropertyData)data.Value[55];
                    int oldValue = intPropertyData.Value;
                    int newValue;
                    
                    try
                    {
                        newValue = Convert.ToInt32(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyInt32PropertyDataValue operation = new(data, intPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxWorkBuffer":
                {
                    UInt64PropertyData uint64PropertyData = (UInt64PropertyData)data.Value[56];
                    ulong oldValue = uint64PropertyData.Value;
                    ulong newValue;
                    
                    try
                    {
                        newValue = Convert.ToUInt64(textBox.Text);
                    }
                    catch (FormatException)
                    {
                        newValue = 0;
                    }

                    ModifyUInt64PropertyDataValue operation = new(data, uint64PropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "TextBoxAssetFullPath":
                {
                    StrPropertyData strPropertyData = (StrPropertyData)data.Value[57];
                    FString oldValue = strPropertyData.Value;
                    FString newValue = new(textBox.Text);

                    ModifyStringPropertyDataValue operation = new(data, strPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break; 
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }

    private void TextBox_OnLostFocus(object? sender, RoutedEventArgs args)
    {
        if (asset == null) return;
        if (undoRedoManager == null) return;
        if (explorerView?.SelectedItem == null) return;
        if (sender is not TextBox textBox) return;

        try
        {
            switch (textBox.Name)
            {
                // Name
                case "TextBoxName":
                {
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        textBox.Text = "NO_NAME";
                    }

                    break;
                }
                    
                // StrProperty
                case "TextBoxMusicMessage":
                case "TextBoxArtistMessage":
                case "TextBoxCopyrightMessage":
                case "TextBoxAssetDirectory":
                case "TextBoxMovieAssetName":
                case "TextBoxMovieAssetNameHard":
                case "TextBoxMovieAssetNameExpert":
                case "TextBoxMovieAssetNameInferno":
                case "TextBoxJacketAssetName":
                case "TextBoxRubi":
                case "TextBoxBpm":
                case "TextBoxHashTag":
                case "TextBoxNotesDesignerNormal":
                case "TextBoxNotesDesignerHard":
                case "TextBoxNotesDesignerExpert":
                case "TextBoxNotesDesignerInferno":
                case "TextBoxAssetFullPath":
                {
                    return;
                }
                    
                // UInt32Property
                case "TextBoxUniqueId":
                case "TextBoxVersionNo":
                {
                    _ = Convert.ToUInt32(textBox.Text, CultureInfo.InvariantCulture);
                    break;
                }
                
                // IntProperty
                case "TextBoxWaccaPointCost":
                case "TextBoxMusicTagForUnlock0":
                case "TextBoxMusicTagForUnlock1":
                case "TextBoxMusicTagForUnlock2":
                case "TextBoxMusicTagForUnlock3":
                case "TextBoxMusicTagForUnlock4":
                case "TextBoxMusicTagForUnlock5":
                case "TextBoxMusicTagForUnlock6":
                case "TextBoxMusicTagForUnlock7":
                case "TextBoxMusicTagForUnlock8":
                case "TextBoxMusicTagForUnlock9":
                {
                    _ = Convert.ToInt32(textBox.Text, CultureInfo.InvariantCulture);
                    break;
                }
                
                // ByteProperty
                case "TextBoxCollaboration": 
                case "TextBoxWaccaOriginal":
                case "TextBoxTrainingLevel":
                case "TextBoxReserved":
                {
                    _ = Convert.ToByte(textBox.Text, CultureInfo.InvariantCulture);
                    break;
                }
                
                // FloatProperty
                case "TextBoxDifficultyNormalLv":
                case "TextBoxDifficultyHardLv":
                case "TextBoxDifficultyExtremeLv":
                case "TextBoxDifficultyInfernoLv":
                case "TextBoxClearNormaRateNormal":
                case "TextBoxClearNormaRateHard":
                case "TextBoxClearNormaRateExtreme":
                case "TextBoxClearNormaRateInferno":
                case "TextBoxPreviewBeginTime":
                case "TextBoxPreviewSeconds":
                {
                    _ = Convert.ToSingle(textBox.Text, CultureInfo.InvariantCulture);
                    break;
                }
                
                // UInt64Property
                case "TextBoxWorkBuffer":
                {
                    _ = Convert.ToUInt64(textBox.Text, CultureInfo.InvariantCulture);
                    break;
                }
                
            }
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
    }

    private void CheckBox_OnIsCheckedChanged(object? sender, RoutedEventArgs args)
    {
        if (ignoreDataChange) return;
        
        if (asset == null) return;
        if (undoRedoManager == null) return;
        if (explorerView?.SelectedItem == null) return;
        if (sender is not CheckBox checkBox) return;
        
        try
        {
            TreeViewItem item = explorerView.SelectedItem;
            if (item.Tag is not StructPropertyData data) return;
            
            switch (checkBox.Name)
            {
                case "CheckBoxValidCulture_ja_JP":
                {
                    BoolPropertyData boolPropertyData = (BoolPropertyData)data.Value[12];
                    bool oldValue = boolPropertyData.Value;
                    bool newValue = checkBox.IsChecked ?? false;

                    ModifyBoolPropertyDataValue operation = new(data, boolPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "CheckBoxValidCulture_en_US":
                {
                    BoolPropertyData boolPropertyData = (BoolPropertyData)data.Value[13];
                    bool oldValue = boolPropertyData.Value;
                    bool newValue = checkBox.IsChecked ?? false;

                    ModifyBoolPropertyDataValue operation = new(data, boolPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "CheckBoxValidCulture_zh_Hant_TW":
                {
                    BoolPropertyData boolPropertyData = (BoolPropertyData)data.Value[14];
                    bool oldValue = boolPropertyData.Value;
                    bool newValue = checkBox.IsChecked ?? false;

                    ModifyBoolPropertyDataValue operation = new(data, boolPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "CheckBoxValidCulture_en_HK":
                {
                    BoolPropertyData boolPropertyData = (BoolPropertyData)data.Value[15];
                    bool oldValue = boolPropertyData.Value;
                    bool newValue = checkBox.IsChecked ?? false;

                    ModifyBoolPropertyDataValue operation = new(data, boolPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "CheckBoxValidCulture_en_SG":
                {
                    BoolPropertyData boolPropertyData = (BoolPropertyData)data.Value[16];
                    bool oldValue = boolPropertyData.Value;
                    bool newValue = checkBox.IsChecked ?? false;

                    ModifyBoolPropertyDataValue operation = new(data, boolPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "CheckBoxValidCulture_ko_KR":
                {
                    BoolPropertyData boolPropertyData = (BoolPropertyData)data.Value[17];
                    bool oldValue = boolPropertyData.Value;
                    bool newValue = checkBox.IsChecked ?? false;

                    ModifyBoolPropertyDataValue operation = new(data, boolPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "CheckBoxValidCulture_h_Hans_CN_Guest":
                {
                    BoolPropertyData boolPropertyData = (BoolPropertyData)data.Value[18];
                    bool oldValue = boolPropertyData.Value;
                    bool newValue = checkBox.IsChecked ?? false;

                    ModifyBoolPropertyDataValue operation = new(data, boolPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "CheckBoxValidCulture_h_Hans_CN_GeneralMember":
                {
                    BoolPropertyData boolPropertyData = (BoolPropertyData)data.Value[19];
                    bool oldValue = boolPropertyData.Value;
                    bool newValue = checkBox.IsChecked ?? false;

                    ModifyBoolPropertyDataValue operation = new(data, boolPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "CheckBoxValidCulture_h_Hans_CN_VipMember":
                {
                    BoolPropertyData boolPropertyData = (BoolPropertyData)data.Value[20];
                    bool oldValue = boolPropertyData.Value;
                    bool newValue = checkBox.IsChecked ?? false;

                    ModifyBoolPropertyDataValue operation = new(data, boolPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "CheckBoxValidCulture_Offline":
                {
                    BoolPropertyData boolPropertyData = (BoolPropertyData)data.Value[21];
                    bool oldValue = boolPropertyData.Value;
                    bool newValue = checkBox.IsChecked ?? false;

                    ModifyBoolPropertyDataValue operation = new(data, boolPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "CheckBoxValidCulture_NoneActive":
                {
                    BoolPropertyData boolPropertyData = (BoolPropertyData)data.Value[22];
                    bool oldValue = boolPropertyData.Value;
                    bool newValue = checkBox.IsChecked ?? false;

                    ModifyBoolPropertyDataValue operation = new(data, boolPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
                
                case "CheckBoxRecommend":
                {
                    BoolPropertyData boolPropertyData = (BoolPropertyData)data.Value[23];
                    bool oldValue = boolPropertyData.Value;
                    bool newValue = checkBox.IsChecked ?? false;

                    ModifyBoolPropertyDataValue operation = new(data, boolPropertyData, oldValue, newValue);
                    undoRedoManager.RedoAndPush(operation);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MainView.ShowWarningMessage("An Error has occurred.", e.Message);
        }
    }
    
    private void TreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs args) => UpdateContent(true);

    private void TextBoxSearch_OnTextChanging(object? sender, TextChangingEventArgs args) => SearchContent();
    private void ToggleSearch_OnIsCheckedChanged(object? sender, RoutedEventArgs args) => SearchContent();
    private void ToggleMatchCase_OnIsCheckedChanged(object? sender, RoutedEventArgs args) => SearchContent();
    private void ToggleInvertQuery_OnIsCheckedChanged(object? sender, RoutedEventArgs args) => SearchContent();
    
    private void ButtonSave_OnClick(object? sender, RoutedEventArgs args) => Save();
    private void ButtonOpen_OnClick(object? sender, RoutedEventArgs args) => Open();
    private void ButtonUndo_OnClick(object? sender, RoutedEventArgs args) => Undo();
    private void ButtonRedo_OnClick(object? sender, RoutedEventArgs args) => Redo();
    private void ButtonMoveElementUp_OnClick(object? sender, RoutedEventArgs args) => MoveElement(ElementMoveDirection.Up);
    private void ButtonMoveElementDown_OnClick(object? sender, RoutedEventArgs args) => MoveElement(ElementMoveDirection.Down);
    private void ButtonAddElement_OnClick(object? sender, RoutedEventArgs args) => AddElement();
    private void ButtonDuplicateElement_OnClick(object? sender, RoutedEventArgs args) => DuplicateElement();
    private void ButtonDeleteElement_OnClick(object? sender, RoutedEventArgs args) => DeleteElement();
}