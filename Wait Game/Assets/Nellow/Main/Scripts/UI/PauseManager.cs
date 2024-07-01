using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private bool isPaused = false;

    public GameObject pauseMenu;
    public static PauseManager Instance;

    private void Awake()
    {
        // Ensure only one instance of the PauseManager exists
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this method to pause the game
    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0;
        LeanTween.pauseAll();
        pauseMenu.SetActive(true);
    }

    // Call this method to resume the game
    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1;
        LeanTween.resumeAll();
        pauseMenu.SetActive(false);
        CursorManager.Instance.SetCursorLock(true);
    }

    
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

   
    public bool IsPaused()
    {
        return isPaused;
    }
}
