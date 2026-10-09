using UnityEngine;
using UnityEngine.UI;

public class CircularFishingMinigame : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject minigamePanel;
    public RectTransform catchZone;
    public RectTransform fishIcon;
    public Image progressBar;

    [Header("Minigame Settings")]
    public float radius = 100f;          // Jarak radius lingkaran
    public float zoneSize = 45f;         // Ukuran area hijau (derajat)
    public float catchSpeed = 50f;       // Kecepatan putar player
    public float fishSpeed = 80f;        // Kecepatan putar ikan
    
    private float currentZoneAngle = 0f;
    private float currentFishAngle = 0f;
    private float catchProgress = 0.3f;  // Mulai dari 30%
    private bool isMinigameActive = false;

    public void StartMinigame()
    {
        minigamePanel.SetActive(true);
        isMinigameActive = true;
        catchProgress = 0.3f;
        currentZoneAngle = 0f;
        currentFishAngle = Random.Range(0f, 360f);
    }

    void Update()
    {
        if (!isMinigameActive) return;

        HandlePlayerInput();
        MoveFish();
        CheckCatchLogic();
        UpdatePositions();
    }

    void HandlePlayerInput()
    {
        // Tahan spasi / klik kiri mouse untuk memutar Catch Zone
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
        {
            currentZoneAngle += catchSpeed * Time.deltaTime * 100f;
        }
        else
        {
            // Melambat / mundur sedikit saat tombol dilepas
            currentZoneAngle -= catchSpeed * 0.5f * Time.deltaTime * 100f;
        }

        currentZoneAngle %= 360f;
    }

    void MoveFish()
    {
        // Pergerakan acak ikan melingkar
        float randomMovement = Mathf.PerlinNoise(Time.time * 2f, 0f) - 0.5f;
        currentFishAngle += randomMovement * fishSpeed * Time.deltaTime * 100f;
        currentFishAngle %= 360f;
    }

    void CheckCatchLogic()
    {
        // Hitung selisih sudut antara ikan dan area hijau
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentZoneAngle, currentFishAngle));

        if (angleDifference <= zoneSize / 2f)
        {
            // Ikan ada di dalam area hijau -> Progress bertambah
            catchProgress += Time.deltaTime * 0.25f;
        }
        else
        {
            // Ikan di luar area hijau -> Progress berkurang
            catchProgress -= Time.deltaTime * 0.15f;
        }

        catchProgress = Mathf.Clamp01(catchProgress);

        if (progressBar != null)
            progressBar.fillAmount = catchProgress;

        // Cek kondisi Menang / Kalah
        if (catchProgress >= 1f)
        {
            CompleteMinigame(true);
        }
        else if (catchProgress <= 0f)
        {
            CompleteMinigame(false);
        }
    }

    void UpdatePositions()
    {
        // Konversi sudut derajat ke posisi posisi 2D X dan Y melingkar
        float zoneRad = currentZoneAngle * Mathf.Deg2Rad;
        catchZone.anchoredPosition = new Vector2(Mathf.Cos(zoneRad) * radius, Mathf.Sin(zoneRad) * radius);

        float fishRad = currentFishAngle * Mathf.Deg2Rad;
        fishIcon.anchoredPosition = new Vector2(Mathf.Cos(fishRad) * radius, Mathf.Sin(fishRad) * radius);
    }

    void CompleteMinigame(bool success)
    {
        isMinigameActive = false;
        minigamePanel.SetActive(false);

        // Beri tahu FishingSpot bahwa minigame selesai
        FishingSpot fishingSpot = GetComponent<FishingSpot>();
        if (fishingSpot != null)
        {
            fishingSpot.OnMinigameCompleted(success);
        }
    }
}