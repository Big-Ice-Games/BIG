// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using UnityEngine;

namespace BIG
{
    [CreateAssetMenu(fileName = "Settings", menuName = "BIG/Settings")]
    public class Settings : ScriptableObject, ISettings
    {
        [field: SerializeField] public uint SteamAppId { get; set; }

#if BIG_WORKBOOK
        [field: SerializeField] public string GoogleWorkbookDictionaryId { get; set; }
#endif
    }
}
