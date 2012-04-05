using System;
using System.Collections.Generic;
using MHGameWork.TheWizards._XNA.Scripting.API;

namespace MHGameWork.TheWizards._XNA.Scene
{
    public class EntityData
    {
        private Dictionary<string, object> dataElements = new Dictionary<string, object>();

        /// <summary>
        /// Gets data element with given name or creates a new one
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDataElement<T> GetDataElement<T>(string name)
        {
            object obj;
            if (dataElements.TryGetValue(name, out obj))
            {
                if (!(obj is IDataElement<T>))
                    throw new InvalidOperationException("Data with given name exists, but it is not of given type!");

                return (IDataElement<T>)obj;
            }

            var ret = new SimpleDataElement<T>();
            obj = ret;
            addDataElement(name, obj);

            return ret;
        }

        /// <summary>
        /// This is the non-type safe method of above
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object GetDataElement(string name, Type type)
        {
            object obj;
            if (dataElements.TryGetValue(name, out obj))
                return obj;

            var genericType = typeof(SimpleDataElement<>).MakeGenericType(new [] { type });

            obj = Activator.CreateInstance(genericType);
            addDataElement(name, obj);
            return obj;
        }

        private void addDataElement(string name, object element)
        {
            dataElements[name] = element;
            
        }

        public bool HasDataElement(string name)
        {
            return dataElements.ContainsKey(name);
        }
    }
}
