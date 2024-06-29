using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [SerializeField] private float maxTime = 100f;
    [SerializeField] private float refuelRate = 5f;
    [SerializeField] private Slider timerSlider;

    private float currentTime;
    private bool isRefueling;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            currentTime = maxTime;
        }
    }

    private void Start()
    {
        timerSlider.maxValue = maxTime;
        UpdateSlider(maxTime);
    }

    private void Update()
    {
        if (timerSlider == null)
        {
            return;
        }

        if (isRefueling)
        {
            currentTime += refuelRate * Time.deltaTime;
            currentTime = Mathf.Min(currentTime, maxTime);
            UpdateSlider(currentTime);
        }

        if (currentTime > maxTime * 0.25f)
        {
            timerSlider.interactable = true;
        }

        if (currentTime >= maxTime)
        {
            isRefueling = false;
        }
    }

    public void UseTime(float amount)
    {
        isRefueling = false;
        timerSlider.interactable = true;

        if (currentTime >= amount && !isRefueling)
        {
            currentTime -= amount;
            UpdateSlider(currentTime);

            if (currentTime <= 0)
            {
                isRefueling = true;
            }
        }
        else
        {
            isRefueling = true;
            Debug.Log("Not enough time, starting refuel.");
        }
    }

    public bool IsRefueling()
    {
        return isRefueling;
    }

    public bool IsHalfTime()
    {
        return currentTime > (maxTime * 0.25f);
    }

    private void UpdateSlider(float value)
    {
        timerSlider.value = value;
        timerSlider.interactable = !isRefueling;
    }
}
