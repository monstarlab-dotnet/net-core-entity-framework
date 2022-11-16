using System.Collections;
using System.Collections.Concurrent;

namespace Monstarlab.EntityFramework.Extension.Utils;

internal static class TypeExtensions
{
    private static readonly ConcurrentDictionary<Type, object?> TypeDefaults = new();

    internal static object? GetDefaultValue(this Type type)
    {
        return type.GetTypeInfo().IsValueType
            ? TypeDefaults.GetOrAdd(type, Activator.CreateInstance)
            : null;
    }   
    
    internal static bool IsAssignableToGenericType(this Type givenType, Type genericType)
    {
        var interfaceTypes = givenType.GetInterfaces();

        if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
        {
            return true;
        }

        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
        {
            return true;
        }
        
        if (givenType.BaseType is null)
        {
            return false;
        }

        return IsAssignableToGenericType(givenType.BaseType, genericType);
    }
}

internal static class ReflectionExtensions
{
    internal static object? GetPropertyValue(this object obj, string name) =>
        obj?.GetType().GetProperty(name)?.GetValue(obj);
}

internal static class PropertyInfoExtensions
{
    internal static bool IsNavigationProperty(this PropertyInfo prop) =>
        prop.PropertyType.IsAssignableToGenericType(typeof(EntityBase<>));
    
    internal static bool IsCollectionNavigationProperty(this PropertyInfo prop) =>
        prop.PropertyType.IsAssignableTo(typeof(IEnumerable)) &&
        prop.PropertyType != typeof(string) &&
        prop.PropertyType.GenericTypeArguments[0].IsAssignableToGenericType(typeof(EntityBase<>));

    internal static bool IsReadOnly(this PropertyInfo prop) => 
        (prop.GetCustomAttribute(typeof(ReadOnlyAttribute), true) as ReadOnlyAttribute)?.IsReadOnly ?? false;
}