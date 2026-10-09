using UnityEngine;

[CreateAssetMenu(fileName = "NewFishData", menuName = "FishMarket/Fish Data")]
public class FishData : ScriptableObject
{
    [Header("Basic Info")]
    public string fishName = "Ikan Baru";
    [TextArea] public string description = "Deskripsi ikan...";
    public Sprite fishIcon;

    [Header("Market & Rarity")]
    public FishRarity rarity = FishRarity.Common;
    public int basePrice = 100;

    [Header("Mini-Game Settings")]
    [Tooltip("Kecepatan gerak ikan saat mini-game memancing")]
    public float movementSpeed = 2f;

    [Tooltip("Tingkat kesulitan ikan (pengali seberapa cepat progress bar berkurang)")]
    public float catchDifficulty = 1f;
}