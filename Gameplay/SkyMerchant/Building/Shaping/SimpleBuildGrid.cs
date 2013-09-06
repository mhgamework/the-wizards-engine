using System.Collections.Generic;
using System.Linq;
using DirectX11;

namespace MHGameWork.TheWizards.SkyMerchant.Building.Shaping
{
    public class SimpleBuildGrid:IBuildGrid
    {
        private Dictionary<Point3, SimpPart> simpParts = new Dictionary<Point3, SimpPart>();

        public bool HasItemAt(Point3 pos)
        {
            return simpParts.ContainsKey(pos);
        }

        public SimpPart GetItemAt(Point3 pos)
        {
            SimpPart retPart ;
            simpParts.TryGetValue(pos, out retPart);
            return retPart;
        }
        public bool RemoveItem(SimpPart part)
        {
            var key = new Point3(int.MaxValue,int.MinValue,int.MaxValue);
            foreach (var simpPart in simpParts.Where(simpPart => simpPart.Value == part))
            {
                key = simpPart.Key;
            }
            if (key == new Point3(int.MaxValue,int.MinValue,int.MaxValue))
                return false;

                simpParts.Remove(key);
            return true;
        }

        public Dictionary<Point3,SimpPart> GetAllBlocks()
        {
            return simpParts;
        }

        public void AddItemAt(Point3 pos,SimpPart item)
        {
            simpParts.Add(pos,item);
        }
    }

}