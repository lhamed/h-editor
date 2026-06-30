namespace HEditor.Models;

/// <summary>
/// EXPORTED.bytes의 최상위 컨테이너.
/// UTF-8 JSON으로 직렬화되어 .bytes 파일로 저장됩니다.
/// </summary>
public class ExportData
{
    public int Version { get; set; } = 1;
    public List<GameEventData> GameEvents { get; set; } = new();
    public List<GameItemData> GameItems { get; set; } = new();
    public List<GameUnitData> GameUnits { get; set; } = new();
    public List<MonsterData> MonsterDatas { get; set; } = new();
    public List<DiceDataModel> DiceDatas { get; set; } = new();
    public List<MonsterGroupData> MonsterGroups { get; set; } = new();
    public List<BattleDataModel> BattleDatas { get; set; } = new();

    /// <summary>
    /// M.bytes와 동일한 형식의 로컬라이제이션 데이터.
    /// null이면 에디터에서 텍스트 미리보기 불가.
    /// </summary>
    public Dictionary<string, object>? Localization { get; set; }
}
