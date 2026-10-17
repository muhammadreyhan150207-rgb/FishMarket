using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Panel Settings")]
    public GameObject inventoryPanel;

    [Header("Slot Grid Settings")]
    public Transform slotContainer;
    public GameObject slotPrefab;

    private bool isPanelOpen = false;

    private void Start()
    {
        // Pastikan panel tertutup saat game dimulai
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

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
        if (slotContainer == null || slotPrefab == null) return;

        // Bersihkan slot lama agar tidak menumpuk
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        if (InventoryManager.Instance == null) return;

        // Buat slot baru sesuai jumlah ikan di inventory
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