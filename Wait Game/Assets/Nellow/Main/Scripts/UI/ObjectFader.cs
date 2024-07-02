using UnityEngine;
using UnityEngine.UI;

public class ObjectFader : MonoBehaviour
{
    public float fadeDuration = 1f; 
    public LeanTweenType easeType = LeanTweenType.easeOutQuad;

    void Start()
    {
        if( PauseManager.Instance != null && PauseManager.Instance.IsPaused() ) return;
        FadeOut(); // Start by fading out the object and its children
    }

    public void FadeIn()
    {
        if( PauseManager.Instance != null && PauseManager.Instance.IsPaused()) return;
        Fade(transform, 1f); // Fade in the object and its children
    }

    public void FadeOut()
    {
        if( PauseManager.Instance != null && PauseManager.Instance.IsPaused()) return;
        Fade(transform, 0f); // Fade out the object and its children
    }

    private void Fade(Transform target, float targetAlpha)
    {
         if( PauseManager.Instance != null && PauseManager.Instance.IsPaused()) return;
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = target.gameObject.AddComponent<CanvasGroup>(); // Add CanvasGroup if not found
        }
        
        LeanTween.alphaCanvas(canvasGroup, targetAlpha, fadeDuration).setEase(easeType);

        // Recursively fade all children
        foreach (Transform child in target)
        {
            Fade(child, targetAlpha);
        }
    }
}
