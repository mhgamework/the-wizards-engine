using MHGameWork.TheWizards.ServerClient;

namespace MHGameWork.TheWizards.Rendering.Vegetation.Trees
{
    /// <summary>
    /// Makes sure that a random number between min and max is spreaded equally.
    /// Deviation ranges from 0 to 1 where 0 means equal spreading and 1 means that a specific
    /// number can be the same as the next one.
    /// </summary>
    public struct RangeSpreading
    {
        public float Min;
        public float Max;
        public float Deviation;
        public RangeSpreading(float min, float max, float deviation)
        {
            Min = min;
            Max = max;
            Deviation = deviation;
        }

        public static void WriteToXML(RangeSpreading rangeSpreading, TWXmlNode node)
        {
            node.AddChildNode("Min", rangeSpreading.Min.ToString());
            node.AddChildNode("Max", rangeSpreading.Max.ToString());
            node.AddChildNode("Deviation", rangeSpreading.Deviation.ToString());
        }

        public static RangeSpreading LoadFromXML(TWXmlNode node)
        {
            RangeSpreading range = new RangeSpreading();
            range.Min = float.Parse(node.ReadChildNodeValue("Min"));
            range.Max = float.Parse(node.ReadChildNodeValue("Max"));
            range.Deviation = float.Parse(node.ReadChildNodeValue("Deviation"));
            return range;
        }
    }
   
}
