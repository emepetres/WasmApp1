// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace WaveEngine.Common.Helpers
{
    /// <summary>
    /// Extensions for TypeInfo.
    /// </summary>
    public static class TypeInfoAllMemberExtensions
    {
        /// <summary>
        /// Get contructors of the type and its base types.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <returns>All the contructors.</returns>
        public static IEnumerable<ConstructorInfo> GetAllConstructors(this TypeInfo typeInfo)
        {
            return GetAll(typeInfo, ti => ti.DeclaredConstructors);
        }

        /// <summary>
        /// Get events of the type and its base types.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <returns>All the events.</returns>
        public static IEnumerable<EventInfo> GetAllEvents(this TypeInfo typeInfo)
        {
            return GetAll(typeInfo, ti => ti.DeclaredEvents);
        }

        /// <summary>
        /// Get fields of the type and its base types.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <returns>All the fields.</returns>
        public static IEnumerable<FieldInfo> GetAllFields(this TypeInfo typeInfo)
        {
            return GetAll(typeInfo, ti => ti.DeclaredFields);
        }

        /// <summary>
        /// Get members of the type and its base types.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <returns>All the members.</returns>
        public static IEnumerable<MemberInfo> GetAllMembers(this TypeInfo typeInfo)
        {
            return GetAll(typeInfo, ti => ti.DeclaredMembers);
        }

        /// <summary>
        /// Get methods of the type and its base types.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <returns>All the methods.</returns>
        public static IEnumerable<MethodInfo> GetAllMethods(this TypeInfo typeInfo)
        {
            return GetAll(typeInfo, ti => ti.DeclaredMethods);
        }

        /// <summary>
        /// Get nested types of the type and its base types.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <returns>All the nested types.</returns>
        public static IEnumerable<TypeInfo> GetAllNestedTypes(this TypeInfo typeInfo)
        {
            return GetAll(typeInfo, ti => ti.DeclaredNestedTypes);
        }

        /// <summary>
        /// Get properties of the type and its base types.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <returns>All the properties.</returns>
        public static IEnumerable<PropertyInfo> GetAllProperties(this TypeInfo typeInfo)
        {
            return GetAll(typeInfo, ti => ti.DeclaredProperties);
        }

        /// <summary>
        /// Helper method to iterate the type and its base types.
        /// </summary>
        /// <typeparam name="T">The returned member type.</typeparam>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="accessor">The func to get the members of the type.</param>
        /// <returns>The members.</returns>
        private static IEnumerable<T> GetAll<T>(TypeInfo typeInfo, Func<TypeInfo, IEnumerable<T>> accessor)
        {
            while (typeInfo != null)
            {
                foreach (var t in accessor(typeInfo))
                {
                    yield return t;
                }

                if (typeInfo.BaseType == null)
                {
                    typeInfo = null;
                }
                else
                {
                    typeInfo = typeInfo.BaseType.GetTypeInfo();
                }
            }
        }
    }
}
