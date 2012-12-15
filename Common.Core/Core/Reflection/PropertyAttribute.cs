using System;
using System.Collections.Generic;
using System.Reflection;

namespace MHGameWork.TheWizards.Reflection
{
    public class PropertyAttribute : IAttribute
    {
        private readonly PropertyInfo fi;
        public Type Type { get { return fi.PropertyType; } }
        public string Name { get { return fi.Name; } }

        public PropertyAttribute(PropertyInfo fi)
        {
            this.fi = fi;
        }


        public object GetData(object obj)
        {
            return fi.GetGetMethod().Invoke(obj, null);
        }

        public void SetData(object obj, object value)
        {
            fi.GetSetMethod().Invoke(obj, new[] { value });
        }

        public object[] GetCustomAttributes()
        {
            return fi.GetCustomAttributes(false);
        }
    }
}