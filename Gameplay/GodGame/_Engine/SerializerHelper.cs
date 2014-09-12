using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using MHGameWork.TheWizards.GodGame.Persistence;

namespace MHGameWork.TheWizards.GodGame._Engine
{
    /// <summary>
    /// Shorthand functions for serializing to bytes
    /// This class uses a custom binder which allows serializing gameplay classes when hotloaded.
    /// Can only serialize types in gameplay dll
    /// </summary>
    public static class SerializerHelper
    {
        public static T Deserialize<T>(byte[] sObj)
        {
            var b = new BinaryFormatter();
            b.Binder = new CustomBinder();
            T s;
            using (var strm = new MemoryStream(sObj))
            {
                s = (T)b.Deserialize(strm);
            }
            return s;
        }

        public static byte[] Serialize(object obj)
        {
            var b = new BinaryFormatter();
            using (var strm = new MemoryStream())
            {
                b.Serialize(strm, obj);
                return strm.ToArray();
            }
        }
        private class CustomBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                return TW.Data.GameplayAssembly.GetTypes().First(t => t.FullName == typeName);
            }
        }
    }
}