using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.XML
{
    public class TWXmlSerializer<T> : IInternalSerializer
    {

        private Dictionary<Type, ICustomElementSerializer> elementSerializers = new Dictionary<Type, ICustomElementSerializer>();
        private List<ICustomSerializer> serializers = new List<ICustomSerializer>();
        /// <summary>
        /// Objects that are castable to give type are serialized with given serializer
        /// </summary>
        public void AddCustomElementSerializer(Type type, ICustomElementSerializer elementSerializer)
        {
            elementSerializers.Add(type, elementSerializer);
        }
        /// <summary>
        /// Objects that are castable to give type are serialized with given serializer
        /// </summary>
        public void AddCustomSerializer(ICustomSerializer serializer)
        {
            serializers.Add(serializer);
        }

        public TWXmlSerializer()
        {
            AddCustomSerializer(new FastArraySerializer());
        }


        public void Serialize(T obj, Stream strm)
        {
            var type = typeof(T);

            var rootNode = new TWXmlNode(TWXmlNode.CreateXmlDocument(), type.Name);

            serializeObject(type, rootNode, obj);


            rootNode.Document.Save(strm);
        }
        public void Deserialize(T obj, Stream strm)
        {
            var type = typeof(T);

            var rootNode = TWXmlNode.GetRootNodeFromStream(strm);
            deserializeObject(type, rootNode, obj);
        }

        private void serializeObject(Type type, TWXmlNode node, object obj)
        {
            foreach (var fieldInfo in type.GetFields())
            {
                object value = fieldInfo.GetValue(obj);
                if (value == null) continue;


                serializeElement(node.CreateChildNode(fieldInfo.Name), fieldInfo.FieldType, value);
            }
            foreach (var propertyInfo in type.GetProperties())
            {
                if (propertyInfo.GetGetMethod() == null || propertyInfo.GetSetMethod() == null)
                    continue;
                if (propertyInfo.GetIndexParameters().Length != 0)
                    continue;

                object value = propertyInfo.GetValue(obj, null);
                if (value == null) continue;

                serializeElement(node.CreateChildNode(propertyInfo.Name), propertyInfo.PropertyType, value);
            }
        }
        private void deserializeObject(Type type, TWXmlNode node, object obj)
        {
            foreach (var fieldInfo in type.GetFields())
            {
                var childNode = node.FindChildNode(fieldInfo.Name);

                if (childNode == null)
                    continue;


                fieldInfo.SetValue(obj, deserializeElement(childNode, fieldInfo.FieldType));


            }
            foreach (var propertyInfo in type.GetProperties())
            {
                if (propertyInfo.GetGetMethod() == null || propertyInfo.GetSetMethod() == null)
                    continue;
                if (propertyInfo.GetIndexParameters().Length != 0)
                    continue;

                var childNode = node.FindChildNode(propertyInfo.Name);

                if (childNode == null)
                    continue;

                propertyInfo.SetValue(obj, deserializeElement(childNode, propertyInfo.PropertyType), null);
            }
        }

        void IInternalSerializer.serializeElement(TWXmlNode node, Type type, object value)
        {
            serializeElement(node, type, value);
        }
        object IInternalSerializer.deserializeElement(TWXmlNode node, Type type)
        {
            return deserializeElement(node, type);
        }


        /// <summary>
        /// The node is the node for this element, not for the parent object
        /// </summary>
        private void serializeElement(TWXmlNode node, Type type, object value)
        {
            if (value == null)
            {
                return;
            }
            foreach (var customSerializer in elementSerializers)
            {
                if (!type.IsAssignableFrom(customSerializer.Key))
                    continue;

                customSerializer.Value.Serialize(node, value);
                return;


            }
            for (int i = 0; i < serializers.Count; i++)
            {
                var s = serializers[i];

                if (s.SerializeElement(node, type, value, this)) return;

            }

            if (type == typeof(float)
                || type == typeof(string)
                || type == typeof(Int16)
                || type == typeof(Int32)
                || type == typeof(Int64))
                node.Value = value.ToString();
            else if (type.IsEnum)
            {
                node.Value = value.ToString();

            }
        
            else if (type.IsArray)
            {
                int count;
                Type elementType;
                count = (int)type.GetProperty("Length").GetValue(value, null);
                elementType = type.GetElementType();
                var getMethod = type.GetMethod("Get");
                for (int i = 0; i < count; i++)
                {
                    serializeElement(
                        node.CreateChildNode(elementType.Name),
                        elementType,
                        getMethod.Invoke(value, new object[] { i }));
                }
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                int count;
                Type elementType;
                elementType = type.GetGenericArguments()[0];
                count = (int)type.GetProperty("Count").GetValue(value, null);
                var indexerProperty = (PropertyInfo)type.GetDefaultMembers()[0];
                for (int i = 0; i < count; i++)
                {

                    serializeElement(
                        node.CreateChildNode(elementType.Name),
                        elementType,
                        indexerProperty.GetValue(value, new object[] { i }));
                }
            }
            else if (type == typeof(Color))
                XMLSerializer.WriteColor(node, (Color)value);
            else if (type == typeof(Matrix))
                XMLSerializer.WriteMatrix(node, (Matrix)value);


            else
            {
                if (type.IsPrimitive)
                    throw new InvalidOperationException("Can't serialize primitive type");

                serializeObject(type, node, value);

            }
        }
        /// <summary>
        /// The node is the node for this element, not for the parent object
        /// </summary>
        private object deserializeElement(TWXmlNode node, Type type)
        {
            if (!type.IsValueType && !node.HasChildNodes)
                return null;

            foreach (var customSerializer in elementSerializers)
            {
                if (!type.IsAssignableFrom(customSerializer.Key))
                    continue;

                return customSerializer.Value.Deserialize(node);


            }

            for (int i = 0; i < serializers.Count; i++)
            {
                var s = serializers[i];
                object obj;
                if (s.DeserializeElement(node, type, out obj, this)) return obj;

            }

            if (type == typeof(float))
                return float.Parse(node.Value);
            if (type == typeof(string))
                return node.Value;
            if (type == typeof(Int16))
                return Int16.Parse(node.Value);
            if (type == typeof(Int32))
                return Int32.Parse(node.Value);
            if (type == typeof(Int64))
                return Int64.Parse(node.Value);

            if (type.IsEnum)
                return Enum.Parse(type, node.Value);
           
            if (type.IsArray)
            {
                var childNodes = node.GetChildNodes();
                var array = (IList)Activator.CreateInstance(type, childNodes.Length);
                int i = 0;
                var elementType = type.GetElementType();

                foreach (var arrayChildNode in childNodes)
                {
                    array[i] = deserializeElement(arrayChildNode, elementType);
                    i++;
                }
                return array;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var childNodes = node.GetChildNodes();
                var array = (IList)Activator.CreateInstance(type);
                var elementType = type.GetGenericArguments()[0];

                foreach (var arrayChildNode in childNodes)
                {
                    array.Add(deserializeElement(arrayChildNode, elementType));
                }
                return array;
            }
            if (type == typeof(Color))
                return XMLSerializer.ReadColor(node);
            if (type == typeof(Matrix))
                return XMLSerializer.ReadMatrix(node);


            if (type.IsPrimitive)
                throw new InvalidOperationException("Can't serialize primitive type");

            var childObject = Activator.CreateInstance(type);

            deserializeObject(type, node, childObject);

            return childObject;

        }



    }
}
