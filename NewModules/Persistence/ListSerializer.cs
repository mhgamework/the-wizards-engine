using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Reflection;
using MHGameWork.TheWizards.Serialization;

namespace MHGameWork.TheWizards.Persistence
{
    /// <summary>
    /// Responsible for serializing lists :)
    /// </summary>
    public class ListSerializer : IConditionalSerializer
    {
        private StringBuilder builder = new StringBuilder();


        public bool CanOperate(Type type)
        {
            return typeof(IList).IsAssignableFrom(type) && type.GetInterfaces().Where(i => ReflectionHelper.IsGenericType(i, typeof(IList<>))).Count() == 1;
        }

        public string Serialize(object obj, Type type, StringSerializer stringSerializer)
        {
            builder.Clear();
            var list = (IList)obj;

            // find generic type
            var elementType = ReflectionHelper.GetGenericListType(type);

            if (CanOperate(elementType))
                throw new InvalidOperationException("This serializer does not support nested arrays!!!");

            foreach (var el in list)
            {
                builder.AppendLine(stringSerializer.Serialize(el, elementType));
            }

            return builder.ToString();
        }


        public object Deserialize(string value, Type type, StringSerializer stringSerializer)
        {
            var list = (IList)Activator.CreateInstance(type);

            var reader = new StringReader(value);
            var elementType = ReflectionHelper.GetGenericListType(type);
            while (reader.Peek() != -1)
            {
                var el = reader.ReadLine();
                var obj = stringSerializer.Deserialize(el, elementType);

                list.Add(obj);

            }

            return list;
        }
    }
}
