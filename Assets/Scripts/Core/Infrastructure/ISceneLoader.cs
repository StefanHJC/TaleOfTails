using System;
using System.Collections;

namespace Core.Infrastructure
{
    public interface ISceneLoader
    {
        public float Progress { get; }
        public SceneLoader.OperationStatus Status { get; }
        public void Load(string name, Action onLoaded);
        public void Load(string name);
        public IEnumerator LoadScene(string name, Action onLoaded = null);
    }
}