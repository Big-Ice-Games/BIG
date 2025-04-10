﻿#if BIG_WORKBOOK || UNITY_EDITOR

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using System;
using System.IO;
using System.Threading.Tasks;

namespace BIG.Editor.Workbook
{
    internal static class TextFileLoader
    {
        private const string TEXT_EXTENSION = ".txt";

        internal static async Task<string[]> LoadTxtFile(string filePath)
        {
            Assert(filePath);
            using (var reader = File.OpenText(filePath))
            {
                return (await reader.ReadToEndAsync().ConfigureAwait(false)).Split(new[] { Environment.NewLine },
                    StringSplitOptions.None);
            }
        }

        internal static bool IsTxtFile(string file)
        {
            return Path.GetExtension(file) == TEXT_EXTENSION;
        }

        private static void Assert(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new InvalidOperationException($"File '{filePath}' does not exists");
            }

            if (!IsTxtFile(filePath))
            {
                throw new InvalidOperationException($"File '{filePath}' should be a .txt file.");
            }
        }
    }
}
#endif