using System;
using System.Collections.Immutable;
using System.Reflection;

namespace Tableau.Migration.Tests
{
    public static class ReflectionExtensions
    {
        public static object? GetFieldValue(this Type type, string fieldName, object obj)
        {
            return type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(obj);
        }

        public static object? GetFieldValue(this object obj, string fieldName)
        {
            return GetFieldValue(obj.GetType(), fieldName, obj);
        }

        public static object? GetFieldValue(this object obj, Type type, string fieldName)
        {
            return GetFieldValue(type, fieldName, obj);
        }

        public static object? GetFieldValue(this Type type, string fieldName)
        {
            return type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null);
        }

        public static TValue? GetFieldValue<TValue>(this Type type, string fieldName)
            => (TValue?)type.GetFieldValue(fieldName);

        public static bool HasGenericTypeDefinition(this Type type, Type genericTypeDefinition)
        {
            if (!genericTypeDefinition.IsGenericTypeDefinition)
                throw new ArgumentException($"Type {genericTypeDefinition.FullName} is not a generic type definition.");

            return type.IsConstructedGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition;
        }

        public static bool IsConcrete(this Type type) => !type.IsInterface && !type.IsAbstract;

        public static bool IsAssignableToAny(this Type type, params Type[] targetTypes)
        {
            foreach (var targetType in targetTypes)
            {
                if (type.IsAssignableTo(targetType))
                    return true;
            }

            return false;
        }

        public static bool IsAssignableFromAny(this Type type, params Type[] targetTypes)
        {
            foreach (var targetType in targetTypes)
            {
                if (type.IsAssignableFrom(targetType))
                    return true;
            }

            return false;
        }

        public static IImmutableList<Type> GetBaseTypes(this Type type)
        {
            var baseTypes = ImmutableArray.CreateBuilder<Type>();

            var baseType = type.BaseType;

            while (baseType is not null)
            {
                baseTypes.Add(baseType);
                baseTypes.AddRange(baseType.GetBaseTypes());

                baseType = baseType.BaseType;
            }

            return baseTypes.ToImmutable();
        }
    }
}
