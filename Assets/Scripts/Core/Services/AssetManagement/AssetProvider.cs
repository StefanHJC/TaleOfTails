using UnityEngine;

namespace Core.Services.AssetManagement
{
    public class AssetProvider : IAssets
    {
        public GameObject Load(string path)
        {
            var prefab = Resources.Load(path);

            return (GameObject)prefab;
        }

        public GameObject Instantiate(string path)
        {
            var prefab = Resources.Load(path);

            return (GameObject)Object.Instantiate(prefab);
        }

        public GameObject Instantiate(string path, Vector3 at)
        {
            var prefab = Resources.Load(path);

            return (GameObject)Object.Instantiate(prefab, at, Quaternion.identity);
        }

        public GameObject Instantiate(GameObject prefab) =>
            Object.Instantiate(prefab);

        public GameObject Instantiate(GameObject prefab, Vector3 at, Quaternion rotation = new()) =>
            Object.Instantiate(prefab, at, rotation);
    }
}