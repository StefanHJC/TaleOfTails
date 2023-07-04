using UnityEngine;

namespace Core.Services.AssetManagement
{
    public interface IAssets : IService
    {
        public GameObject Load(string path);
        public GameObject Instantiate(string path);
        public GameObject Instantiate(GameObject prefab);
        public GameObject Instantiate(string path, Vector3 at);
        public GameObject Instantiate(GameObject prefab, Vector3 at, Quaternion rotation = new());
    }
}