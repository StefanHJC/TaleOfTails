using System;
using System.Collections;
using Core.Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.HUD
{
    public class RoundIndicator : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private TMP_Text _text;
        
        private ITurnResolver _turnResolver;
        private Coroutine _fadeIn;
        private Coroutine _fadeOut;
        private Coroutine _showNewRound;

        public void Construct(ITurnResolver turnResolver)
        {
            _turnResolver = turnResolver;

            _turnResolver.RoundStarted += OnNewRound;
        }

        private void OnDestroy() => StopAllCoroutines();

        private void OnNewRound()
        {
            _showNewRound = StartCoroutine(ShowNewRoundIndex());
        }

        private IEnumerator ShowNewRoundIndex()
        {
            _fadeOut = StartCoroutine(FadeOut());
            _text.text = _turnResolver.CurrentRound.ToString();
            yield return new WaitForSeconds(2f);
            _fadeIn = StartCoroutine(FadeIn());
        }

        private IEnumerator FadeIn()
        {
            while (_background.color.a > 0)
            {
                _background.color = new Color(_background.color.r, _background.color.g, _background.color.b, _background.color.a - 0.03f);
                _text.color = new Color(_background.color.r, _background.color.g, _background.color.b, _background.color.a - 0.03f);
                yield return null;
            }
        }

        private IEnumerator FadeOut()
        {
            while (_background.color.a < 1)
            {
                _background.color = new Color(_background.color.r, _background.color.g, _background.color.b, _background.color.a + 0.03f);
                _text.color = new Color(_background.color.r, _background.color.g, _background.color.b, _background.color.a + 0.03f);
                yield return null;
            }
        }
    }
}