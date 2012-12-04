using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Raycasting;

namespace MHGameWork.TheWizards.CG.Raytracing
{
    /// <summary>
    /// Responsible for rendering Generic surfaces
    /// </summary>
    public class GenericTraceableScene : ITraceableScene
    {
        private List<IGenericSurface> entities = new List<IGenericSurface>();

        private SolidShadeCommand shadeCommand = new SolidShadeCommand(new SlimDX.Color4(Color.SkyBlue));

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

                bool closer = false;

                if (!temp.HasValue)
                    closer = false;
                else if (!closest.HasValue)
                    closer = true;
                else if (closest.Value > temp.Value)
                    closer = true;

                if (closer)
                {
                    closest = temp;
                    command = newCommand;

                    if (temp.HasValue)
                    {
                        if (!generateShadeCommand)
                        {
                            //Dont wait for generating shading commands since we dont need, just return hit
                            return true;
                        }
                    }
                }
            }

            return false;

        }
    }
}
