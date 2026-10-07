using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Data (Prototype)")]
    public int playerCoins = 1000;
    public int currentDay = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Agar objek ini tidak hancur saat pindah scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCoins(int amount)
    {
        playerCoins += amount;
        Debug.Log("Uang Bertambah! Total Uang: " + playerCoins);
    }

    public bool SpendCoins(int amount)
    {
        if (playerCoins >= amount)
        {
            playerCoins -= amount;
            Debug.Log("Uang Berkurang! Sisa Uang: " + playerCoins);
            return true;
        }
        Debug.Log("Uang Tidak Cukup!");
        return false;
    }
}