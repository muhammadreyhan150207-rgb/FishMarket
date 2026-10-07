using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    private void Start()
    {
        // Otomatis pindah ke MainMenu setelah manager siap
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene("01_MainMenu");
        }
    }
}