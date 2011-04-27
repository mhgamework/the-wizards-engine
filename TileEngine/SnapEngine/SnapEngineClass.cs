using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.TileEngine
{
    public class SnapEngineClass
    {
        public List<SnapInformation> ActiveSnapList = new List<SnapInformation>();

        private float snapRange = 1;

        public void addSnapInformation(SnapInformation information)
        {
            ActiveSnapList.Add(information);
        }

        /*
        public void UpdateSnaps()
        {
            for (int i = 0; i < ActiveSnapList.Count-1; i++)
            {
                SnapInformation inf1 = ActiveSnapList[i];
                SnapInformation inf2 = ActiveSnapList[i+1];

                //Point to point only
                for(int j=0; j<inf1.SnapList.Count;j++)
                {
                    SnapPoint p1 = inf1.SnapList[j];
                        for(int k=0; k<inf2.SnapList.Count;k++)
                        {
                            SnapPoint p2 = inf2.SnapList[k];

                            if (isInSnapRange(p1, p2))
                            {

                            }
                        }
                }
                
                
            }
        }*/

        public void SnapTo(SnapInformation source, SnapInformation target)
        {
            //Point to point only
            for (int j = 0; j < target.SnapList.Count; j++)
            {
                SnapPoint pTarget = target.SnapList[j];
                for (int k = 0; k < source.SnapList.Count; k++)
                {
                    SnapPoint pSource = source.SnapList[k];

                    if (isSameType(pTarget, pSource) && isInSnapRange(pTarget, pSource))
                    {                        
                        source.Snap(pSource, pTarget);
                    }
                }
            }
        }

        private bool isSameType(SnapPoint p1, SnapPoint p2)
        {
            if (p1.SnapType == p2.SnapType)
                return true;
            else return false;
        }

        private bool isInSnapRange(SnapPoint p1, SnapPoint p2)
        {
            float distance = (p2.Position - p1.Position).Length();

            if (distance < snapRange)
                return true;
            else return false;
        }
    }
}
