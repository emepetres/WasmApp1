// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WaveEngine.Common.Helpers
{
    /// <summary>
    /// Helper methods for reflection.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// List of castable primitive types by each primitive type.
        /// </summary>
        private static readonly Dictionary<Type, List<Type>> castablePrimitiveTypes = new Dictionary<Type, List<Type>>()
        {
            { typeof(decimal), new List<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(char) } },
            { typeof(double), new List<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(char), typeof(float) } },
            { typeof(float), new List<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(char), typeof(float) } },
            { typeof(ulong), new List<Type> { typeof(byte), typeof(ushort), typeof(uint), typeof(char) } },
            { typeof(long), new List<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(char) } },
            { typeof(uint), new List<Type> { typeof(byte), typeof(ushort), typeof(char) } },
            { typeof(int), new List<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(char) } },
            { typeof(ushort), new List<Type> { typeof(byte), typeof(char) } },
            { typeof(short), new List<Type> { typeof(byte) } },
        };

        /// <summary>
        /// Determines whether the specified <see cref="Type" /> is enum.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="Type" /> is a enum type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEnum(this Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }

        /// <summary>
        /// Determines whether the specified type is an interface.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="Type"/> is a enum type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInterface(this Type type)
        {
            return type.GetTypeInfo().IsInterface;
        }

        /// <summary>
        /// Gets a value indicating whether the specified type is a value type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="Type"/> is a value type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }

        /// <summary>
        /// Gets a value indicating whether the specified <see cref="Type" /> is a generic type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="Type"/> is a generic type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }

        /// <summary>
        /// Gets a value indicating whether the specified type is a generic type definition.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="Type"/> is a generic type definition; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsGenericTypeDefinition(this Type type)
        {
            return type.GetTypeInfo().IsGenericTypeDefinition;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Type" /> is one of the primitive types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="Type" /> is one of the primitive types; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPrimitive(this Type type)
        {
            return type.GetTypeInfo().IsPrimitive;
        }

        /// <summary>
        /// Determines whether an instance of the second specified type can be assigned to the first specified type instance.
        /// </summary>
        /// <param name="first">The first type.</param>
        /// <param name="second">The type to check.</param>
        /// <returns>
        /// <c>true</c> if the first specified type can be assigned from the second type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAssignableFrom(this Type first, Type second)
        {
            return first.GetTypeInfo().IsAssignableFrom(second.GetTypeInfo());
        }

        /// <summary>
        /// Determines whether an instance of the second specified type can be casted to the first specified type instance.
        /// </summary>
        /// <param name="first">The first type.</param>
        /// <param name="second">The type to check.</param>
        /// <returns>
        /// <c>true</c> if the first specified type can be casted from the second type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCastableFrom(this Type first, Type second)
        {
            bool castable = false;

            if (first.IsAssignableFrom(second) ||
                castablePrimitiveTypes.Any(p => p.Key == second && p.Value.Contains(first)) ||
                second.GetMethods(BindingFlags.Public | BindingFlags.Static).Any(m => m.ReturnType == first && (m.Name == "op_Implicit" || m.Name == "op_Explicit")))
            {
                castable = true;
            }
            else
            {
                if (first.GetTypeInfo().IsEnum && second == typeof(int))
                {
                    castable = true;
                }
            }

            return castable;
        }

        /// <summary>
        /// Indicates whether custom attributes of a specified type are applied to a specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>
        /// <c>true</c> if an attribute of the specified type is applied to the specified type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAttributeDefined<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            return type.GetTypeInfo().IsDefined(typeof(TAttribute));
        }

        /// <summary>
        /// Gets the <see cref="Assembly" /> in which the member type is declared. For generic types, gets the <see cref="Assembly" /> in which the generic type is defined.
        /// </summary>
        /// <param name="obj">The member.</param>
        /// <returns>
        /// The type assembly.
        /// </returns>
        public static Assembly GetMemberAssembly(this object obj)
        {
            return obj.GetType().GetTypeInfo().Assembly;
        }

        /// <summary>
        /// Gets the <see cref="Assembly" /> in which the type is declared. For generic types, gets the <see cref="Assembly" /> in which the generic type is defined.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The type assembly.
        /// </returns>
        public static Assembly GetTypeAssembly(this Type type)
        {
            return type.GetTypeInfo().Assembly;
        }

        /// <summary>
        /// Gets the <see cref="Assembly"/> name in which the type is declared. For generic types, gets the <see cref="Assembly"/> name in which the generic type is defined.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A <see cref="string"/> containing the name of this member assembly.
        /// </returns>
        public static string GetTypeAssemblyName(this Type type)
        {
            return type.GetTypeInfo().Assembly.GetName().Name;
        }

        /// <summary>
        /// Gets the type from which the current System.Type directly inherits.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The <see cref="Type"/> from which the current System.Type directly inherits, or null
        /// if the current <see cref="Type"/> represents the System.Object class or an interface.
        /// </returns>
        public static Type GetBaseType(this Type type)
        {
            return type.GetTypeInfo().BaseType;
        }

        /// <summary>
        /// Returns a <see cref="Type"/> object that represents a generic type definition from which the current generic type can be constructed.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A <see cref="Type"/> object representing a generic type from which the current type can be constructed.
        /// </returns>
        public static Type GetGenericTypeDefinition(this Type type)
        {
            return type.GetTypeInfo().GetGenericTypeDefinition();
        }

        /// <summary>
        /// Gets the interfaces.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The interfaces.</returns>
        public static Type[] GetInterfaces(Type type)
        {
            return type.GetInterfaces();
        }

        /// <summary>
        /// Gets the name of the current member.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A <see cref="string"/> containing the name of this member type.
        /// </returns>
        public static string GetTypeName(this object obj)
        {
            return obj.GetType().GetTypeInfo().Name;
        }

        /// <summary>
        /// Gets the size of the specified type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The size of the specified type.</returns>
        public static int GetSizeOf<T>()
        {
#if NETSTANDARD1_6 || UAP
            return Marshal.SizeOf<T>();
#else
            return Marshal.SizeOf(typeof(T));
#endif
        }

        /// <summary>
        /// Gets the full name of the name of the specified type without assembly full name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The full name of the name without assembly full name.</returns>
        public static string GetFullNameWithoutAssemblyInfo(this Type type)
        {
            string result;

            var typeInfo = type.GetTypeInfo();

            if (typeInfo.IsGenericType &&
                !typeInfo.IsGenericTypeDefinition)
            {
                var typeFullName = type.GetGenericTypeDefinition().FullName;
                var genericFullNames = type.GenericTypeArguments.Select(t => t.GetFullNameWithoutAssemblyInfo());
                result = typeFullName + "[" + string.Join(",", genericFullNames) + "]";
            }
            else
            {
                result = type.FullName;
            }

            return result;
        }

        /// <summary>
        /// Find the types that match the corresponding predicate.
        /// </summary>
        /// <param name="assembly">The target search assembly.</param>
        /// <param name="predicate">The predicate to filter types.</param>
        /// <returns>A collection of matching types.</returns>
        public static IEnumerable<Type> FindTypes(Assembly assembly, Func<Type, bool> predicate)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (!assembly.IsDynamic)
            {
                Type[] exportedTypes = null;
                try
                {
                    exportedTypes = assembly.GetExportedTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    exportedTypes = e.Types;
                }

                if (exportedTypes != null)
                {
                    foreach (var type in exportedTypes)
                    {
                        if (predicate(type))
                        {
                            yield return type;
                        }
                    }
                }
            }
        }
    }
}
