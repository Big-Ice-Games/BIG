#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using UnityEngine;

namespace BIG
{
    [CreateAssetMenu(fileName = "Settings", menuName = "BIG/Settings")]
    public class Settings : ScriptableObject, ISettings
    {
        [field: SerializeField] public uint SteamAppId { get; set; }
    }
}
