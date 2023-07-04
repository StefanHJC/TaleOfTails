using System.Collections;
using UnityEngine;

namespace Core.Logic
{
    public class LoadingCurtain : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _curtain;
        [SerializeField] private float _deltaAlpha = .03f;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _curtain.alpha = 1;
        }
        public void ShowWithFade()
        {
            gameObject.SetActive(true);
            _curtain.alpha = 0;
            StartCoroutine(FadeOutRoutine());
        }

        public void Hide()
        {
            gameObject.SetActive(true);
            StartCoroutine(FadeInRoutine());
        }

        private IEnumerator FadeInRoutine()
        {
            while (_curtain.alpha > 0)
            {
                _curtain.alpha -= _deltaAlpha;
                yield return new WaitForSeconds(_deltaAlpha);
            }
            gameObject.SetActive(false);    
        }

        private IEnumerator FadeOutRoutine()
        {
            while (_curtain.alpha <=  1)
            {
                _curtain.alpha += _deltaAlpha;
                yield return new WaitForSeconds(_deltaAlpha);
            }
        }
    }
}