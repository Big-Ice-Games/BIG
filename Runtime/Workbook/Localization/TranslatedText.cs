#if BIG_WORKBOOK

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BIG.Workbook.Localization
{
    public class TextTranslator : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private string _key;
        [SerializeField] private string _postfix;
        [SerializeField] private bool _upperCase;
        [SerializeField] private List<string> _parameters;

        private void Start()
        {
            Refresh();
            GameTranslator.OnLanguageChanged += Refresh;
        }

        private void OnDestroy() => GameTranslator.OnLanguageChanged -= Refresh;

        private void Refresh()
        {
            try
            {
                if (string.IsNullOrEmpty(_key)) return;

                var translated = GameTranslator.Translate(_key);
                if (_upperCase)
                    translated = translated.ToUpper();
                
                if (string.IsNullOrEmpty(_postfix) == false)
                    translated += _postfix;

                _label.text = translated;
                ReplaceParameters();
            }
            catch(Exception e)
            {
                this.Log($"Exception occur refreshing label for key: {_key}\n{e}", LogLevel.Error);
            }
        }

        private void ReplaceParameters()
        {
            try
            {
                if (_parameters != null && _parameters.Count > 0)
                    _label.text = string.Format(_label.text, _parameters);
            }
            catch (Exception e)
            {
                this.Log($"Translation parameters replacing failed on LabelTranslator attached to {gameObject.name}. \n{e}", LogLevel.Error);
            }
        }
        
        public void UpdateKey(string key)
        {
            _key = key;
            Refresh();
        }
        
        private void Reset()
        {
            _label = GetComponent<TMP_Text>();
            if(string.IsNullOrEmpty(_key) == false)
                UpdateKey(_key);
        }
    }
}
#endif