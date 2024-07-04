using UnityEngine;

public class CameraWakeUp : MonoBehaviour
{
    public Transform targetPosition;
    public float wakeUpSpeed = 1.0f;
    public float staggerFrequency = 0.5f;
    public float staggerMagnitude = 0.1f;
    public AudioClip gruntingSound;
    public MonoBehaviour[] scriptsToEnable;

    private bool isWakingUp = true;
    private AudioSource audioSource;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float staggerTimer = 0.0f;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(90, 0, 0);  

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = gruntingSound;
        audioSource.Play();

        foreach (MonoBehaviour script in scriptsToEnable)
        {
            script.enabled = false;
        }
    }

    void Update()
    {
        if (isWakingUp)
        {
            staggerTimer += Time.deltaTime;
            

            float step = wakeUpSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, targetPosition.position, step);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetPosition.rotation, step);

            if (Vector3.Distance(transform.position, targetPosition.position) < 0.01f)
            {
                transform.position = targetPosition.position;
                transform.rotation = targetPosition.rotation;
                isWakingUp = false;
                foreach (MonoBehaviour script in scriptsToEnable)
                {
                    script.enabled = true;
                }
                Destroy(this);
            }
        }
    }
}
