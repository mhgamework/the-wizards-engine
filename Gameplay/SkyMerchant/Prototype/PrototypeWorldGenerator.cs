using System;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant._Windsor;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype
{
    public class PrototypeWorldGenerator
    {
        #region Injection

        [NonOptional]
        public ObjectsFactory Factory { get; set; }
        [NonOptional]
        public Random Random { get; set; }
        #endregion


        public void GenerateWorld(float size)
        {
            var worldMin = -new Vector3(size);
            var worldMax = new Vector3(size);
            worldMin.Y = 0;
            worldMax.Y = 20;

            var density = 1/(20*20f);

            for (int i = 0; i < size * size *density; i++)
            {
                var n = Factory.CreateIsland();
                n.Seed = Random.Next(3);
                n.Physical.SetPosition(nextVector3(worldMin, worldMax));
                n.TargetHeight = n.Physical.GetPosition().Y;
            }
        }

        private Vector3 nextVector3(Vector3 min, Vector3 max)
        {
            return new Vector3(nextFloat(min.X, max.X), nextFloat(min.Y, max.Y), nextFloat(min.Z, max.Z));
        }
        private float nextFloat(float min, float max)
        {
            return (float)Random.NextDouble() * (max - min) + min;
        }
    }

    public class ObjectsFactory
    {
        private readonly ITypedFactory factory;

        public ObjectsFactory(ITypedFactory factory)
        {
            this.factory = factory;
        }

        public IslandPart CreateIsland()
        {
            var ret = factory.CreateIsland();
            ret.Physical = factory.CreatePhysical();
            ret.Physics = factory.CreatePhysics();
            return ret;

        }
    }

    public interface ITypedFactory
    {
        IslandPart CreateIsland();
        BasicPhysicsPart CreatePhysics();
        Physical CreatePhysical();

    }
}