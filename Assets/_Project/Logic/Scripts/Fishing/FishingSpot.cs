using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingSpot : MonoBehaviour
{
    [Header("Fish Database")]
    public List<FishData> possibleFishes = new List<FishData>();

    [Header("Settings")]
    public float minWaitTime = 2f;
    public float maxWaitTime = 5f;

    private bool isPlayerInZone = false;
    private bool isFishing = false;

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
            // Ambil ikan acak dari daftar
            int randomIndex = Random.Range(0, possibleFishes.Count);
            FishData caughtFish = possibleFishes[randomIndex];

            // Masukkan ke inventaris
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AddFish(caughtFish);
            }

            Debug.Log($"HOOOK! Kamu mendapatkan: {caughtFish.fishName}!");
        }

        isFishing = false;
    }
}