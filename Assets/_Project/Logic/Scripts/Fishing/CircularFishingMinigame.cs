using UnityEngine;
using UnityEngine.UI;

public class CircularFishingMinigame : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject minigamePanel;   // FishingMinigamePanel
    [SerializeField] private RectTransform circularTrack; // Img_CircularTrack
    [SerializeField] private RectTransform catchZone;     // Img_CatchZone (Zona Hijau/Pencari)
    [SerializeField] private RectTransform fishIcon;      // Img_FishIcon (Ikan Merah)
    [SerializeField] private Image progressBarImage;      // Img_ProgressBar

    [Header("Track Boundary Settings")]
    [SerializeField] private float trackRadius = 60f;      // Nilai diperkecil agar ikan TIDAK KELUAR lingkaran

    [Header("Fish Movement Speeds")]
    [SerializeField] private float slowSpeed = 40f;        // Kecepatan momen pelan
    [SerializeField] private float fastSpeed = 160f;       // Kecepatan momen melesat cepat
    [SerializeField] private float directionChangeInterval = 1.2f; // Jeda ganti arah/kecepatan

    [Header("Catch Zone Settings")]
    [SerializeField] private float catchZoneRadius = 30f;  
    [SerializeField] private float idleDriftSpeed = 30f;   

    [Header("Progress Settings")]
    [SerializeField] private float baseFillSpeed = 0.3f;   // Kecepatan dasar bar bertambah
    [SerializeField] private float drainSpeed = 0.25f;     // Kecepatan bar berkurang jika meleset
    [SerializeField] private float slowdownThreshold = 0.7f;// Titik melambat saat hampir strike (70%)
    [SerializeField] private float slowdownFactor = 0.4f;   // Pengali kelambatan (40% dari kecepatan biasa)
    
    // Internal Variables
    private float currentProgress = 0.2f; 
    private Vector2 fishTargetPosition;
    private float fishTimer;
    private float currentFishSpeed;
    private bool isMinigameActive = false;

    public void StartMinigame()
    {
        if (minigamePanel != null) minigamePanel.SetActive(true);
        gameObject.SetActive(true);

        currentProgress = 0.2f;
        if (progressBarImage != null) progressBarImage.fillAmount = currentProgress;
        
        if (fishIcon != null) fishIcon.anchoredPosition = Vector2.zero;
        if (catchZone != null) catchZone.anchoredPosition = Vector2.zero;
        
        GetNewFishTarget();
        isMinigameActive = true;
    }

    private void Update()
    {
        if (!isMinigameActive) return;

        HandleCatchZoneMovement();
        HandleFishMovement();
        CheckOverlapAndProgress();
    }

    // 1. Kontrol Catch Zone via Mouse
    private void HandleCatchZoneMovement()
    {
        if (Input.GetMouseButton(0)) // Klik Kiri Ditahan
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                circularTrack,
                Input.mousePosition,
                null,
                out Vector2 localMousePos
            );

            // Batasi agar catch zone tetap di dalam track
            catchZone.anchoredPosition = Vector2.ClampMagnitude(localMousePos, trackRadius);
        }
        else
        {
            catchZone.anchoredPosition += Random.insideUnitCircle * (idleDriftSpeed * Time.deltaTime);
            catchZone.anchoredPosition = Vector2.ClampMagnitude(catchZone.anchoredPosition, trackRadius);
        }
    }

    // 2. Pergerakan Ikan Dinamis (Variasi Pelan & Cepat)
    private void HandleFishMovement()
    {
        fishTimer += Time.deltaTime;

        if (fishTimer >= directionChangeInterval || Vector2.Distance(fishIcon.anchoredPosition, fishTargetPosition) < 5f)
        {
            GetNewFishTarget();
        }

        fishIcon.anchoredPosition = Vector2.MoveTowards(
            fishIcon.anchoredPosition,
            fishTargetPosition,
            currentFishSpeed * Time.deltaTime
        );
    }

    private void GetNewFishTarget()
    {
        fishTimer = 0f;
        // Tentukan target acak di dalam radius ketat
        fishTargetPosition = Random.insideUnitCircle * trackRadius;

        // Acak momen: 60% peluang momen Pelan, 40% peluang momen Meleset Cepat
        if (Random.value < 0.6f)
        {
            currentFishSpeed = slowSpeed;
        }
        else
        {
            currentFishSpeed = fastSpeed;
        }
    }

    // 3. Overlap & Progress Bar Non-Linear (Melambat di Akhir)
    private void CheckOverlapAndProgress()
    {
        float distance = Vector2.Distance(catchZone.anchoredPosition, fishIcon.anchoredPosition);

        if (distance <= catchZoneRadius)
        {
            // Jika progres sudah melewati threshold (70%), laju bertambahnya melambat
            float activeFillSpeed = baseFillSpeed;
            if (currentProgress >= slowdownThreshold)
            {
                activeFillSpeed *= slowdownFactor; 
            }

            currentProgress += activeFillSpeed * Time.deltaTime;
        }
        else
        {
            currentProgress -= drainSpeed * Time.deltaTime;
        }

        currentProgress = Mathf.Clamp01(currentProgress);

        if (progressBarImage != null)
        {
            progressBarImage.fillAmount = currentProgress;
        }

        if (currentProgress >= 1f)
        {
            OnCatchSuccess();
        }
        else if (currentProgress <= 0f)
        {
            OnCatchFailed();
        }
    }

    private void OnCatchSuccess()
    {
        Debug.Log("Hooray! Ikan Berhasil Ditangkap!");
        EndMinigame();
    }

    private void OnCatchFailed()
    {
        Debug.Log("Sayang sekali, Ikan Lepas!");
        EndMinigame();
    }

    private void EndMinigame()
    {
        isMinigameActive = false;
        if (minigamePanel != null) minigamePanel.SetActive(false);
    }
}