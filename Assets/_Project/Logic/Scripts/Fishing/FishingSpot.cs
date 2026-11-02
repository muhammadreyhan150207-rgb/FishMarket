using UnityEngine;

public class FishingSpot : MonoBehaviour
{
    [Header("Reference ke Minigame")]
    public CircularFishingMinigame minigameScript; // Masukkan objek yang punya script minigame di Inspector

    [Header("Fish Database")]
    public ScriptableObject[] possibleFishes; // Database ikanmu

    private bool isPlayerInZone = false;

    void Update()
    {
        // Player menekan Space di dalam zona untuk mulai memancing
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.Space))
        {
            if (minigameScript != null)
            {
                minigameScript.StartFishingSequence();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            Debug.Log("Tekan [SPACE] untuk mulai memancing!"); // Pastikan sudah [SPACE], bukan [E]
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
        }
    }
}