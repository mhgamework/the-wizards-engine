using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.Raytracing
{
    /// <summary>
    /// Responsible for rendering Generic surfaces
    /// </summary>
    public class GenericTraceableScene : ITraceableScene
    {
        private List<IGenericSurface> entities = new List<IGenericSurface>();

        private SolidShadeCommand shadeCommand = new SolidShadeCommand(new Color4(Color.SkyBlue));

        public void AddGenericSurface(IGenericSurface surface)
        {
            entities.Add(surface);
        }

        public bool Intersect(RayTrace rayTrace, out IShadeCommand command, bool generateShadeCommand)
        {
            float? temp;

            float? closest = null;
            command = shadeCommand;

            foreach (var ent in entities)
            {
                IShadeCommand newCommand;
                ent.Intersects(ref rayTrace, out temp, out newCommand, generateShadeCommand);

                changeWhenCloser(ref closest, ref command, ref temp, ref newCommand);
                if (!generateShadeCommand && closest.HasValue)
                {
                    //Dont wait for generating shading commands since we dont need, just return hit
                    return true;
                }
            }

            return false;

        }

        public static void changeWhenCloser(ref float? closestHit, ref IShadeCommand closestCommand, ref float? newHit, ref IShadeCommand newCommand)
        {
            bool closer = false;

            if (!newHit.HasValue)
                closer = false;
            else if (!closestHit.HasValue)
                closer = true;
            else if (closestHit.Value > newHit.Value)
                closer = true;

            if (closer)
            {
                closestHit = newHit;
                closestCommand = newCommand;


            }
        }
    }
}
