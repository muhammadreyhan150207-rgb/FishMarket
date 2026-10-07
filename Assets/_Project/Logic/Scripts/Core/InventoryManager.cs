using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Player Inventory")]
    public List<FishData> caughtFishes = new List<FishData>();

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

    public void AddFish(FishData fish)
    {
        caughtFishes.Add(fish);
        Debug.Log($"[INVENTORY] Berhasil menangkap: {fish.fishName} ({fish.rarity})!");
    }

    public void RemoveFish(FishData fish)
    {
        if (caughtFishes.Contains(fish))
        {
            caughtFishes.Remove(fish);
        }
    }
}