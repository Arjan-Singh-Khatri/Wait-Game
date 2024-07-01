using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    private bool isCursorLocked;

    void Awake()
    {
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorState();
        }

        UpdateCursorState();
    }

    public void ToggleCursorState()
    {
        isCursorLocked = !isCursorLocked;
        UpdateCursorState();
    }

    public void SetCursorLock(bool locked)
    {
        isCursorLocked = locked;
        UpdateCursorState();
    }

    private void UpdateCursorState()
    {
        if (!isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            //Cursor.visible = false;
        }
    }
}
