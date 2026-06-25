using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HEditor.Models;

// ─── Enums (Unity GameEvent.cs와 동일) ────────────────────────────────────────

[JsonConverter(typeof(StringEnumConverter))]
public enum GameDataType { Npc, Item, Fact, Flag, Party, Battle }

[JsonConverter(typeof(StringEnumConverter))]
public enum ConditionOperator { Equal, NotEqual, Greater, GreaterEqual, Less, LessEqual }

[JsonConverter(typeof(StringEnumConverter))]
public enum EffectOperator { Add, Subtract, Assign }

[JsonConverter(typeof(StringEnumConverter))]
public enum ContentItemType { Content, Dialogue, Image }

[JsonConverter(typeof(StringEnumConverter))]
public enum NextEventMode { Specific, Conditional, Random }

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

    public EventContentItem Clone() => new()
    {
        Type = Type, TextKey = TextKey,
        SpeakerNameKey = SpeakerNameKey, ExpressionPath = ExpressionPath,
        ImagePath = ImagePath
    };
}

// ─── EventSelection ───────────────────────────────────────────────────────────

public class EventSelection
{
    public string SelectionTextKey { get; set; } = "";
    public string ResultTextKey { get; set; } = "";
    public List<EventEffect> Effects { get; set; } = new();
    public long BattleVictoryEventKey { get; set; }
    public long BattleDefeatEventKey { get; set; }

    public EventSelection Clone() => new()
    {
        SelectionTextKey = SelectionTextKey,
        ResultTextKey = ResultTextKey,
        Effects = Effects.Select(e => e.Clone()).ToList(),
        BattleVictoryEventKey = BattleVictoryEventKey,
        BattleDefeatEventKey = BattleDefeatEventKey
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
    public long MonsterGroupKey { get; set; }
}
