using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern agar SceneLoader tidak terduplikasi
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Fungsi untuk pindah scene berdasarkan nama
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Fungsi untuk pindah scene berdasarkan indeks di Build Settings
    public void LoadSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }
}