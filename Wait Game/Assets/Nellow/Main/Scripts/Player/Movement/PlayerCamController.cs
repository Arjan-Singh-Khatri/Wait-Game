using UnityEngine;

public class PlayerCamController : MonoBehaviour
{
    [SerializeField] private Transform playerBody;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private bool restrictLook = true;

    private float xRotation = 0f;
    private float yRotation = 0f;
    public float refuelLookLimit = 30f;
    private void Start()
    {
        if (CursorManager.Instance == null)
        {
            Debug.LogError("Cursor Manager is null, Please assign it");
            return;
        }
        CursorManager.Instance.SetCursorLock(true);
    }

    public void Look(float mouseX, float mouseY)
    {
        if (TimeManager.Instance == null)
        {
            Debug.LogError("Time Manager is not set");
        }

        mouseX *= mouseSensitivity * Time.deltaTime;
        mouseY *= mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (TimeManager.Instance.IsRefueling() && restrictLook)
        {
            yRotation += mouseX;
        }
        else
        {
            yRotation += mouseX;
        }

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        playerBody.localRotation = Quaternion.Euler(0f, yRotation, 0f);

        if (!TimeManager.Instance.IsRefueling())
        {
            TimeManager.Instance.UseTime(1f * Time.deltaTime);
        }
    }
}
