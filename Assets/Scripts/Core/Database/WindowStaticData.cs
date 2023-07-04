using System.Collections.Generic;
using Core.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Database
{
    [CreateAssetMenu(fileName = "Window data", menuName = "Static data/Window")]
    public class WindowStaticData : SerializedScriptableObject
    {
        public List<WindowData> Data;
    }
}