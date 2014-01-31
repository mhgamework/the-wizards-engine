using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Engine.Raycasting;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Raycasting;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Selecting
{
    /// <summary>
    /// Provides functionality to select items in a 'world'. 
    /// Selectables are provided by WorldSelectableProvider.
    /// </summary>
    public class WorldSelector
    {
        private List<IWorldSelectableProvider> providers = new List<IWorldSelectableProvider>();

        public void UpdateTarget(Ray ray)
        {
            var closest = new RaycastResult();
            var newResult = new RaycastResult();

            IWorldSelectableProvider closestProvider = null;
            Selectable closestSelectable = null;

            foreach (var p in providers)
            {
                foreach (var s in p.GetSelectables())
                {
                    newResult = s.Intersects(ray);
                    if (newResult.IsCloser(closest))
                    {
                        newResult.CopyTo(closest);
                        closestProvider = p;
                        closestSelectable = s;
                    }

                }
            }
            setTargeted(closestProvider, closestSelectable);
        }

        public void Select()
        {
            if (lastTargetedProvider == null) return;
            lastTargetedProvider.Select(lastSelectable);
        }

        public void RenderSelection()
        {
            foreach (var p in providers)
                p.Render();
        }

        private IWorldSelectableProvider lastTargetedProvider = null;
        private Selectable lastSelectable;
        private void setTargeted(IWorldSelectableProvider provider, Selectable selectable)
        {
            if (lastTargetedProvider != null)
            {
                lastTargetedProvider.SetTargeted(null);
            }
            if (provider != null)
                provider.SetTargeted(selectable);
            lastTargetedProvider = provider;
            lastSelectable = selectable;
        }

        private void raycastEntity(Engine.WorldRendering.Entity ent, Ray ray, RaycastResult newResult)
        {
            bool abort = ent.Mesh == null;

            if (abort)
            {
                newResult.Set(null, ent);
                return;
            }

            var transformed = ray.Transform(Matrix.Invert(ent.WorldMatrix));


            //TODO: do course boundingbox check
            var bb = TW.Assets.GetBoundingBox(ent.Mesh);
            if (!transformed.xna().Intersects(bb.xna()).HasValue)
            {
                newResult.Set(null, ent);
                return;
            }



            Vector3 v1, v2, v3;
            var distance = MeshRaycaster.RaycastMesh(ent.Mesh, transformed, out v1, out v2, out v3);


            newResult.Set(distance, ent);
            newResult.V1 = Vector3.TransformCoordinate(v1, ent.WorldMatrix);
            newResult.V2 = Vector3.TransformCoordinate(v2, ent.WorldMatrix);
            newResult.V3 = Vector3.TransformCoordinate(v3, ent.WorldMatrix);

        }

        public void AddProvider(BoundingBoxSelectableProvider provider)
        {
            if (providers.Contains(provider)) return;
            providers.Add(provider);
        }
        public void ClearProviders()
        {
            providers.Clear();
        }
    }
}
