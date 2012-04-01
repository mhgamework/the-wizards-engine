using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Forms;

namespace MHGameWork.TheWizards.Reflection
{
    /// <summary>
    /// This class provides methods to access fields and properties on a type like they are one and the same concept
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
                if (!fi.CanRead || !fi.CanWrite || !fi.GetGetMethod().IsPublic || !fi.GetSetMethod().IsPublic) continue;

                ret.Add(new PropertyAttribute(fi));
            }
            return ret;
        }

        public static IAttribute getAttributeByName(Type type, string name)
        {
            var field = type.GetFields().First(f => f.Name == name);
            if (field != null)
            {
                return new FieldAttribute(field);
            }

            var property = type.GetProperties().First(f => f.Name == name);
            if (property != null)
            {
                return new PropertyAttribute(property);
            }

            return null;

        }
    }
}
