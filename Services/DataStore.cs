using HEditor.Models;
using Newtonsoft.Json.Linq;

namespace HEditor.Services;

/// <summary>
/// 앱 전체에서 공유하는 인메모리 데이터 저장소 (싱글톤).
/// </summary>
public class DataStore
{
    public List<GameEventData>    GameEvents    { get; set; } = new();
    public List<GameItemData>     GameItems     { get; set; } = new();
    public List<GameUnitData>     GameUnits     { get; set; } = new();
    public List<MonsterData>      MonsterDatas  { get; set; } = new();
    public List<DiceDataModel>    DiceDatas     { get; set; } = new();
    public List<MonsterGroupData> MonsterGroups { get; set; } = new();
    public List<BattleDataModel>  BattleDatas   { get; set; } = new();

    /// <summary>M.bytes와 동일한 형식. Localization 텍스트 미리보기에 사용.</summary>
    public Dictionary<string, object>? Localization { get; set; }

    public string? StatusMessage { get; set; }
    public bool    StatusIsError { get; set; }

    public void SetStatus(string msg, bool isError = false)
    {
        StatusMessage = msg;
        StatusIsError = isError;
    }

    // ─── 로컬라이제이션 조회 ─────────────────────────────────────────────────

    public string? LookupText(string key, string lang)
    {
        if (Localization == null || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(lang))
            return null;

        foreach (var tableObj in Localization.Values)
        {
            if (tableObj is not JArray arr) continue;
            foreach (var row in arr)
            {
                if (row["Key"]?.ToString() == key)
                    return row[lang]?.ToString();
            }
        }
        return null;
    }

    public void SetLocalizationText(string key, string lang, string value)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(lang))
            return;

        var arr = EnsureLocalizationTable();
        var row = FindLocalizationRow(key);
        if (row == null)
        {
            row = new JObject { ["Key"] = key };
            foreach (var language in GetLanguageColumns())
                row[language] = "";
            arr.Add(row);
        }

        row[lang] = value ?? "";
    }

    public List<string> GetLanguageColumns()
    {
        if (Localization == null) return new();
        foreach (var tableObj in Localization.Values)
        {
            if (tableObj is not JArray arr || arr.Count == 0) continue;
            if (arr[0] is not JObject obj) continue;
            return obj.Properties()
                .Select(p => p.Name)
                .Where(n => n != "Key")
                .ToList();
        }
        return new();
    }

    JArray EnsureLocalizationTable()
    {
        Localization ??= new Dictionary<string, object>();

        if (Localization.TryGetValue("Localization", out var tableObj) && tableObj is JArray localizationArr)
            return localizationArr;

        foreach (var value in Localization.Values)
        {
            if (value is JArray arr)
            {
                Localization["Localization"] = arr;
                return arr;
            }
        }

        var created = new JArray();
        Localization["Localization"] = created;
        return created;
    }

    JObject? FindLocalizationRow(string key)
    {
        if (Localization == null)
            return null;

        foreach (var tableObj in Localization.Values)
        {
            if (tableObj is not JArray arr) continue;
            foreach (var row in arr.OfType<JObject>())
            {
                if (row["Key"]?.ToString() == key)
                    return row;
            }
        }

        return null;
    }

    // ─── 키 중복 검사 유틸 ───────────────────────────────────────────────────

    public bool HasDuplicateEventKey(long key, GameEventData self)
        => GameEvents.Count(e => e.Key == key && e != self) > 0;

    public bool HasDuplicateItemKey(long key, GameItemData self)
        => GameItems.Count(e => e.Key == key && e != self) > 0;

    public bool HasDuplicateUnitKey(long key, GameUnitData self)
        => GameUnits.Count(e => e.Key == key && e != self) > 0;

    public bool HasDuplicateMonsterKey(long key, MonsterData self)
        => MonsterDatas.Count(e => e.Key == key && e != self) > 0;

    public bool HasDuplicateDiceKey(long key, DiceDataModel self)
        => DiceDatas.Count(e => e.Key == key && e != self) > 0;

    public bool HasDuplicateGroupKey(long key, MonsterGroupData self)
        => MonsterGroups.Count(e => e.Key == key && e != self) > 0;

    public bool HasDuplicateBattleKey(long key, BattleDataModel self)
        => BattleDatas.Count(e => e.Key == key && e != self) > 0;

    // ─── 조회 유틸 ───────────────────────────────────────────────────────────

    public GameUnitData? GetUnit(long key) => GameUnits.FirstOrDefault(u => u.Key == key);
    public MonsterData? GetMonster(long key) => MonsterDatas.FirstOrDefault(m => m.Key == key);
    public DiceDataModel? GetDice(long key) => DiceDatas.FirstOrDefault(d => d.Key == key);
    public MonsterGroupData? GetGroup(long key) => MonsterGroups.FirstOrDefault(g => g.Key == key);
}
