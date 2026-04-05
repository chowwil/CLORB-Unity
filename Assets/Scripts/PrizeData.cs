using UnityEngine;

public enum Rarity { Common, Uncommon, Rare, Legendary }

[CreateAssetMenu(fileName = "NewPrize", menuName = "Clorb/Prize Data")]
public class PrizeData : ScriptableObject
{
    [Header("Identity")]
    public string prizeID;          // Unique key used in save data (e.g. "sock_argyle")
    public string displayName;
    [TextArea] public string description;

    [Header("Visuals")]
    public Sprite sprite;
    public Rarity rarity;

    [Header("Coupon")]
    // Optional: prizes can be flagged as bonus coupon progress
    public int couponProgressValue = 1;
}
