// Copyright (c) 2025, Big Ice Games
// All rights reserved.

namespace BIG
{
    public interface ISettings
    {
        uint SteamAppId { get; }
        bool UseWorkbook { get; }
        string GoogleWorkbookDictionaryId { get; }
    }
}