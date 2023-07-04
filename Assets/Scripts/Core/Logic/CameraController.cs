using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Logic
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float _horizontalSencivity;
        [SerializeField] private float _verticalSencivity;

        [SerializeField] [Min(-90)] [MaxValue(90)]
        private float _minVerticalLimit = -30;

        [SerializeField] [Min(0)] [MaxValue(90)]
        private float _maxVerticalLimit = 50;

        [SerializeField] private float _panSpeed = 20f;
        [SerializeField] private float _scrollSpeed = 20f;

        [SerializeField] private BoxCollider _boundsBoxCollider;
        private Vector3 _minBound;
        private Vector3 _maxBound;

        private Vector3 _startPoint;
        private Quaternion _startRotation;

        public static CameraController Instance { get; private set; }

        private void Start()
        {
            _startPoint = transform.position;
            _startRotation = transform.rotation;
        }

        private void Awake()
        {
            Instance = this;
            var bounds = _boundsBoxCollider.bounds;
            _minBound = bounds.min;
            _maxBound = bounds.max;
        }

        private void LateUpdate() // TEMP SHIT 
        {
            if (Input.GetKey(KeyCode.Space))
            {
                transform.SetPositionAndRotation(_startPoint, _startRotation);
            }

            if (Input.GetKey(KeyCode.Mouse2))
            {
                float h = _horizontalSencivity * Input.GetAxis("Mouse X") * Time.deltaTime;
                float v = _verticalSencivity * Input.GetAxis("Mouse Y") * Time.deltaTime;
                Quaternion rot = Quaternion.Euler(transform.rotation.eulerAngles.x - v, 0, 0);
                float newX = rot.eulerAngles.x;

                if (newX > 90)
                {
                    newX -= 360;
                }

                newX = Mathf.Clamp(newX, _minVerticalLimit, _maxVerticalLimit);
                transform.rotation = Quaternion.Euler(newX, transform.rotation.eulerAngles.y + h, 0);
            }

            Vector3 pos = transform.position;
            float yPos = pos.y;

            if (Input.GetKey("w"))
                pos += transform.forward * (_panSpeed * Time.deltaTime);

            if (Input.GetKey("s"))
                pos -= transform.forward * (_panSpeed * Time.deltaTime);

            if (Input.GetKey("d"))
                pos += transform.right * (_panSpeed * Time.deltaTime);

            if (Input.GetKey("a"))
                pos -= transform.right * (_panSpeed * Time.deltaTime);

            pos.y = yPos;

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            pos += transform.forward * (scroll * _scrollSpeed * 100f * Time.deltaTime);

            pos.x = Mathf.Clamp(pos.x, _minBound.x, _maxBound.x);
            pos.y = Mathf.Clamp(pos.y, _minBound.y, _maxBound.y);
            pos.z = Mathf.Clamp(pos.z, _minBound.z, _maxBound.z);

            transform.position = pos;
        }
    }
}