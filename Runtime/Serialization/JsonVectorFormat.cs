using System.Globalization;

namespace BIG
{
    /// <summary>
    /// Shared parsing/formatting plumbing for the JSON converters nested inside BIG types.
    /// All BIG types serialize to JSON as a compact string "{a, b, c}" — always with InvariantCulture,
    /// so files are portable between machines with different regional settings.
    /// </summary>
    internal static class JsonVectorFormat
    {
        /// <summary>
        /// Split "{a, b, c}" into trimmed parts. Returns null when input is empty or part count does not match.
        /// </summary>
        public static string[]? Split(object? rawValue, int expectedParts)
        {
            var str = rawValue?.ToString();
            if (string.IsNullOrEmpty(str)) return null;

            var parts = str.Trim('{', '}', ' ').Split(',');
            return parts.Length == expectedParts ? parts : null;
        }

        public static bool TryFloat(string part, out float value)
            => float.TryParse(part.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out value);

        public static bool TryInt(string part, out int value)
            => int.TryParse(part.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

        public static bool TryByte(string part, out byte value)
            => byte.TryParse(part.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

        public static string Format(float x, float y)
            => string.Format(CultureInfo.InvariantCulture, "{{{0}, {1}}}", x, y);

        public static string Format(float x, float y, float z)
            => string.Format(CultureInfo.InvariantCulture, "{{{0}, {1}, {2}}}", x, y, z);

        public static string Format(float x, float y, float z, float w)
            => string.Format(CultureInfo.InvariantCulture, "{{{0}, {1}, {2}, {3}}}", x, y, z, w);

        public static string Format(int x, int y)
            => string.Format(CultureInfo.InvariantCulture, "{{{0}, {1}}}", x, y);

        public static string Format(int x, int y, int z)
            => string.Format(CultureInfo.InvariantCulture, "{{{0}, {1}, {2}}}", x, y, z);

        public static string Format(int x, int y, int z, int w)
            => string.Format(CultureInfo.InvariantCulture, "{{{0}, {1}, {2}, {3}}}", x, y, z, w);
    }
}
