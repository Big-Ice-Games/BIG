#if BIG_WORKBOOK

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using System;
using System.IO;
using System.Linq;
using BIG.Workbook;
using UnityEditor;
using UnityEngine;

namespace BIG.Editor.Workbook
{
    public static class BigWorkbookReloadDictionary
    {
        [MenuItem("BIG/Workbook - Reload Dictionary")]
        public static async void DownloadAndProcessExcel()
        {
            var dictionaryId = GetDictionaryId();
            byte[] excelData = await GoogleSheetDownloader.DownloadExcelFileFromGoogleSheets(dictionaryId);
            if (excelData == null)
                return;
            
            string tempPath = Path.Combine(Application.dataPath, "Resources/Big/Dictionary.xlsx");
            await File.WriteAllBytesAsync(tempPath, excelData);
            
            Debug.Log("<color=green>[BIG] Resources/Big/Dictionary.xlsx. downloaded.</color>");
            
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
        }
        
        private static string GetDictionaryId()
        {
            var loaded = Resources.LoadAll<ScriptableObject>("BIG");
            var settings = loaded.OfType<ISettings>().FirstOrDefault();

            return settings?.GoogleWorkbookDictionaryId ??
                   throw new Exception("Assets/Resources/Big/Settings not found or GoogleWorkbookDictionaryId not set.\n" +
                                       "You need to assign GoogleWorkbookDictionaryId with your publicly available Google Sheets ID.\n" +
                                       "For more information check https://github.com/Big-Ice-Games/BIGWorkbook/blob/main/README.md");
        }
    }
}
#endif