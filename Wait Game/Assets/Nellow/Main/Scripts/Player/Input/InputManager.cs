using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerCamController playerCamController;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerCamController = FindObjectOfType<PlayerCamController>();
    }

    private void Update()
    {
        HandleKeyboardInput();
        HandleMouseInput();
    }

    private void HandleKeyboardInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            playerController.Move(horizontal, vertical);
        }
    }

    private void HandleMouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (mouseX != 0 || mouseY != 0)
        {
            playerCamController.Look(mouseX, mouseY);
        }
    }
}
