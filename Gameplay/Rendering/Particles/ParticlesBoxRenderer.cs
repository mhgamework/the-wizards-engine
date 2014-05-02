using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.Engine.WorldRendering;
using System.Linq;
using MHGameWork.TheWizards.RTSTestCase1;
using SlimDX;

namespace MHGameWork.TheWizards.Rendering.Particles
{
    /// <summary>
    /// Use a proper way to render meshes instead of layer leaking here!!
    /// </summary>
    public class ParticlesBoxRenderer
    {
        private List<Entity> boxes = new List<Entity>();
        private IMesh boxMesh;
        public ParticlesBoxRenderer()
        {
            boxMesh = UtilityMeshes.CreateBoxWithTexture(null, new Vector3(1, 1, 1));
        }

        private void createBoxesPool(int size)
        {
            while (boxes.Count < size)
            {
                var ent = new Entity(); // AFJRAKLJFALKZEJF FIX THIS

                ent.Mesh = boxMesh;

                boxes.Add(ent);
            }
        }

        public void RenderEffect(ParticleEffect effect)
        {
            RenderEffects(new[]{effect});
        }

        private int currentBox = 0;

        public void RenderEffects(IEnumerable<ParticleEffect> effects)
        {
            currentBox = 0;
            foreach (var e in effects)
            {
                renderSingle(e);
            }
        }

        private void renderSingle(ParticleEffect effect)
        {
            foreach (var p in effect.Particles)
            {
                if (currentBox >= boxes.Count)
                    createBoxesPool(boxes.Count + 10);

                var box = boxes[currentBox];
                box.Visible = true;
                box.WorldMatrix = Matrix.Scaling(p.Size * MathHelper.One) * Matrix.Translation(effect.CalculatePosition(p));
                currentBox++;
            }

            for (int i = currentBox; i < boxes.Count; i++)
            {
                boxes[i].Visible = false;
            }
        }


    }
}