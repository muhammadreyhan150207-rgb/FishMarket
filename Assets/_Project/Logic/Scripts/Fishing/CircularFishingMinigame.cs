using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CircularFishingMinigame : MonoBehaviour
{
    public enum FishingState
    {
        Idle,
        WaitingForBite, 
        FishHooked,     
        MinigameActive, 
        Success,
        Failed
    }

    [Header("UI References")]
    public GameObject minigamePanel;
    public RectTransform circularTrack; 
    public RectTransform catchZone;     
    public RectTransform fishIcon;      
    public Image progressBarImage;

    [Header("Fish Database & Spawn Rates")]
    public List<FishData> allFishList; // Taruh asset FishData kamu di Inspector
    
    [Range(0f, 100f)] public float commonChance = 80f;
    [Range(0f, 100f)] public float rareChance = 50f;
    [Range(0f, 100f)] public float uniqueChance = 10f;

    [Header("Success UI Panel (Congrats Popup)")]
    public GameObject successPanel;          // Panel pop-up hasil tangkapan
    public TMP_Text txtFishName;             // Text untuk Teks Congrats & Nama Ikan
    public Image imgCaughtFishSprite;        // Image untuk Menampilkan Gambar Ikan
    private FishData caughtFishResult;       // Menampung data ikan yang didapat

    [Header("Track Settings")]
    public float trackPadding = 10f; 

    [Header("Fish Movement Settings")]
    public float slowSpeed = 40f;
    public float fastSpeed = 180f;
    public float directionChangeInterval = 0.8f;

    [Header("Catch Zone Settings")]
    public float catchZoneRadius = 15f;

    [Header("Progress Balancing")]
    public float baseFillSpeed = 0.35f;
    public float baseDrainSpeed = 0.25f;

    [Header("Hook Timing Settings")]
    public float minWaitTime = 2f;
    public float maxWaitTime = 5f;
    public float hookWindowTime = 3f; 

    private FishingState currentState = FishingState.Idle;
    private float fishAngle = 0f;
    private float currentFishSpeed;
    private float speedTimer;
    private float catchProgress = 0f;
    private bool isPlayerInRange = false;
    private Coroutine fishingCoroutine;

    void Start()
    {
        if (minigamePanel != null) minigamePanel.SetActive(false);
        if (successPanel != null) successPanel.SetActive(false);
    }

    void Update()
    {
        switch (currentState)
        {
            case FishingState.Idle:
                if (isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
                {
                    StartFishingSequence();
                }
                break;

            case FishingState.FishHooked:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (fishingCoroutine != null) StopCoroutine(fishingCoroutine);
                    StartMinigame();
                }
                break;

            case FishingState.MinigameActive:
                UpdateCatchZonePositionWithMouse();
                UpdateFishMovement();
                CheckCatchStatus();
                UpdateProgressBar();
                break;
        }
    }

    public void StartFishingSequence()
    {
        if (currentState != FishingState.Idle) return;

        currentState = FishingState.WaitingForBite;
        Debug.Log("Melempar kail... Menunggu ikan menggigit...");
        fishingCoroutine = StartCoroutine(WaitBetweenBite());
    }

    private IEnumerator WaitBetweenBite()
    {
        float waitDuration = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitDuration);

        // Pilih jenis ikan acak berdasarkan persentase rarity saat menggigit
        DecideHookedFish();

        currentState = FishingState.FishHooked;
        Debug.Log("HOOK! Ikan menggigit kail! Tekan [SPACE] sekarang dalam 3 detik!");

        float timer = 0f;
        while (timer < hookWindowTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (currentState == FishingState.FishHooked)
        {
            Debug.Log("Ikan terlepas! Terlambat menekan Space.");
            EndFishing(FishingState.Failed);
        }
    }

    // --- STRATEGI PENENTUAN RARITY IKAN (80% Common, 50% Rare, 10% Unique) 
    private void DecideHookedFish()
    {
        // 1. Cek apakah list null atau kosong
        if (allFishList == null || allFishList.Count == 0)
        {
            Debug.LogWarning("All Fish List masih kosong di Inspector!");
            return;
        }

        // 2. Bersihkan list dari item null/kosong secara otomatis
        List<FishData> validFishList = allFishList.FindAll(fish => fish != null);

        if (validFishList.Count == 0)
        {
            Debug.LogError("Tidak ada asset FishData yang valid di dalam list!");
            return;
        }

        // 3. Roll persentase Rarity (10% Unique, 50% Rare, 80% Common)
        float roll = Random.Range(0f, 100f);
        FishRarity chosenRarity = FishRarity.Common;

        if (roll <= uniqueChance)
        {
            chosenRarity = FishRarity.Unique;
        }
        else if (roll <= rareChance)
        {
            chosenRarity = FishRarity.Rare;
        }
        else
        {
            chosenRarity = FishRarity.Common;
        }

        // 4. Filter ikan berdasarkan rarity DAN pastikan fish tidak null
        List<FishData> eligibleFish = validFishList.FindAll(fish => fish != null && fish.rarity == chosenRarity);

        // Jika jenis rarity tersebut belum ada ikannya di list, pakai semua ikan valid yang ada
        if (eligibleFish.Count == 0)
        {
            eligibleFish = validFishList;
        }

        // 5. Pilih 1 ikan acak dari list hasil filter
        caughtFishResult = eligibleFish[Random.Range(0, eligibleFish.Count)];

        if (caughtFishResult != null)
        {
            fastSpeed = caughtFishResult.movementSpeed > 0 ? caughtFishResult.movementSpeed : fastSpeed;
            Debug.Log($"Ikan terpancing: {caughtFishResult.fishName} ({caughtFishResult.rarity})");
        }
    }

    public void StartMinigame()
    {
        currentState = FishingState.MinigameActive;
        catchProgress = 0.25f; 
        fishAngle = Random.Range(0f, 360f);
        currentFishSpeed = slowSpeed;

        if (minigamePanel != null)
            minigamePanel.SetActive(true);

        Debug.Log("Minigame dimulai!");
    }

    private void UpdateCatchZonePositionWithMouse()
    {
        if (circularTrack == null || catchZone == null) return;

        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            circularTrack,
            Input.mousePosition,
            null,
            out localMousePos
        );

        float maxRadius = (circularTrack.rect.width / 2f) - trackPadding;

        if (localMousePos.magnitude > maxRadius)
        {
            catchZone.anchoredPosition = localMousePos.normalized * maxRadius;
        }
        else
        {
            catchZone.anchoredPosition = localMousePos;
        }
    }

    private void UpdateFishMovement()
    {
        speedTimer += Time.deltaTime;
        if (speedTimer >= directionChangeInterval)
        {
            currentFishSpeed = (Random.value > 0.5f) ? fastSpeed : -fastSpeed;
            if (Random.value > 0.7f) currentFishSpeed *= -1f;
            speedTimer = 0f;
        }

        fishAngle += currentFishSpeed * Time.deltaTime;
        fishAngle %= 360f;

        float rad = fishAngle * Mathf.Deg2Rad;
        float currentTrackMaxRadius = (circularTrack.rect.width / 2f) - trackPadding;
        float dynamicFishRadius = currentTrackMaxRadius * Mathf.Abs(Mathf.Sin(Time.time * 1.5f)) * 0.8f + (currentTrackMaxRadius * 0.2f);

        Vector2 fishPos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * dynamicFishRadius;
        fishIcon.anchoredPosition = fishPos;
    }

    private void CheckCatchStatus()
    {
        if (catchZone == null || fishIcon == null) return;

        float distance = Vector2.Distance(catchZone.anchoredPosition, fishIcon.anchoredPosition);
        bool isOverlapping = distance <= catchZoneRadius;

        float currentFillSpeed = baseFillSpeed;
        float currentDrainSpeed = baseDrainSpeed;

        if (catchProgress < 0.2f)
        {
            currentDrainSpeed *= 0.4f;
            currentFillSpeed *= 1.2f;
        }
        else if (catchProgress > 0.8f)
        {
            currentFillSpeed *= 0.65f;
        }

        if (isOverlapping)
        {
            catchProgress += currentFillSpeed * Time.deltaTime;
        }
        else
        {
            catchProgress -= currentDrainSpeed * Time.deltaTime;
        }

        catchProgress = Mathf.Clamp01(catchProgress);

        if (catchProgress >= 1f)
        {
            EndFishing(FishingState.Success);
        }
        else if (catchProgress <= 0f)
        {
            EndFishing(FishingState.Failed);
        }
    }

    private void UpdateProgressBar()
    {
        if (progressBarImage != null)
        {
            progressBarImage.fillAmount = catchProgress;
        }
    }

    public void EndFishing(FishingState resultState)
    {
        currentState = resultState;

        if (fishingCoroutine != null) StopCoroutine(fishingCoroutine);
        if (minigamePanel != null) minigamePanel.SetActive(false);

        if (resultState == FishingState.Success)
        {
            Debug.Log("Hooray! Ikan Berhasil Ditangkap!");
            ShowSuccessUI();
        }
        else if (resultState == FishingState.Failed)
        {
            Debug.Log("Sayang sekali, Ikan Berhasil Kabur!");
            Invoke(nameof(ResetToIdle), 1.5f);
        }
    }

    private void ShowSuccessUI()
    {
        if (successPanel != null)
        {
            successPanel.SetActive(true);
            
            // JALANKAN ANIMASI BOUNCE DI SINI
            StopAllCoroutines();
            StartCoroutine(AnimatePopupBounce());

            if (txtFishName != null && caughtFishResult != null)
            {
                txtFishName.text = "CONGRATS!\nYou caught a " + caughtFishResult.fishName + " (" + caughtFishResult.rarity.ToString() + ")";
            }

            if (imgCaughtFishSprite != null && caughtFishResult != null)
            {
                imgCaughtFishSprite.gameObject.SetActive(true);
                imgCaughtFishSprite.sprite = caughtFishResult.fishIcon;
            }
        }
    }

    // Panggil fungsi ini lewat OnClick Button pada SuccessPanel/Layar
    public void OnClickCloseSuccessPanel()
    {
        if (successPanel != null)
        {
            successPanel.SetActive(false);
        }

        ResetToIdle();
    }

    private void ResetToIdle()
    {
        currentState = FishingState.Idle;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (currentState != FishingState.Idle)
            {
                EndFishing(FishingState.Failed);
            }
        }
    }
    // --- EFEK ANIMASI POP-UP BOUNCE ---
    private IEnumerator AnimatePopupBounce()
    {
        float timer = 0f;
        float duration = 0.25f; // Durasi animasi dalam detik
        
        if (successPanel != null)
        {
            successPanel.transform.localScale = Vector3.zero;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float progress = timer / duration;
                float scale = Mathf.Sin(progress * Mathf.PI * 0.5f);
                
                successPanel.transform.localScale = Vector3.one * scale;
                yield return null;
            }

            successPanel.transform.localScale = Vector3.one;
        }
    }
}