using System;
using UnityEngine;

namespace Core.Database
{
    [Serializable]
    public class Phrase
    {
        public TextAsset InkJSON;
        public PhraseType Type;
    }
}