#if BIG_WORKBOOK || UNITY_EDITOR

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace BIG.Editor.Workbook
{
    public static class GoogleSheetDownloader
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<byte[]> DownloadExcelFileFromGoogleSheets(string spreadSheetId)
        {
            string downloadUrl = $"https://docs.google.com/spreadsheets/d/{spreadSheetId}/export?format=xlsx";

            try
            {
                Debug.Log("Downloading Excel from Google Sheets...");
                HttpResponseMessage response = await _httpClient.GetAsync(downloadUrl);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.LogError($"Failed to download file. Status: {response.StatusCode}");
                    return null;
                }

                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error downloading Excel: {e.Message}");
                return null;
            }
        }
    }
}
#endif