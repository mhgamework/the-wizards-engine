using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MHGameWork.TheWizards.Forms;

namespace MHGameWork.TheWizards.Reflection
{
    /// <summary>
    /// Helper methods for c# reflection
    /// 
    /// Note: This class provides methods to access fields and properties on a type like they are one and the same concept
    /// Note: also look at the postharp reflectionhelper for this functionality
    /// </summary>
    public static class ReflectionHelper
    {
        public static List<IAttribute> GetAllAttributes(Type type)
        {
            var ret = new List<IAttribute>();
            foreach (var fi in type.GetFields())
            {
                if (!fi.IsPublic) continue;

                ret.Add(new FieldAttribute(fi));
            }
            foreach (var fi in type.GetProperties())
            {
                if (!fi.CanRead || !fi.CanWrite || fi.GetGetMethod() == null || fi.GetSetMethod() == null || !fi.GetGetMethod().IsPublic || !fi.GetSetMethod().IsPublic) continue;

                ret.Add(new PropertyAttribute(fi));
            }
            return ret;
        }

        public static IAttribute GetAttributeByName(Type type, string name)
        {
            var field = type.GetFields().FirstOrDefault(f => f.Name == name);
            if (field != null)
            {
                return new FieldAttribute(field);
            }

            var property = type.GetProperties().FirstOrDefault(f => f.Name == name);
            if (property != null)
            {
                return new PropertyAttribute(property);
            }

            return null;

        }

        public static bool IsGenericType(Type targetType, Type genericType)
        {
            return targetType.IsGenericType && targetType.GetGenericTypeDefinition() ==genericType;
        }
        public static Type GetGenericListType(Type type)
        {
           return type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>)).GetGenericArguments()[0];
        }


        /// <summary>
        /// Returns the default value for type t
        /// </summary>
        public static object GetDefaultValue(Type t)
        {
            object val = null;
            if (t.IsValueType)
                val = Activator.CreateInstance(t);
            return val;
        }

        /// <summary>
        /// Returns whether the given method is a property getter or setter
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool IsMethodFromProperty(MethodInfo method)
        {
            return GetPropertyForMethod(method) != null;
        }

        /// <summary>
        /// Returns the property corresponding to the given method setter or getter
        /// returns null when method is not a property method
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyForMethod(MethodInfo method)
        {
            return method.DeclaringType.GetProperties().FirstOrDefault((prop => prop.GetSetMethod() == method || prop.GetGetMethod() == method));
        }


    }
}
