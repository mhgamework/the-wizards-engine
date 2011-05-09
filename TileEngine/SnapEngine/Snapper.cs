using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Editor;

namespace MHGameWork.TheWizards.TileEngine.SnapEngine
{
    public class Snapper
    {
        private List<ISnapObjectSnapper> snappers = new List<ISnapObjectSnapper>();

        public List<Transformation> SnapTo(SnapInformation obj, List<ISnappableWorldTarget> targets)
        {
            List<Transformation> transformations = new List<Transformation>();

            for (int i = 0; i < targets.Count; i++)
                for (int k = 0; k < obj.SnapList.Count; k++)
                {
                    var target = targets[i];
                    for (int l = 0; l < target.SnapInformation.SnapList.Count; l++)
                    {
                        var objSnapObjectType = obj.SnapList[k];
                        var targetSnapObjectType = target.SnapInformation.SnapList[l];


                        if (objSnapObjectType.TileFaceType.GetRoot() == targetSnapObjectType.TileFaceType.GetRoot())
                        {

                            for (int index = 0; index < snappers.Count; index++)
                            {
                                ISnapObjectSnapper t = snappers[index];

                                var snapper = t;


                                if (snapper.SnapObjectTypeA == objSnapObjectType.GetType() &&
                                    snapper.SnapObjectTypeB == targetSnapObjectType.GetType())
                                {
                                    snapper.SnapAToB(objSnapObjectType, targetSnapObjectType, target.Transformation, transformations);
                                }
                                else if (snapper.SnapObjectTypeB == objSnapObjectType.GetType() &&
                                         snapper.SnapObjectTypeA == targetSnapObjectType.GetType())
                                {
                                    snapper.SnapBToA(targetSnapObjectType, objSnapObjectType, target.Transformation, transformations);
                                }
                            }
                        }
                    }
                }


            return transformations;
        }

        public void AddSnapper(ISnapObjectSnapper snapper)
        {
            snappers.Add(snapper);
        }

        /*
        public ISnapObjectSnapper FindSnapper(Type a, Type b)
        {
            throw new NotImplementedException();
        }*/
    }
}
