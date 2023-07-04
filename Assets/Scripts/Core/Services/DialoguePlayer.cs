using System;
using System.Collections;
using System.Collections.Generic;
using Core.Database;
using Core.Infrastructure;
using Core.UI;
using Core.UI.Dialogue;
using Ink.Runtime;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Services
{
    public class DialoguePlayer
    {
        private const string SPEAKER_TAG = "speaker";
        private const string PORTRAIT_TAG = "portrait";
        private const string LAYOUT_TAG = "layout";

        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IDatabaseService _data;
        private readonly IGameFactory _gameFactory;
        private readonly DialogueSettings _settings;
        private readonly ISoundService _sound;

        private DialogueWindow _window;
        private Story _currentStory;
        private Coroutine _displayLineCoroutine;
        private DialogueVariables _dialogueVariables;
        private bool _isPlaying;
        private bool _canContinueToNextLine;
        private float _typeSpeed;

        public event Action DialogueEnded;

        public DialoguePlayer(ICoroutineRunner coroutineRunner, IDatabaseService data, IGameFactory gameFactory, ISoundService sound)
        {
            _coroutineRunner = coroutineRunner;
            _gameFactory = gameFactory;
            _data = data;
            _settings = data.TryGetDialogueSettings();
            _typeSpeed = _settings.TypingSpeed;
            _dialogueVariables = new DialogueVariables(_settings.LoadGlobalsJSON);
        }


        public void PlayDialogue(TextAsset inkJSON)
        {
            _currentStory = new Story(inkJSON.text);
            _isPlaying = true;
            _window = _gameFactory.UI.CreateDialogueWindow();

            _dialogueVariables.StartListening(_currentStory);
            
            PlayNext();
        }
        
        public void PlayNext()
        {
            if (_currentStory.canContinue)
            {
                if (_displayLineCoroutine != null)
                {
                    _coroutineRunner.StopCoroutine(_displayLineCoroutine);
                }
                _displayLineCoroutine = _coroutineRunner.StartCoroutine(DisplayLineRoutine(_currentStory.Continue()));
                HandleTags(_currentStory.currentTags);
            }
            else
            {
                _coroutineRunner.StartCoroutine(ExitDialogueRoutine());
            }

        }

        public IEnumerator ExitDialogueRoutine()
        {
            yield return new WaitForSeconds(.2f);

            _dialogueVariables.StopListening(_currentStory);
            DialogueEnded?.Invoke();
            _isPlaying = false;
            Object.Destroy(_window.gameObject);
        }

        private IEnumerator DisplayLineRoutine(string line)
        {
            _window.Text.text = line;
            _window.Text.maxVisibleCharacters = 0;

            _canContinueToNextLine = false;

            bool isAddingRichTextTag = false;

            foreach (char letter in line.ToCharArray())
            {
                if (letter == '<' || isAddingRichTextTag)
                {
                    isAddingRichTextTag = true;
                    if (letter == '>')
                    {
                        isAddingRichTextTag = false;
                    }
                }
                else
                {
                    _window.Text.maxVisibleCharacters++;
                    yield return new WaitForSeconds(_typeSpeed);
                }
            }

            _canContinueToNextLine = true;
        }

        private void HandleTags(List<string> currentTags)
        {
            foreach (string tag in currentTags)
            {
                string[] splitTag = tag.Split(':');

                if (splitTag.Length != 2)
                {
                    Debug.LogError("Tag could not be appropriately parsed: " + tag);
                }
                string tagKey = splitTag[0].Trim();
                string tagValue = splitTag[1].Trim();

                switch (tagKey)
                {
                    case SPEAKER_TAG:
                        _window.FirstSpeakerLabel.text = tagValue;
                        break;
                    case PORTRAIT_TAG:
                        _window.PortaitAnimator.Play(tagValue);
                        break;
                    default:
                        Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                        break;
                }
            }
        }
    }
}