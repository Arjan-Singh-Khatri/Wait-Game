using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SinglePlayer
{
    public class InfoText : MonoBehaviour
    {
        public static InfoText instance;
        private TMP_Text information;
        ObjectFader objectFader;
        [SerializeField] float fadeDuration;
        void Start()
        {
            if (instance != null) {
                Destroy(this);
            }

            instance = this;

            information = GetComponent<TMP_Text>();
            objectFader = GetComponent<ObjectFader>();
            if (information == null)
            {
                Debug.LogWarning("Info Text not set.");
            }

            DontDestroyOnLoad(this);
        }

        public void SetContent(string content)
        {
            StopAllCoroutines();
            information.text = content;
            objectFader.FadeIn();
            StartCoroutine(WaitToFadeOut(fadeDuration));
        }

        IEnumerator WaitToFadeOut(float t)
        {
            yield return new WaitForSeconds(t);
            objectFader.FadeOut();
        }
    }

}
