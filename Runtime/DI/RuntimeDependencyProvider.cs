using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace BIG
{
    /// <summary>
    /// This provider can provide dependencies into the game object on the engine side through <see cref="InjectAttribute"/>.
    /// </summary>
    public static class RuntimeDependencyProvider
    {
        private const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        /// <summary>
        /// Reflection metadata cached per type. Injection is called per game object instance (potentially hundreds
        /// of instances of the same type during scene load), while the set of injectable members never changes per type.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, InjectionInfo> CACHE = new ConcurrentDictionary<Type, InjectionInfo>();

        private sealed class InjectionInfo
        {
            public readonly FieldInfo[] Fields;
            public readonly PropertyInfo[] Properties;

            public InjectionInfo(FieldInfo[] fields, PropertyInfo[] properties)
            {
                Fields = fields;
                Properties = properties;
            }
        }

        private static InjectionInfo GetInjectionInfo(Type type)
        {
            if (CACHE.TryGetValue(type, out var cached))
                return cached;

            var allFields = type.GetFields(FLAGS);
            int fieldCount = 0;
            for (int i = 0; i < allFields.Length; i++)
            {
                if (allFields[i].IsDefined(typeof(InjectAttribute), false))
                    fieldCount++;
            }

            var fields = fieldCount == 0 ? Array.Empty<FieldInfo>() : new FieldInfo[fieldCount];
            for (int i = 0, f = 0; i < allFields.Length; i++)
            {
                if (allFields[i].IsDefined(typeof(InjectAttribute), false))
                    fields[f++] = allFields[i];
            }

            var allProperties = type.GetProperties(FLAGS);
            int propertyCount = 0;
            for (int i = 0; i < allProperties.Length; i++)
            {
                if (allProperties[i].IsDefined(typeof(InjectAttribute), false))
                    propertyCount++;
            }

            var properties = propertyCount == 0 ? Array.Empty<PropertyInfo>() : new PropertyInfo[propertyCount];
            for (int i = 0, p = 0; i < allProperties.Length; i++)
            {
                if (allProperties[i].IsDefined(typeof(InjectAttribute), false))
                    properties[p++] = allProperties[i];
            }

            var info = new InjectionInfo(fields, properties);
            CACHE[type] = info;
            return info;
        }

        /// <summary>
        /// Clear cached reflection metadata (e.g. after domain/assembly reload in the Editor).
        /// </summary>
        public static void ClearCache() => CACHE.Clear();

        /// <summary>
        /// Inject instances into fields and properties marked with <see cref="InjectAttribute"/>.
        /// Members that already have a value are skipped.
        /// </summary>
        /// <typeparam name="T">Extension method type.</typeparam>
        /// <param name="obj">Extension method parameter.</param>
        /// <exception cref="Exception">Throw exception if we failed to inject field or property.</exception>
        public static void ResolveMyDependencies<T>(this T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var info = GetInjectionInfo(obj.GetType());

            for (int i = 0; i < info.Fields.Length; i++)
            {
                var fieldInfo = info.Fields[i];
                try
                {
                    if (fieldInfo.GetValue(obj) != null)
                        continue;

                    fieldInfo.SetValue(obj, God.PrayFor(fieldInfo.FieldType));
                }
                catch (Exception e)
                {
                    throw new Exception(
                        $"Exception occur during injecting field {fieldInfo.FieldType} into {obj.ToString()} type of {typeof(T)}: {e}\n{e.StackTrace}");
                }
            }

            for (int i = 0; i < info.Properties.Length; i++)
            {
                var propertyInfo = info.Properties[i];
                try
                {
                    if (propertyInfo.GetValue(obj) != null)
                        continue;

                    propertyInfo.SetValue(obj, God.PrayFor(propertyInfo.PropertyType));
                }
                catch (Exception e)
                {
                    throw new Exception($"Exception occur during injecting property {propertyInfo.PropertyType} into {typeof(T)}: {e}\n{e.StackTrace}");
                }
            }
        }
    }
}
