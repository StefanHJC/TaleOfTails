using System;
using System.Collections.Generic;
using Core.Logic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Database
{
    [CreateAssetMenu(fileName = "Cursor sprite settings", menuName = "Game settings/Cursor ")]
    public class CursorSpriteSettings : SerializedScriptableObject
    {
        public Dictionary<CursorState, Texture2D> Sprites;
    }
}