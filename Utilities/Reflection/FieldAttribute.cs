using System;
using System.Reflection;

namespace MHGameWork.TheWizards.Reflection
{
    public class FieldAttribute : IAttribute
    {
        public Type Type { get { return fi.FieldType; } }
        public string Name { get { return fi.Name; } }
        private readonly FieldInfo fi;
        public FieldAttribute(FieldInfo fi)
        {
            this.fi = fi;
        }


        public object GetData(object obj)
        {
            return fi.GetValue(obj);
        }

        public void SetData(object obj, object value)
        {
            fi.SetValue(obj, value);
        }

        public object[] GetCustomAttributes()
        {
            return fi.GetCustomAttributes(false);
        }
    }
}