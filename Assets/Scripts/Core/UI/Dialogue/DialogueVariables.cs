using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

namespace Core.UI.Dialogue
{
    public class DialogueVariables
    {
        public Dictionary<string, Ink.Runtime.Object> _variables { get; private set; }

        private Story _globalVariablesStory;
        private const string _saveVariablesKey = "INK_VARIABLES";

        public DialogueVariables(TextAsset loadGlobalsJSON)
        {
            // create the story
            _globalVariablesStory = new Story(loadGlobalsJSON.text);
            // if we have saved data, load it
            // if (PlayerPrefs.HasKey(_saveVariablesKey))
            // {
            //     string jsonState = PlayerPrefs.GetString(_saveVariablesKey);
            //     _globalVariablesStory.state.LoadJson(jsonState);
            // }

            // initialize the dictionary
            _variables = new Dictionary<string, Ink.Runtime.Object>();
            foreach (string name in _globalVariablesStory.variablesState)
            {
                Ink.Runtime.Object value = _globalVariablesStory.variablesState.GetVariableWithName(name);
                _variables.Add(name, value);
                Debug.Log("Initialized global dialogue variable: " + name + " = " + value);
            }
        }

        public void SaveVariables()
        {
            if (_globalVariablesStory != null)
            {
                // Load the current state of all of our _variables to the globals story
                VariablesToStory(_globalVariablesStory);
                // NOTE: eventually, you'd want to replace this with an actual save/load method
                // rather than using PlayerPrefs.
                PlayerPrefs.SetString(_saveVariablesKey, _globalVariablesStory.state.ToJson());
            }
        }

        public void StartListening(Story story)
        {
            // it's important that VariablesToStory is before assigning the listener!
            VariablesToStory(story);
            story.variablesState.variableChangedEvent += VariableChanged;
        }

        public void StopListening(Story story)
        {
            story.variablesState.variableChangedEvent -= VariableChanged;
        }

        private void VariableChanged(string name, Ink.Runtime.Object value)
        {
            // only maintain _variables that were initialized from the globals ink file
            if (_variables.ContainsKey(name))
            {
                _variables.Remove(name);
                _variables.Add(name, value);
            }
        }

        private void VariablesToStory(Story story)
        {
            foreach (KeyValuePair<string, Ink.Runtime.Object> variable in _variables)
            {
                story.variablesState.SetGlobal(variable.Key, variable.Value);
            }
        }

    }

}