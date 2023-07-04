using System.Collections;
using Cinemachine;
using UnityEngine;

namespace Core.Logic
{
    public class CameraShaker : MonoBehaviour
    {
        private CinemachineVirtualCamera _virtualCamera;
        private CinemachineBasicMultiChannelPerlin _cinemachinePerlin;

        private float _timer;

        private void Awake()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _cinemachinePerlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        public void ShakeCamera(float intensity, float time)
        {
            _cinemachinePerlin.m_AmplitudeGain = intensity;
            StartCoroutine(ShakeOff(time));
        }

        private IEnumerator ShakeOff(float time)
        {
            yield return new WaitForSeconds(time);
            _cinemachinePerlin.m_AmplitudeGain = 0;
        }
    }
}