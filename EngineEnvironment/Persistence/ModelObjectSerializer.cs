using System;
using System.Collections;
using System.Collections.Generic;
using MHGameWork.TheWizards.Collections;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Reflection;
using MHGameWork.TheWizards.Serialization;

namespace MHGameWork.TheWizards.Persistence
{
    /// <summary>
    /// Responsible for string-serializing ModelObjects
    /// Part of the ModelSerializer
    /// </summary>
    public class ModelObjectSerializer : IConditionalSerializer
    {
        private readonly IIDResolver resolver;

        public ModelObjectSerializer(IIDResolver resolver)
        {
            this.resolver = resolver;
        }

        public bool CanOperate(Type type)
        {
            return typeof(IModelObject).IsAssignableFrom(type);
        }

        public string Serialize(object obj, Type type, StringSerializer stringSerializer)
        {
            var modelObject = (IModelObject)obj;

            return stringSerializer.Serialize(resolver.GetObjectID(modelObject), typeof(int));
        }

        public object Deserialize(string value, Type type, StringSerializer stringSerializer)
        {
            var objectID = (int)stringSerializer.Deserialize(value, typeof(int));
            var modelObject = resolver.GetObjectByID(objectID);
            return modelObject;
        }



        /// <summary>
        /// Resolves ID's for modelobjects
        /// </summary>
        public interface IIDResolver
        {
            int GetObjectID(IModelObject obj);
            IModelObject GetObjectByID(int id);

        }
    }
}