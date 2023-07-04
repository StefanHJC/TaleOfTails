using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;

namespace Core.Infrastructure
{
    public class SceneLoader : ISceneLoader
    {
        private readonly ICoroutineRunner _coroutineRunner;

        private AsyncOperation _loadingOperation;

        public float Progress => _loadingOperation?.progress ?? 0;
        public OperationStatus Status
        {
            get
            {
                if (_loadingOperation == null)
                    return OperationStatus.Sleep;
                if (_loadingOperation.progress > 0)
                    return OperationStatus.Progress;
                if (_loadingOperation.isDone)
                    return OperationStatus.Done;

                return OperationStatus.Done;
            }
        }

        public enum OperationStatus
        {
            Progress,
            Sleep,
            Done,
        }

        public SceneLoader(ICoroutineRunner coroutineRunner) => _coroutineRunner = coroutineRunner;

        public void Load(string name, Action onLoaded) => _coroutineRunner.StartCoroutine(LoadScene(name, onLoaded));

        public void Load(string name) => _coroutineRunner.StartCoroutine(LoadScene(name));

        public IEnumerator LoadScene(string name, Action onLoaded = null)
        {
            if (SceneManager.GetActiveScene().name == name)
            {
                onLoaded?.Invoke();
                yield break;
            }
            AsyncOperation waitForNextScene = SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
            
            _loadingOperation = waitForNextScene;

            while (waitForNextScene.isDone == false)
            {
                yield return null;

            }
            onLoaded?.Invoke();
        }
    }
}