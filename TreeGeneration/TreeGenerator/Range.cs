using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics.Xna.XML;
using MHGameWork.TheWizards.ServerClient;

namespace TreeGenerator
{
    public struct Range
    {
        public float Min;
        public float Max;

        public Range(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public static Range  LoadFromXML(TWXmlNode node)
        {
            Range range=new Range();
            range.Min = float.Parse(node.ReadChildNodeValue("Min"));
            range.Max = float.Parse(node.ReadChildNodeValue("Max"));
            return range;
        }

        public static void WriteToXML(TWXmlNode node, Range range)
        {
            node.AddChildNode("Min",range.Min.ToString());
            node.AddChildNode("Max", range.Max.ToString());
        }

    }
}
