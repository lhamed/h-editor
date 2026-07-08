using Microsoft.JSInterop;

namespace HEditor.Services;

public class DraftService
{
    const string DraftKey = "h-editor-exported-draft";
    const string DraftSavedAtKey = "h-editor-exported-draft-saved-at";

    readonly DataStore _store;
    readonly ExportService _exportService;
    readonly IJSRuntime _js;
    string? _lastSavedJson;

    public DraftService(DataStore store, ExportService exportService, IJSRuntime js)
    {
        _store = store;
        _exportService = exportService;
        _js = js;
    }

    public bool HasEditableData =>
        _store.GameEvents.Count > 0 ||
        _store.GameItems.Count > 0 ||
        _store.GameUnits.Count > 0 ||
        _store.MonsterDatas.Count > 0 ||
        _store.DiceDatas.Count > 0 ||
        _store.MonsterGroups.Count > 0 ||
        _store.BattleDatas.Count > 0 ||
        _store.Localization != null;

    public async Task<bool> HasDraftAsync()
        => !string.IsNullOrWhiteSpace(await _js.InvokeAsync<string?>("localStorageGet", DraftKey));

    public async Task<string?> GetSavedAtAsync()
        => await _js.InvokeAsync<string?>("localStorageGet", DraftSavedAtKey);

    public async Task<bool> SaveAsync(bool force = false)
    {
        if (!HasEditableData)
            return false;

        var json = _exportService.ExportJson();
        if (!force && json == _lastSavedJson)
            return false;

        await _js.InvokeVoidAsync("localStorageSet", DraftKey, json);
        await _js.InvokeVoidAsync("localStorageSet", DraftSavedAtKey, DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        _lastSavedJson = json;
        return true;
    }

    public async Task<bool> RestoreAsync()
    {
        var json = await _js.InvokeAsync<string?>("localStorageGet", DraftKey);
        if (string.IsNullOrWhiteSpace(json))
            return false;

        _exportService.ImportJson(json);
        _lastSavedJson = json;
        return true;
    }

    public async Task<bool> RestoreIfEmptyAsync()
    {
        if (HasEditableData)
            return false;

        return await RestoreAsync();
    }

    public async Task ClearAsync()
    {
        await _js.InvokeVoidAsync("localStorageRemove", DraftKey);
        await _js.InvokeVoidAsync("localStorageRemove", DraftSavedAtKey);
        _lastSavedJson = null;
    }
}
