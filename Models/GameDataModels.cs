using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HEditor.Models;

// ─── Enums (Unity GameEvent.cs와 동일) ────────────────────────────────────────

[JsonConverter(typeof(StringEnumConverter))]
public enum GameDataType { Npc, Item, Fact, Flag, Party, Battle, Shop, History, Gold, Event, Stat }

[JsonConverter(typeof(StringEnumConverter))]
public enum ConditionOperator { Equal, NotEqual, Greater, GreaterEqual, Less, LessEqual }

[JsonConverter(typeof(StringEnumConverter))]
public enum EffectOperator { Add, Subtract, Assign }

[JsonConverter(typeof(StringEnumConverter))]
public enum ContentItemType { Content, Dialogue, Image, Flush }

[JsonConverter(typeof(StringEnumConverter))]
public enum NextEventMode { Specific, Conditional, Random }

// ─── EventUIEffectSettings ───────────────────────────────────────────────────

public class EventUIEffectSettings
{
    public bool OverrideEffects { get; set; }

    public bool PanelScale { get; set; }
    public bool PanelScaleWidth { get; set; } = true;
    public bool PanelScaleHeight { get; set; } = true;
    public bool FadeIn { get; set; }
    public bool FadeOut { get; set; }

    public bool ButtonClickScale { get; set; }
    public bool ButtonFadePulse { get; set; }
    public bool ButtonActive { get; set; }

    public string SfxName { get; set; } = "";

    public EventUIEffectSettings Clone() => new()
    {
        OverrideEffects = OverrideEffects,
        PanelScale = PanelScale,
        PanelScaleWidth = PanelScaleWidth,
        PanelScaleHeight = PanelScaleHeight,
        FadeIn = FadeIn,
        FadeOut = FadeOut,
        ButtonClickScale = ButtonClickScale,
        ButtonFadePulse = ButtonFadePulse,
        ButtonActive = ButtonActive,
        SfxName = SfxName
    };
}

// ─── EventCondition ───────────────────────────────────────────────────────────

public class EventCondition
{
    public GameDataType DataType { get; set; }
    public long DataKey { get; set; }
    public ConditionOperator Op { get; set; }
    public long CompareValue { get; set; }

    public EventCondition Clone() => new()
    {
        DataType = DataType, DataKey = DataKey, Op = Op, CompareValue = CompareValue
    };
}

// ─── EventEffect ──────────────────────────────────────────────────────────────

public class EventEffect
{
    public GameDataType DataType { get; set; }
    public long DataKey { get; set; }
    public EffectOperator Op { get; set; }
    public long Value { get; set; }

    public EventEffect Clone() => new()
    {
        DataType = DataType, DataKey = DataKey, Op = Op, Value = Value
    };
}

// ─── EventContentItem ─────────────────────────────────────────────────────────

public class EventContentItem
{
    public ContentItemType Type { get; set; } = ContentItemType.Content;
    public string TextKey { get; set; } = "";
    public string SpeakerNameKey { get; set; } = "";   // Dialogue 전용
    /// <summary>
    /// Dialogue 전용. Unity 스프라이트 에셋 경로 (예: "Assets/Project/Contents/UI/FACES.png")
    /// 또는 스프라이트 이름. 웹 에디터에서는 문자열로 표시/편집하며,
    /// Unity 임포트 시 AssetDatabase.LoadAssetAtPath 로 복원합니다.
    /// </summary>
    public string ExpressionPath { get; set; } = "";   // Dialogue 전용
    public string ImagePath      { get; set; } = "";   // Image 전용
    public List<EventSelection> Selections { get; set; } = new();
    public EventUIEffectSettings UIEffect { get; set; } = new();

    public EventContentItem Clone() => new()
    {
        Type = Type, TextKey = TextKey,
        SpeakerNameKey = SpeakerNameKey, ExpressionPath = ExpressionPath,
        ImagePath = ImagePath,
        Selections = Selections.Select(s => s.Clone()).ToList(),
        UIEffect = UIEffect.Clone()
    };
}

// ─── EventSelection ───────────────────────────────────────────────────────────

public class EventSelection
{
    public string SelectionTextKey { get; set; } = "";
    public string ResultTextKey { get; set; } = "";
    public List<EventEffect> Effects { get; set; } = new();
    public List<EventContentBlock> BranchBlocks { get; set; } = new();
    public long BattleVictoryEventKey { get; set; }
    public long BattleDefeatEventKey { get; set; }
    public EventUIEffectSettings UIEffect { get; set; } = new();

    public EventSelection Clone() => new()
    {
        SelectionTextKey = SelectionTextKey,
        ResultTextKey = ResultTextKey,
        Effects = Effects.Select(e => e.Clone()).ToList(),
        BranchBlocks = BranchBlocks.Select(b => b.Clone()).ToList(),
        BattleVictoryEventKey = BattleVictoryEventKey,
        BattleDefeatEventKey = BattleDefeatEventKey,
        UIEffect = UIEffect.Clone()
    };
}

// ─── EventContentBlock ────────────────────────────────────────────────────────

public class EventContentBlock
{
    public List<EventContentItem> ContentItems { get; set; } = new();
    public List<EventEffect> CommonEffects { get; set; } = new();
    public List<EventSelection> Selections { get; set; } = new();

    public EventContentBlock Clone() => new()
    {
        ContentItems = ContentItems.Select(i => i.Clone()).ToList(),
        CommonEffects = CommonEffects.Select(e => e.Clone()).ToList(),
        Selections = Selections.Select(s => s.Clone()).ToList()
    };
}

// ─── NextEventEntry ───────────────────────────────────────────────────────────

public class NextEventEntry
{
    public List<EventCondition> Conditions { get; set; } = new();
    public long EventKey { get; set; }

    public NextEventEntry Clone() => new()
    {
        Conditions = Conditions.Select(c => c.Clone()).ToList(),
        EventKey = EventKey
    };
}

// ─── GameEventData ────────────────────────────────────────────────────────────

public class GameEventData
{
    public long Key { get; set; }
    public bool ExcludeFromInitialRandomPool { get; set; }
    public List<EventCondition> Conditions { get; set; } = new();
    public List<EventContentBlock> ContentBlocks { get; set; } = new();
    public NextEventMode NextEventMode { get; set; } = NextEventMode.Random;
    public List<long> NextEventKeys { get; set; } = new();
    public List<NextEventEntry> NextEventEntries { get; set; } = new();
}

// ─── GameItemData ─────────────────────────────────────────────────────────────

public class GameItemData
{
    public long Key { get; set; }
    public string NameKey { get; set; } = "";
    public string DescriptionKey { get; set; } = "";
    public bool Stackable { get; set; }
    public bool Usable { get; set; }
    public bool Equippable { get; set; }
    public string EquipmentType { get; set; } = "None";
    public string WeaponType { get; set; } = "None";
    public int AttackPower { get; set; }
    public int MeleeAttackPower { get; set; }
    public int MagazineSize { get; set; }
    public long MagazineItemKey { get; set; }
    public int DefensePower { get; set; }
    public int MaxHpBonus { get; set; }
    public int EvasionBonus { get; set; }
}

// ─── DiceDataModel ────────────────────────────────────────────────────────────

public class DiceDataModel
{
    public long Key { get; set; }
    public int Count { get; set; } = 1;
    public int Faces { get; set; } = 6;
    public List<int> CustomFaces { get; set; } = new();

    [JsonIgnore]
    public string Summary => CustomFaces.Count > 0
        ? $"{Count}개 × [{string.Join(", ", CustomFaces)}] 중 랜덤"
        : $"{Count}d{Faces}  (합산 {Count}~{Count * Faces})";
}

// ─── GameUnitData ─────────────────────────────────────────────────────────────

public class GameUnitData
{
    public long Key { get; set; }
    public string NameKey { get; set; } = "";
    public int MaxHP { get; set; } = 10;
    public int Mental { get; set; } = 1;
    public int Vitality { get; set; } = 1;
    public int Strength { get; set; } = 1;
    public int Dexterity { get; set; } = 1;
    public long AttackDiceKey { get; set; }
    public bool ParticipatesInBattle { get; set; } = true;
    public bool CanEquipItems { get; set; } = true;
    public long WeaponItemKey { get; set; }
    public long AuxiliaryItemKey { get; set; }
    public int WeaponAmmo { get; set; }
    public long ArmorItemKey { get; set; }
    public long LeftHandItemKey  { get; set; }
    public long RightHandItemKey { get; set; }
    public long HeadItemKey      { get; set; }
    public long BodyItemKey      { get; set; }
    public long ShoesItemKey     { get; set; }
    public long Ring1ItemKey     { get; set; }
    public long Ring2ItemKey     { get; set; }
}

// ─── MonsterData ─────────────────────────────────────────────────────────────

public class MonsterData
{
    public long Key { get; set; }
    public string NameKey { get; set; } = "";
    public int MaxHP { get; set; } = 10;
    public int AttackPower { get; set; } = 2;
    public int DefensePower { get; set; }
    public int Evasion { get; set; } = 1;
    public int Dexterity { get; set; } = 1;
    public long AttackDiceKey { get; set; }
}

// ─── MonsterGroupData ─────────────────────────────────────────────────────────

public class MonsterGroupData
{
    public long Key { get; set; }
    public List<long> MemberKeys { get; set; } = new();
}

// ─── BattleDataModel ──────────────────────────────────────────────────────────

public class BattleDataModel
{
    public long Key { get; set; }
    public string BackgroundKey { get; set; } = "";
    public long MonsterGroupKey { get; set; }
}
