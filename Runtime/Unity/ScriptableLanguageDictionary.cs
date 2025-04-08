#if BIG_WORKBOOK

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using System;
using System.Collections.Generic;
using BIG.Workbook;
using UnityEngine;

namespace BIG
{
    [Serializable]
    [ClassMapping(true, typeof(ScriptableLanguageDictionary), "Assets/Resources/BIG/Dictionary.xlsx", "Dictionary")]
    public struct DictionaryRecord
    {
        public string Tag;
        public string English;
        public string Polish;
        public string French;
        public string Italian;
        public string German;
        public string Spanish;
        public string Japanese;
        public string Portuguese;
        public string Arabic;
        public string Korean;
        public string Russian;
        public string ChineseSimplified;
        public string ChineseTraditional;
        public string Turkish;
        public string Hungarian;
        public string Thai;

        public string GetValue(SystemLanguage language)
        {
            switch (language)
            {
                case SystemLanguage.Arabic: return Arabic;
                case SystemLanguage.English: return English;
                case SystemLanguage.French: return French;
                case SystemLanguage.German: return German;
                case SystemLanguage.Hungarian: return Hungarian;
                case SystemLanguage.Italian: return Italian;
                case SystemLanguage.Japanese: return Japanese;
                case SystemLanguage.Korean: return Korean;
                case SystemLanguage.Polish: return Polish;
                case SystemLanguage.Portuguese: return Portuguese;
                case SystemLanguage.Russian: return Russian;
                case SystemLanguage.Spanish: return Spanish;
                case SystemLanguage.Thai: return Thai;
                case SystemLanguage.Turkish: return Turkish;
                case SystemLanguage.ChineseSimplified: return ChineseSimplified;
                case SystemLanguage.ChineseTraditional: return ChineseTraditional;
                default: return English;
            }
        }
    }
    

    public class ScriptableLanguageDictionary : ScriptableCollection<DictionaryRecord>
    {
        protected override string ConcretePath => "LanguageDictionary";

        public Dictionary<string, string> GetDictionary(SystemLanguage language)
        {
            try
            {
                Dictionary<string, string> dict = new Dictionary<string, string>(Entities.Length);
                for (int i = 0; i < Entities.Length; i++)
                    dict.Add(Entities[i].Tag, Entities[i].GetValue(language));
            
                return dict;
            }
            catch (Exception e)
            {
                this.Log("Exception occur during creating dictionary: " + e, LogLevel.Error);
                return new Dictionary<string, string>(0);
            }
        }
    }
}
#endif