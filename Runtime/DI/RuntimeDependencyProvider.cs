using System;
using System.Linq;
using System.Reflection;

namespace BIG
{
    /// <summary>
    /// This provider can provide dependencies into the game object on the engine side through <see cref="InjectAttribute"/>.
    /// </summary>
    public static class RuntimeDependencyProvider
    {
        /// <summary>
        /// Inject instances into fields and properties marked with <see cref="InjectAttribute"/>.
        /// </summary>
        /// <typeparam name="T">Extension method type.</typeparam>
        /// <param name="obj">Extension method parameter.</param>
        /// <exception cref="Exception">Throw exception if we failed to inject field or property.</exception>
        public static void ResolveMyDependencies<T>(this T obj)
        {
            foreach (FieldInfo fieldInfo in obj.GetFieldsWithAttribute<T, InjectAttribute>().Where(f => f.GetValue(obj) == null))
            {
                try
                {
                    fieldInfo.SetValue(obj, God.PrayFor(fieldInfo.FieldType));
                }
                catch (Exception e)
                {
                    throw new Exception(
                        $"Exception occur during injecting field {fieldInfo.FieldType} into {obj.ToString()} type of {typeof(T)}: {e.ToString()}\n{e.StackTrace}");
                }

            }
            foreach (PropertyInfo propertyInfo in obj.GetPropertiesWithAttribute<T, InjectAttribute>().Where(f => f.GetValue(obj) == null))
            {
                try
                {
                    propertyInfo.SetValue(obj, God.PrayFor(propertyInfo.PropertyType));
                }
                catch (Exception e)
                {
                    throw new Exception($"Exception occur during injecting property {propertyInfo.PropertyType} into {typeof(T)}: {e.ToString()}\n{e.StackTrace}");
                }
            }
        }

    }
}
