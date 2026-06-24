using System.Text;
using HEditor.Models;
using Newtonsoft.Json;

namespace HEditor.Services;

/// <summary>
/// EXPORTED.bytes 직렬화 / 역직렬화 담당.
/// </summary>
public class ExportService
{
    private readonly DataStore _store;

    private static readonly JsonSerializerSettings Settings = new()
    {
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore
    };

    public ExportService(DataStore store) => _store = store;

    // ─── Export ──────────────────────────────────────────────────────────────

    public byte[] Export()
    {
        var data = new ExportData
        {
            GameEvents    = _store.GameEvents,
            GameItems     = _store.GameItems,
            GameUnits     = _store.GameUnits,
            DiceDatas     = _store.DiceDatas,
            MonsterGroups = _store.MonsterGroups,
            BattleDatas   = _store.BattleDatas,
            Localization  = _store.Localization
        };
        return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, Settings));
    }

    // ─── Import ──────────────────────────────────────────────────────────────

    public void Import(byte[] bytes)
    {
        string json = Encoding.UTF8.GetString(bytes);
        var data = JsonConvert.DeserializeObject<ExportData>(json)
            ?? throw new InvalidDataException("EXPORTED.bytes 파싱 실패");

        _store.GameEvents    = data.GameEvents    ?? new();
        _store.GameItems     = data.GameItems     ?? new();
        _store.GameUnits     = data.GameUnits     ?? new();
        _store.DiceDatas     = data.DiceDatas     ?? new();
        _store.MonsterGroups = data.MonsterGroups ?? new();
        _store.BattleDatas   = data.BattleDatas   ?? new();
        _store.Localization  = data.Localization;
    }
}
