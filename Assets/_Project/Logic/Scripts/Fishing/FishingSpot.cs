using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingSpot : MonoBehaviour
{
    [Header("Minigame Reference")]
    public CircularFishingMinigame minigameScript;

    [Header("Fish Database")]
    public List<FishData> possibleFishes = new List<FishData>();

    [Header("Settings")]
    public float minWaitTime = 2f;
    public float maxWaitTime = 5f;

    private bool isPlayerInZone = false;
    private bool isFishing = false;
    private FishData pendingFish;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            Debug.Log("Tekan [E] untuk mulai memancing!");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            isFishing = false;
            StopAllCoroutines();
            Debug.Log("Keluar dari zona memancing.");
        }
    }

    private void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.E) && !isFishing)
        {
            StartCoroutine(StartFishingRoutine());
        }
    }

    private IEnumerator StartFishingRoutine()
    {
        isFishing = true;
        Debug.Log("Melempar kail... Menunggu ikan menggigit...");

        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        if (isFishing && possibleFishes.Count > 0)
        {
            // Pick ikan acak yang akan ditangkap
            int randomIndex = Random.Range(0, possibleFishes.Count);
            pendingFish = possibleFishes[randomIndex];

            Debug.Log($"HOOK! Ikan {pendingFish.fishName} menyambar kail! Mulai tarik!");

            // Panggil Minigame Melingkar
            if (minigameScript != null)
            {
                minigameScript.StartMinigame();
            }
        }
    }

    // Dipanggil oleh CircularFishingMinigame saat minigame selesai
    public void OnMinigameCompleted(bool success)
    {
        if (success && pendingFish != null)
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AddFish(pendingFish);
            }
            Debug.Log($"BERHASIL! {pendingFish.fishName} masuk ke inventaris!");
        }
        else
        {
            Debug.Log("Gagal! Ikan lepas dari kail!");
        }

        pendingFish = null;
        isFishing = false;
    }
}