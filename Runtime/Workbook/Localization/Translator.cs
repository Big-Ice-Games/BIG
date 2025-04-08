#if BIG_WORKBOOK

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace BIG.Workbook.Localization
{
	public class GameTranslator
	{
		private const string LANGUAGE = "language";
		private static GameTranslator _instance;
		private static GameTranslator Instance => _instance ??= new GameTranslator();
		private Dictionary<string, string> _dictionary;
		private readonly ScriptableLanguageDictionary _languageDictionary;
		public static event Action OnLanguageChanged; 
		
		public GameTranslator()
		{
			_languageDictionary = Resources.Load<ScriptableLanguageDictionary>("BIG/LanguageDictionary");
			var language = GetLanguage();
			SetLanguage(language);
		}
		
		private void SetLanguage(SystemLanguage language)
		{
			PlayerPrefs.SetString(LANGUAGE, language.ToString());
			PlayerPrefs.Save();
			
			Debug.Log($"<color=green>Translator loaded and saved with language</color> [{language}].");
			
			_dictionary = _languageDictionary.GetDictionary(language);
			OnLanguageChanged?.Invoke();
		}

		private SystemLanguage GetLanguage()
		{
			var savedLanguage = PlayerPrefs.GetString(LANGUAGE, Application.systemLanguage.ToString());
			return Enum.Parse<SystemLanguage>(savedLanguage, true);
		}
		
		public static string Translate(string key)
		{
			return Instance._dictionary.GetIfExists(key) ?? key;
		}

		public static string Translate(string key, params object[] param)
		{
			try
			{
				if (param != null && param.Length > 0)
				{
					return string.Format(Translate(key), param);
				}

				return Translate(key);
			}
			catch (Exception e)
			{
				Debug.LogWarning($"Translation parameters replacing failed for key ({key}). \n{e}");
				return key;
			}
		}
	}
}
#endif