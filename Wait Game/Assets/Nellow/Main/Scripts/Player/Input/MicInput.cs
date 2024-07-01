using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MicInput : MonoBehaviour
{
	public float volumeThreshold = 0.1f;
	public float sliderAmplifier = 1000f;
	public float stalkingPositionThreshold = 0.23f;
	public Slider micOutputSlider; 
	public TMP_Text micOutputText; 
	[SerializeField] EnemyAI enemyAI;

	private string microphone;
	private float micVolumeLevel;
	private AudioClip micClip;

	void Start()
	{
		if (Microphone.devices.Length > 0)
		{
			microphone = Microphone.devices[0];
			micClip = Microphone.Start(microphone, true, 10, 44100);
			if (micClip == null)
			{
				Debug.LogError("Microphone failed to start.");
				return;
			}
		}
		else
		{
			Debug.LogError("No microphone detected.");
		}

		micOutputSlider.minValue = 0;
		micOutputSlider.maxValue = 100; 
		micOutputSlider.value = 0;
		UpdateMicOutputText(0);
	}

	void Update()
	{
		micVolumeLevel = GetAveragedVolume();
		micOutputSlider.value = micVolumeLevel * sliderAmplifier; 
		UpdateMicOutputText(micVolumeLevel);

		if (micVolumeLevel > volumeThreshold)
		{
			enemyAI.SetPlayerLoud(true);
		}
		else if (micVolumeLevel > volumeThreshold / stalkingPositionThreshold)
		{
			enemyAI.SetPlayerQuiet(true); 
		}
		else
		{
			enemyAI.SetPlayerLoud(false);
		}
	}

	float GetAveragedVolume()
	{
		float[] data = new float[256];
		int micPosition = Microphone.GetPosition(microphone) - 256;
		if (micPosition < 0) return 0;
		micClip.GetData(data, micPosition);
		float totalVolume = 0;
		foreach (var s in data)
		{
			totalVolume += Mathf.Abs(s);
		}
		return totalVolume / data.Length;
	}

	public float GetMicVolumeLevel()
	{
		return micVolumeLevel;
	}

	void UpdateMicOutputText(float value)
	{
		micOutputText.text = $"Mic Output Level: {value:F2}";
	}
}
