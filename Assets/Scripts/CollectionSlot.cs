using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to the CollectionSlot prefab.
/// 
/// Prefab hierarchy expected:
///   CollectionSlot (this script + Image for rarity border)
///   ├── PrizeImage       (Image)
///   ├── CountBadge       (GameObject)
///   │   └── CountText    (TextMeshProUGUI)  — shown only when count > 1
///   └── NameText         (TextMeshProUGUI)
/// </summary>
public class CollectionSlot : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image prizeImage;
    [SerializeField] private Image borderImage;         // The slot background/border
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject countBadge;     // Root object for the badge
    [SerializeField] private TextMeshProUGUI countText;

    [Header("Rarity Colors")]
    [SerializeField] private Color commonColor     = new Color(0.75f, 0.75f, 0.75f);
    [SerializeField] private Color uncommonColor   = new Color(0.40f, 0.80f, 0.40f);
    [SerializeField] private Color rareColor       = new Color(0.30f, 0.55f, 1.00f);
    [SerializeField] private Color legendaryColor  = new Color(1.00f, 0.80f, 0.15f);

    // ── Public API ────────────────────────────────────────────────────────────

    /// Populate this slot with prize data and a collected count.
    public void Populate(PrizeData prize, int count)
    {
        if (prize == null)
        {
            gameObject.SetActive(false);
            return;
        }

        // Sprite
        prizeImage.sprite = prize.sprite;
        prizeImage.preserveAspect = true;

        // Name
        nameText.text = prize.displayName;

        // Rarity border color
        borderImage.color = RarityToColor(prize.rarity);

        // Count badge — only visible when there are duplicates
        bool hasDupes = count > 1;
        countBadge.SetActive(hasDupes);
        if (hasDupes)
            countText.text = $"x{count}";
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private Color RarityToColor(Rarity rarity) => rarity switch
    {
        Rarity.Common     => commonColor,
        Rarity.Uncommon   => uncommonColor,
        Rarity.Rare       => rareColor,
        Rarity.Legendary  => legendaryColor,
        _                 => commonColor
    };
}
