using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject inventoryPanel;
    public TextMeshProUGUI fishListText;

    private bool isPanelOpen = false;

    private void Update()
    {
        // Tekan tombol Tab untuk buka/tutup inventaris
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isPanelOpen = !isPanelOpen;
        inventoryPanel.SetActive(isPanelOpen);

        if (isPanelOpen)
        {
            UpdateInventoryDisplay();
        }
    }

    private void UpdateInventoryDisplay()
    {
        if (InventoryManager.Instance == null || fishListText == null) return;

        var fishes = InventoryManager.Instance.caughtFishes;

        if (fishes.Count == 0)
        {
            fishListText.text = "Inventaris Kosong";
            return;
        }

        string displayText = "<b>=== IKAN TANGKAPAN ===</b>\n\n";
        foreach (var fish in fishes)
        {
            displayText += $"- {fish.fishName} ({fish.rarity}) | Harga: ${fish.basePrice}\n";
        }

        fishListText.text = displayText;
    }
}