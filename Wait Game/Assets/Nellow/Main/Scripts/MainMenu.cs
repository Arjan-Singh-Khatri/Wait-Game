using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public TMPro.TMP_Dropdown qualityDropdown;
    public GameObject loadingScreen;
    public Image loadingFillImage; //

    void Start()
    {
        PopulateQualityDropdown();
        qualityDropdown.onValueChanged.AddListener(delegate { SetQualityLevel(); });
        loadingScreen.SetActive(false);
    }

    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadYourAsyncScene(sceneName));
    }

    IEnumerator LoadYourAsyncScene(string sceneName)
    {
        loadingScreen.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        
        while (!asyncLoad.isDone)
        {
            loadingFillImage.fillAmount = asyncLoad.progress / 0.9f;

            if (asyncLoad.progress >= 0.9f)
            {
                loadingFillImage.fillAmount = 1f;
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    void PopulateQualityDropdown()
    {
        qualityDropdown.ClearOptions();
        string[] names = QualitySettings.names;
        foreach (string name in names)
        {
            qualityDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(name));
        }
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();
    }

    public void SetQualityLevel()
    {
        QualitySettings.SetQualityLevel(qualityDropdown.value);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
