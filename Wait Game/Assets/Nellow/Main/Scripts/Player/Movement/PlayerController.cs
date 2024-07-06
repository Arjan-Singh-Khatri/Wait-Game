using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float gravity = -9.81f;

    private Rigidbody rb;
    private Transform cameraTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        cameraTransform = Camera.main.transform;
    }

    public void Move(float horizontal, float vertical)
    {
        if(TimeManager.Instance == null)
        {
            Debug.LogError("Time Manager is not set");
        }
        if (!TimeManager.Instance.IsRefueling() || TimeManager.Instance.IsHalfTime())
        {
            Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
            direction = cameraTransform.TransformDirection(direction);
            direction.y = 0;

            Vector3 movement = direction * moveSpeed * Time.deltaTime;

            rb.MovePosition(rb.position + movement);

            Vector3 gravityVector = new Vector3(0, gravity, 0);
            rb.AddForce(gravityVector, ForceMode.Acceleration);
            if (!TimeManager.Instance.IsRefueling() || TimeManager.Instance.IsHalfTime())
            {
                TimeManager.Instance.UseTime(1f * Time.deltaTime);
            }
        }
    }
}
