using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image fishIconImage;
    public TextMeshProUGUI fishNameText;

    public void SetSlot(FishData fish)
    {
        if (fish != null)
        {
            fishIconImage.sprite = fish.fishIcon;
            fishIconImage.enabled = true;
            fishNameText.text = fish.fishName;
        }
        else
        {
            fishIconImage.enabled = false;
            fishNameText.text = "";
        }
    }
}