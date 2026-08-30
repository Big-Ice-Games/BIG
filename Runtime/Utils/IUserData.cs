using System;

namespace BIG
{
    /// <summary>
    /// Strongly-typed key for persistent user data — <see cref="IUserData"/> accepts only declared keys,
    /// so a typo in a raw string can never silently create a new entry.
    /// Implicitly converts to string, so implementations (PlayerPrefs, Steam Cloud, iOS Keychain...)
    /// work with plain string keys underneath.
    /// Libraries expose their keys as static fields in domain classes; the game composes them
    /// into a single entry point (e.g. Keys.Sound.MusicLevel, Keys.Graphic.VSync).
    /// </summary>
    public readonly struct UserDataKey : IEquatable<UserDataKey>
    {
        public readonly string Value;

        public UserDataKey(string value) => Value = value;

        public static implicit operator string(UserDataKey key) => key.Value;

        public override string ToString() => Value;

        public bool Equals(UserDataKey other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is UserDataKey other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;

        public static bool operator ==(UserDataKey a, UserDataKey b) => a.Value == b.Value;
        public static bool operator !=(UserDataKey a, UserDataKey b) => a.Value != b.Value;
    }

    /// <summary>
    /// Marker for user-data key providers. Implement it on a top-level, NON-SEALED class exposing keys
    /// as public static readonly <see cref="UserDataKey"/> fields, e.g.:
    /// <code>
    /// public class SoundKeys : IUserDataKeysProvider
    /// {
    ///     public static readonly UserDataKey MusicLevel = new UserDataKey("Sound.MusicLevel");
    /// }
    /// </code>
    /// The BIG > Generate User Keys editor tool collects every implementation and generates a single
    /// game-side entry point, so all keys are reachable as Keys.SoundKeys.MusicLevel etc.
    /// </summary>
    public interface IUserDataKeysProvider { }

    /// <summary> Interface to manage persistent user data, like player preferences or game settings. </summary>
    public interface IUserData
    {
        public void Set<T>(UserDataKey key, T value);
        public void Set(UserDataKey key, string value);
        public void Set(UserDataKey key, int value);
        public void Set(UserDataKey key, bool value);
        public void Set(UserDataKey key, float value);
        public T Get<T>(UserDataKey key);
        public T Get<T>(UserDataKey key, T defaultValue);
        public string GetString(UserDataKey key, string defaultValue = "");
        public int GetInt(UserDataKey key, int defaultValue = 0);
        public bool GetBool(UserDataKey key, bool defaultValue = false);
        public float GetFloat(UserDataKey key, float defaultValue = 0f);
        public T GetEnum<T>(UserDataKey key, T defaultValue = default(T)) where T : struct, Enum;
    }
}
