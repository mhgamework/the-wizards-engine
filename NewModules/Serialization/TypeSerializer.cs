using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.Serialization
{
    public interface ITypeSerializer
    {
        string Serialize(Type t);

        /// <summary>
        /// Resolves a remote type 'FullName' to a local loaded Type (this could require more specific logic later on)
        /// Note: probably INCREDIBLY SLOW
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        Type Deserialize(string value);
    }

    /// <summary>
    /// This class provides a way to serialize a type to a string and back
    /// Currently this is a dummy implementation
    /// TODO: maybe move to better location
    /// </summary>
    public class TypeSerializer : ITypeSerializer
    {

        private readonly TWEngine engine;

        public TypeSerializer(TWEngine engine)
        {
            this.engine = engine;
        }

        public string Serialize(Type t)
        {
            return t.FullName;
        }

        /// <summary>
        /// This is a simple caching system, so the search is only done once :P
        /// </summary>
        private Dictionary<string, Type> cache = new Dictionary<string, Type>();

        /// <summary>
        /// Resolves a remote type 'FullName' to a local loaded Type (this could require more specific logic later on)
        /// Note: probably INCREDIBLY SLOW
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Type Deserialize(string value)
        {

            Type ret = null;
            //if (cache.TryGetValue(value, out ret))
            //    return ret;

            ret = engine.GetLoadedGameplayAssembly().GetTypes().First(o => o.FullName == value);

            //if (ret != null)
            //    cache.Add(value, ret);

            return ret;
        }


    }
    /// <summary>
    /// Loads from any loaded type!
    /// </summary>
    public class LoadedTypeSerializer : ITypeSerializer
    {

        public string Serialize(Type t)
        {
            return t.FullName;
        }

        /// <summary>
        /// This is a simple caching system, so the search is only done once :P
        /// </summary>
        private Dictionary<string, Type> cache = new Dictionary<string, Type>();

        /// <summary>
        /// Resolves a remote type 'FullName' to a local loaded Type (this could require more specific logic later on)
        /// Note: probably INCREDIBLY SLOW
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Type Deserialize(string value)
        {
            Type ret;
            if (cache.TryGetValue(value, out ret))
                return ret;

            ret = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).First(o => o.FullName == value);

            if (ret != null)
                cache.Add(value, ret);

            return ret;
        }


    }
}
