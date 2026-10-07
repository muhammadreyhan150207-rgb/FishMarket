using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void OnClickPlay()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene("03_FishingSpot");
        }
    }

    public void OnClickQuit()
    {
        Debug.Log("Keluar dari Game!");
        Application.Quit();
    }
}