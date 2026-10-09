using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Panel Settings")]
    public GameObject inventoryPanel;

    [Header("Slot Grid Settings")]
    public Transform slotContainer; // Drag 'InventoryPanel' (tempat Grid Layout Group) ke sini
    public GameObject slotPrefab;   // Drag Prefab 'InventorySlotUI' ke sini

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
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isPanelOpen);
        }

        if (isPanelOpen)
        {
            UpdateInventoryDisplay();
        }
    }

    public void UpdateInventoryDisplay()
    {
        if (InventoryManager.Instance == null || slotContainer == null || slotPrefab == null) return;

        // Bersihkan slot lama
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        // Spawn slot gambar baru berdasarkan isi tas
        foreach (FishData fish in InventoryManager.Instance.caughtFishes)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotContainer);
            InventorySlotUI slotScript = newSlot.GetComponent<InventorySlotUI>();

            if (slotScript != null)
            {
                slotScript.SetSlot(fish);
            }
        }
    }
}