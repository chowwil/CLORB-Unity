using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Singleton. Owns all collection state: what prizes have been earned,
/// how many total, and whether a coupon unlock should fire.
/// Persists to a JSON file in Application.persistentDataPath.
/// </summary>
public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance { get; private set; }

    // ── Events ──────────────────────────────────────────────────────────────
    /// Fired whenever a prize is added. Passes the PrizeData that was added.
    public UnityEvent<PrizeData> OnPrizeAdded = new();

    /// Fired when the player crosses a coupon threshold (every 5 items).
    /// Passes the coupon code string.
    public UnityEvent<string> OnCouponUnlocked = new();

    // ── Config ───────────────────────────────────────────────────────────────
    [Header("All prizes that exist in the game (assign in Inspector)")]
    public List<PrizeData> allPrizes = new();

    [Tooltip("How many collected items earn one coupon.")]
    public int itemsPerCoupon = 5;

    // ── Internal state (mirrors the save file) ───────────────────────────────
    private CollectionSaveData _saveData = new();

    // Convenience: quick lookup from prizeID → PrizeData SO
    private Dictionary<string, PrizeData> _prizeRegistry = new();

    // ── Accessors ────────────────────────────────────────────────────────────

    /// Returns a read-only view of the collected entries.
    public IReadOnlyList<CollectedEntry> CollectedEntries => _saveData.entries;

    /// Total items collected across all time (drives coupon math).
    public int TotalCollected => _saveData.totalCollected;

    /// How many items into the current coupon cycle (0–4).
    public int ProgressInCycle => _saveData.totalCollected % itemsPerCoupon;

    // ── Unity lifecycle ──────────────────────────────────────────────────────

    private void Awake()
    {
        // Singleton enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildRegistry();
        Load();
    }

    // ── Public API ───────────────────────────────────────────────────────────

    /// Call this from your gacha/reward screen after an item is revealed.
    public void AddPrize(PrizeData prize)
    {
        if (prize == null) return;

        // Find existing entry or create one
        CollectedEntry entry = _saveData.entries.Find(e => e.prizeID == prize.prizeID);
        if (entry == null)
        {
            entry = new CollectedEntry { prizeID = prize.prizeID, count = 0 };
            _saveData.entries.Add(entry);
        }
        entry.count++;

        _saveData.totalCollected++;

        Save();
        OnPrizeAdded.Invoke(prize);

        // Coupon check — fires AFTER the prize event so UI can sequence correctly
        if (_saveData.totalCollected % itemsPerCoupon == 0)
        {
            string code = GenerateCouponCode(_saveData.totalCollected);
            _saveData.couponCodes.Add(code);
            Save();
            OnCouponUnlocked.Invoke(code);
        }
    }

    /// Utility: get count for a specific prize (returns 0 if never collected).
    public int GetCount(string prizeID)
    {
        CollectedEntry entry = _saveData.entries.Find(e => e.prizeID == prizeID);
        return entry?.count ?? 0;
    }

    /// Utility: resolve prizeID → PrizeData SO at runtime.
    public PrizeData GetPrizeData(string prizeID)
    {
        _prizeRegistry.TryGetValue(prizeID, out PrizeData data);
        return data;
    }

    // ── Persistence ──────────────────────────────────────────────────────────

    private string SavePath => Path.Combine(Application.persistentDataPath, "clorb_collection.json");

    private void Save()
    {
        string json = JsonUtility.ToJson(_saveData, prettyPrint: true);
        File.WriteAllText(SavePath, json);
    }

    private void Load()
    {
        if (!File.Exists(SavePath)) return;

        try
        {
            string json = File.ReadAllText(SavePath);
            _saveData = JsonUtility.FromJson<CollectionSaveData>(json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[CollectionManager] Failed to load save: {e.Message}. Starting fresh.");
            _saveData = new CollectionSaveData();
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void BuildRegistry()
    {
        _prizeRegistry.Clear();
        foreach (var prize in allPrizes)
        {
            if (prize == null) continue;
            if (!_prizeRegistry.TryAdd(prize.prizeID, prize))
                Debug.LogWarning($"[CollectionManager] Duplicate prizeID: {prize.prizeID}");
        }
    }

    /// Deterministic but looks random enough for a casual coupon code.
    private string GenerateCouponCode(int totalCollected)
    {
        // Format: CLORB-{timestamp hash}-{totalCollected}
        int hash = Mathf.Abs((int)(DateTime.UtcNow.Ticks % 9999));
        return $"CLORB-{hash:D4}-{totalCollected:D3}";
    }

#if UNITY_EDITOR
    // ── Debug helpers (Editor only) ──────────────────────────────────────────
    [ContextMenu("DEBUG: Add First Prize")]
    private void Debug_AddFirstPrize()
    {
        if (allPrizes.Count > 0) AddPrize(allPrizes[0]);
    }

    [ContextMenu("DEBUG: Wipe Save")]
    private void Debug_WipeSave()
    {
        _saveData = new CollectionSaveData();
        Save();
        Debug.Log("[CollectionManager] Save wiped.");
    }
#endif
}

// ── Save data structures (must be serializable by JsonUtility) ───────────────

[Serializable]
public class CollectionSaveData
{
    public List<CollectedEntry> entries = new();
    public int totalCollected = 0;
    public List<string> couponCodes = new();
}

[Serializable]
public class CollectedEntry
{
    public string prizeID;
    public int count;
}
