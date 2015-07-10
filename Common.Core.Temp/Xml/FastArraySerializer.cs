using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics.Xna.XML;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Xml
{
    public class FastArraySerializer : ICustomSerializer
    {
        public bool SerializeElement(TWXmlNode node, Type type, object value, IInternalSerializer s)
        {
                 if (type == typeof(Vector3[]))
            {
                var array = (Vector3[])value;
                var floats = new float[array.Length * 3];
                for (int i = 0; i < array.Length; i++)
                {
                    floats[i * 3 + 0] = array[i].X;
                    floats[i * 3 + 1] = array[i].Y;
                    floats[i * 3 + 2] = array[i].Z;
                }
                XMLSerializer.WriteFloatArray(node, floats);
                return true;
            }
            if (type == typeof(Vector2[]))
            {
                var array = (Vector2[])value;
                var floats = new float[array.Length * 2];
                for (int i = 0; i < array.Length; i++)
                {
                    floats[i * 2 + 0] = array[i].X;
                    floats[i * 2 + 1] = array[i].Y;
                }
                XMLSerializer.WriteFloatArray(node, floats);
                return true;
            }
            if (type == typeof(int[]))
            {
                var array = (int[])value;
                XMLSerializer.WriteIntArray(node, array);
                return true;
            }


            return false;
        }

        public bool DeserializeElement(TWXmlNode node, Type type, out object value, IInternalSerializer s)
        {
            if (type == typeof(Vector3[]))
            {
                var floats = XMLSerializer.ReadFloatArray(node);
                var array = new Vector3[floats.Length / 3];

                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = new Vector3(floats[i * 3 + 0], floats[i * 3 + 1], floats[i * 3 + 2]);
                }
                value =  array;
                return true;
            }
            if (type == typeof(Vector2[]))
            {
                var floats = XMLSerializer.ReadFloatArray(node);
                var array = new Vector2[floats.Length / 2];

                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = new Vector2(floats[i * 2 + 0], floats[i * 2 + 1]);
                }
                value =  array;
                return true;
            }
            if (type == typeof(int[]))
            {
                value = XMLSerializer.ReadIntArray(node);
                return true;
            }
            value = null;
            return false;
        }
    }
}
