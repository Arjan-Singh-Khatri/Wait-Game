using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private float footstepInterval = 0.5f; // interval in seconds between footsteps

    private Rigidbody rb;
    private Transform cameraTransform;
    private AudioSource audioSource;
    private int previousFootstepIndex = -1;
    private float footstepTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        cameraTransform = Camera.main.transform;
        audioSource = GetComponent<AudioSource>();
    }

    public void Move(float horizontal, float vertical)
    {
        if (TimeManager.Instance == null)
        {
            Debug.LogError("Time Manager is not set");
            return;
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

            // Check if the player is moving and play footstep sounds
            if (movement.magnitude > 0.1f) // Adjust the threshold as necessary
            {
                footstepTimer -= Time.deltaTime;
                if (footstepTimer <= 0f)
                {
                    PlayFootstepSound();
                    footstepTimer = footstepInterval;
                }
            }
        }
    }

    private void PlayFootstepSound()
    {
        if (footstepSounds.Length == 0)
        {
            Debug.LogWarning("No footstep sounds assigned.");
            return;
        }

        int newFootstepIndex;
        do
        {
            newFootstepIndex = Random.Range(0, footstepSounds.Length);
        } while (newFootstepIndex == previousFootstepIndex);

        audioSource.PlayOneShot(footstepSounds[newFootstepIndex]);
        previousFootstepIndex = newFootstepIndex;
    }
}
