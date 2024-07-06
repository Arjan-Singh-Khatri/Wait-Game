using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SinglePlayer
{
    public class InfoText : MonoBehaviour
    {
        private TMP_Text information;
        ObjectFader objectFader;
        [SerializeField] float fadeDuration;
        void Start()
        {
            information = GetComponent<TMP_Text>();
            objectFader = GetComponent<ObjectFader>();
            if (information == null)
            {
                Debug.LogWarning("Info Text not set.");
            }
        }

        public void SetContent(string content)
        {
            StopAllCoroutines();
            information.text = content;
            objectFader.FadeIn();
            StartCoroutine(waitToFadeOut(fadeDuration));
        }

        IEnumerator waitToFadeOut(float t)
        {
            yield return new WaitForSeconds(t);
            objectFader.FadeOut();
        }
    }

}
