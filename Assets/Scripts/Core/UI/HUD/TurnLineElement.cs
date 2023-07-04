using System.Collections;
using Core.Unit;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.HUD
{
    public class TurnLineElement : MonoBehaviour
    {
        [SerializeField] private Image _unitImage;
        [SerializeField] private Image _background;

        private BaseUnit _unit;

        public BaseUnit Unit => _unit;

        public void Construct(BaseUnit unit)
        {
            _unit = unit;
            _unitImage.sprite = _unit.Config.UiImage;
        }

        [Sirenix.OdinInspector.Button]
        public void AnimateFadeOut()
        {
            StartCoroutine(KickDown());
        }

        [Sirenix.OdinInspector.Button]
        public void AnimateMoveRight()
        {
            StartCoroutine(AffectPositionByX(-90));
        }

        private IEnumerator KickDown()
        {
            while (_unitImage.color.a > 0)
            {
                _unitImage.color = new Color(_unitImage.color.r, _unitImage.color.g, _unitImage.color.b, _unitImage.color.a - .05f);
                _unitImage.rectTransform.position = new Vector3(_unitImage.rectTransform.position.x,
                    _unitImage.rectTransform.position.y - 3.5f, _unitImage.rectTransform.position.z);
                yield return null;
            }
        }

        private IEnumerator AffectPositionByX(int deltaX)
        {
            int currDelta = 0;

            while (currDelta < deltaX)
            {
                _unitImage.rectTransform.position = new Vector3(_unitImage.rectTransform.position.x - 3.5f,
                    _unitImage.rectTransform.position.y, _unitImage.rectTransform.position.z);
                currDelta++;
                yield return null;
            }
        }
    }
}