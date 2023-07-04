using System.Collections.Generic;
using UnityEngine;

namespace Core.Database
{
    [CreateAssetMenu(fileName = "DialogueSettings", menuName = "Game settings/Dialogue")]
    public class DialogueSettings : ScriptableObject
    {
        public TextAsset LoadGlobalsJSON;

        public List<Phrase> SadDialogues;
        public List<Phrase> EpicDialogues;

        public float TypingSpeed;
    }
}