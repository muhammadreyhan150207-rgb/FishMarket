using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Player Inventory")]
    public List<FishData> caughtFishes = new List<FishData>();
    public int maxCapacity = 10;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool AddFish(FishData fish)
    {
        if (caughtFishes.Count >= maxCapacity)
        {
            Debug.LogWarning("[INVENTORY] Tas Penuh! Tidak bisa menyimpan ikan lagi.");
            return false;
        }

        caughtFishes.Add(fish);
        Debug.Log($"[INVENTORY] Berhasil menangkap: {fish.fishName} ({fish.rarity})!");
        return true;
    }

    public void RemoveFish(FishData fish)
    {
        if (caughtFishes.Contains(fish))
        {
            caughtFishes.Remove(fish);
        }
    }
}