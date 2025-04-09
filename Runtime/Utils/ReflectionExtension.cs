// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#pragma warning disable CS8603
#pragma warning disable CS8602

namespace BIG
{
    /// <summary>
    /// Extension for common reflection usage.
    /// </summary>
    public static class ReflectionExtension
    {
        private const BindingFlags DEFAULT_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        /// <summary>
        /// If wew have multiple places where we need to get all types from assemblies we can cache them.
        /// This cache is cleared at the start of the game and when we call <see cref="ClearCache"/> method because it is used by the Editor and by the game.
        /// </summary>
        private static List<Type> _cachedTypes = null;
        
        /// <summary>
        /// Gets fields with given attribute type.
        /// </summary>
        /// <typeparam name="T">Typeof class which you want to search in.</typeparam>
        /// <typeparam name="TA">Attribute type.</typeparam>
        /// <param name="obj">Instance which you are search in.</param>
        /// <param name="flags">Reflection binding flags.</param>
        /// <returns>Array of field info.</returns>
        public static FieldInfo[] GetFieldsWithAttribute<T, TA>(
          this T obj,
          BindingFlags flags = DEFAULT_FLAGS) where TA : Attribute
        {
            return obj.GetType().GetFields(flags).Where(p => p.IsDefined(typeof(TA), false)).ToArray();
        }

        /// <summary>
        /// Gets properties with given attribute type.
        /// </summary>
        /// <typeparam name="T">Typeof class which you want to search in.</typeparam>
        /// <typeparam name="TA">Attribute type.</typeparam>
        /// <param name="obj">Instance which you are search in.</param>
        /// <param name="flags">Reflection binding flags.</param>
        /// <returns>Array of property info.</returns>
        public static PropertyInfo[] GetPropertiesWithAttribute<T, TA>(
          this T obj,
          BindingFlags flags = DEFAULT_FLAGS) where TA : Attribute
        {
            return obj.GetType().GetProperties(flags).Where(p => p.IsDefined(typeof(TA), false)).ToArray();
        }

        /// <summary>
        /// Check if given object is decorated by out Attribute param.
        /// </summary>
        /// <typeparam name="T">Type o object.</typeparam>
        /// <typeparam name="TA">Type o attribute.</typeparam>
        /// <param name="obj">Object that you want to search for attribute on.</param>
        /// <param name="attr">Out attribute result if exists.</param>
        /// <returns>Value indicating whether attribute is defined on given object.</returns>
        public static bool TryToGetAttributeOfType<T, TA>(this T obj, out TA attr) where TA : Attribute
        {
            var result = (TA)obj.GetType().GetCustomAttributes(typeof(TA), true).FirstOrDefault();
            return (attr = result) != null;
        }

        /// <summary>
        /// Gets all the properties that can be threat like TP.
        /// </summary>
        /// <typeparam name="T">Type of object that we are searching on.</typeparam>
        /// <typeparam name="TP">Type of property - for example <see cref="IDisposable"/> if you want to retrieve all the disposable properties.</typeparam>
        /// <param name="obj">Object instance that you want to search on.</param>
        /// <returns>Array of properties.</returns>
        public static PropertyInfo[] GetAssignableProperties<T, TP>(this T obj)
        {
            return obj.GetType().GetProperties(DEFAULT_FLAGS).Where(p => typeof(TP).IsAssignableFrom(p.PropertyType))
              .ToArray();
        }

        /// <summary>
        /// Gets all the fields that can be threat like TF.
        /// </summary>
        /// <typeparam name="T">Type of object that we are searching on.</typeparam>
        /// <typeparam name="TF">Type of field - for example <see cref="IDisposable"/> if you want to retrieve all the disposable fields.</typeparam>
        /// <param name="obj">Object instance that you want to search on.</param>
        /// <returns>Array of fields.</returns>
        public static FieldInfo[] GetAssignableFields<T, TF>(this T obj)
        {
            return obj.GetType().GetFields(DEFAULT_FLAGS).Where(f => typeof(TF).IsAssignableFrom(f.FieldType)).ToArray();
        }

        /// <summary>
        /// Gets all the properties decorated with given attribute.
        /// </summary>
        /// <typeparam name="T">Type of object that we are searching on.</typeparam>
        /// <typeparam name="TP">Type of attribute above property.</typeparam>
        /// <param name="obj">Object instance that you want to search on.</param>
        /// <returns>Array of properties.</returns>
        public static PropertyInfo[] GetPropertiesWithAttribute<T, TP>(this T obj) where TP : Attribute
        {
            return obj.GetType().GetProperties(DEFAULT_FLAGS).Where(p => p.IsDefined(typeof(TP), false)).ToArray();
        }

        /// <summary>
        /// Gets all the fields decorated with given attribute.
        /// </summary>
        /// <typeparam name="T">Type of object that we are searching on.</typeparam>
        /// <typeparam name="TF">Type of attribute above field.</typeparam>
        /// <param name="obj">Object instance that you want to search on.</param>
        /// <returns>Array of fields.</returns>
        public static FieldInfo[] GetFieldsWithAttribute<T, TF>(this T obj) where TF : Attribute
        {
            return obj.GetType().GetFields(DEFAULT_FLAGS).Where(f => f.IsDefined(typeof(TF), false)).ToArray();
        }

        /// <summary>
        /// Gets all the methods decorated with given attribute.
        /// </summary>
        /// <typeparam name="T">Type of object that we are searching on.</typeparam>
        /// <typeparam name="TM">Type of attribute above method.</typeparam>
        /// <param name="obj">Object instance that you want to search on.</param>
        /// <returns>Array of methods.</returns>
        public static MethodInfo[] GetMethodsWithAttribute<T, TM>(this T obj) where TM : Attribute
        {
            return obj.GetType().GetMethods(DEFAULT_FLAGS).Where(m => m.IsDefined(typeof(TM), false)).ToArray();
        }

        /// <summary>
        /// Gets all members info decorated with given attribute.
        /// </summary>
        /// <typeparam name="T">Type of object that we are searching on.</typeparam>
        /// <typeparam name="TF">Type of attribute above member.</typeparam>
        /// <param name="obj">Object instance that you want to search on.</param>
        /// <returns>Array of members info.</returns>
        public static MemberInfo[] GetMembersInfoWithAttribute<T, TF>(this T obj) where TF : Attribute
        {
            return obj.GetType().GetMembers(DEFAULT_FLAGS).Where(f => f.IsDefined(typeof(TF), false)).ToArray();
        }
        
        public static void ClearCache()
        {
            _cachedTypes = null;
        }
        
        public static List<Type> GetAllTypes()
        {
            if(_cachedTypes != null) return _cachedTypes;
            
            _cachedTypes = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var types = assembly.GetTypes();
                    _cachedTypes.AddRange(types);
                }
                catch
                {
                    // Ignore types loading exception
                    continue;
                }
            }

            return _cachedTypes;
        }

        public static IEnumerable<TypeWithAttribute<T>> GetAllTypesByAttribute<T>() where T : Attribute
        {
            var types = GetAllTypes().Where(t => t.IsDefined(typeof(T)));
            foreach (Type type in types)
            {
                var attribute = type.GetCustomAttribute<T>(true);
                if (attribute != null)
                {
                    yield return new TypeWithAttribute<T>(type, attribute);
                }
            }
        }

        public static IEnumerable<Type> FindTypesWithAttribute<T>() where T : Attribute
        {
            var types = GetAllTypes();
            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<T>();
                if (attribute != null)
                {
                    yield return type;
                }
            }
        }
        
        public static IEnumerable<MethodWithAttribute<T>> FindStaticMethodsWithAttribute<T>() where T : Attribute
        {
            var types = GetAllTypes();
            foreach (var type in types)
            {
                foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    var attribute = method.GetCustomAttribute<T>();
                    if (attribute != null)
                    {
                        yield return new MethodWithAttribute<T>(method, attribute);
                    }
                }
            }
        }

        /// <summary>
        /// Find methods decorated with T Attribute but only in given TT type.
        /// </summary>
        /// <typeparam name="T">Attribute type.</typeparam>
        /// <typeparam name="TT">Instance type.</typeparam>
        /// <returns>Methods with this attribute from TT type.</returns>
        public static IEnumerable<MethodWithAttribute<T>> FindStaticMethodsWithAttribute<T, TT>() where T : Attribute where TT : Type
        {
            var methods = typeof(TT).GetMethods(BindingFlags.Static | BindingFlags.Public);

            foreach (var m in methods)
            {
                var attr = m.GetCustomAttribute<T>();
                if (attr != null)
                {
                    yield return new MethodWithAttribute<T>(m, attr);
                }
            }
        }
    }

    public struct TypeWithAttribute<T>
    {
        public TypeWithAttribute(Type type, T attribute)
        {
            Type = type;
            Attribute = attribute;
        }
        
        public readonly Type Type;
        public readonly T Attribute;
        
        public static implicit operator Type(TypeWithAttribute<T> typeWithAttribute)
        {
            return typeWithAttribute.Type;
        }
        
        public static implicit operator T(TypeWithAttribute<T> typeWithAttribute)
        {
            return typeWithAttribute.Attribute;
        }
    }
    
    public struct MethodWithAttribute<T>
    {
        public MethodWithAttribute(MethodInfo method, T attribute)
        {
            Method = method;
            Attribute = attribute;
        }

        public readonly MethodInfo Method;
        public readonly T Attribute;
        
        public static implicit operator MethodInfo(MethodWithAttribute<T> methodWithAttribute)
        {
            return methodWithAttribute.Method;
        }
        
        public static implicit operator T(MethodWithAttribute<T> methodWithAttribute)
        {
            return methodWithAttribute.Attribute;
        }
    }
}