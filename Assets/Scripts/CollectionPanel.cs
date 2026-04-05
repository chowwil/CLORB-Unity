using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to the root CollectionPanel canvas group.
///
/// Panel hierarchy expected:
///   CollectionPanel
///   ├── ShelfPanel
///   │   ├── ScrollView
///   │   │   └── Content          (GridLayoutGroup here)
///   │   └── CouponProgressBar    (Slider)
///   └── CouponPopup              (initially inactive)
///       ├── CouponCodeText       (TextMeshProUGUI)
///       └── DismissButton        (Button)
/// </summary>
public class CollectionPanel : MonoBehaviour
{
    [Header("Shelf")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform gridContent;         // The GridLayoutGroup parent
    [SerializeField] private Slider couponProgressBar;      // 0–5 visual progress

    [Header("Coupon Popup")]
    [SerializeField] private GameObject couponPopup;
    [SerializeField] private TextMeshProUGUI couponCodeText;
    [SerializeField] private Button dismissButton;

    [Header("Animation")]
    [SerializeField] private float panelFadeTime = 0.25f;
    [SerializeField] private CanvasGroup canvasGroup;

    // ── Unity lifecycle ──────────────────────────────────────────────────────

    private void Awake()
    {
        // Wire dismiss button
        dismissButton.onClick.AddListener(DismissCouponPopup);

        // Listen for new prizes and coupon unlocks
        CollectionManager.Instance.OnPrizeAdded.AddListener(OnPrizeAdded);
        CollectionManager.Instance.OnCouponUnlocked.AddListener(OnCouponUnlocked);

        // Start hidden
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (CollectionManager.Instance == null) return;
        CollectionManager.Instance.OnPrizeAdded.RemoveListener(OnPrizeAdded);
        CollectionManager.Instance.OnCouponUnlocked.RemoveListener(OnCouponUnlocked);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// Call from your menu/inventory button.
    public void Open()
    {
        gameObject.SetActive(true);
        RefreshGrid();
        RefreshProgress();
        StartCoroutine(FadeIn());
    }

    /// Call from a close/back button.
    public void Close()
    {
        StartCoroutine(FadeOutAndDeactivate());
    }

    // ── Grid ─────────────────────────────────────────────────────────────────

    private void RefreshGrid()
    {
        // Clear existing slots
        foreach (Transform child in gridContent)
            Destroy(child.gameObject);

        // Repopulate from CollectionManager
        foreach (CollectedEntry entry in CollectionManager.Instance.CollectedEntries)
        {
            PrizeData prize = CollectionManager.Instance.GetPrizeData(entry.prizeID);
            if (prize == null) continue;

            GameObject slotGO = Instantiate(slotPrefab, gridContent);
            CollectionSlot slot = slotGO.GetComponent<CollectionSlot>();
            slot.Populate(prize, entry.count);
        }
    }

    private void RefreshProgress()
    {
        if (couponProgressBar == null) return;
        int progress = CollectionManager.Instance.ProgressInCycle;
        int max = CollectionManager.Instance.itemsPerCoupon;

        couponProgressBar.minValue = 0;
        couponProgressBar.maxValue = max;
        couponProgressBar.value = progress;
    }

    // ── Event handlers ────────────────────────────────────────────────────────

    /// Refresh the shelf whenever a new prize lands (panel may be open or closed).
    private void OnPrizeAdded(PrizeData prize)
    {
        if (gameObject.activeSelf)
        {
            RefreshGrid();
            RefreshProgress();
        }
    }

    private void OnCouponUnlocked(string code)
    {
        // Make sure panel is visible before showing popup
        if (!gameObject.activeSelf) Open();
        ShowCouponPopup(code);
    }

    // ── Coupon Popup ──────────────────────────────────────────────────────────

    private void ShowCouponPopup(string code)
    {
        couponCodeText.text = code;
        couponPopup.SetActive(true);
    }

    private void DismissCouponPopup()
    {
        couponPopup.SetActive(false);
    }

    // ── Animations ────────────────────────────────────────────────────────────

    private IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0f;
        float elapsed = 0f;
        while (elapsed < panelFadeTime)
        {
            canvasGroup.alpha = elapsed / panelFadeTime;
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutAndDeactivate()
    {
        float elapsed = 0f;
        while (elapsed < panelFadeTime)
        {
            canvasGroup.alpha = 1f - (elapsed / panelFadeTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}
