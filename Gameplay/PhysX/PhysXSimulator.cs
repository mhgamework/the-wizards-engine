using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.PhysX;
using SlimDX;

namespace MHGameWork.TheWizards.Simulators
{
    /// <summary>
    /// This simulator is responsible for setting and managing PhysX for Entity's
    /// </summary>
    public class PhysXSimulator : ISimulator
    {
        private ClientPhysicsQuadTreeNode root;
        private MeshPhysicsElementFactory factory;

        private EntityPhysXUpdater entityUpdater;


        public PhysicsDebugRenderer DebugRenderer { get; private set; }

        private PhysXData data;

        public PhysXSimulator()
        {
            data = TW.Data.GetSingleton<PhysXData>();

            root = new ClientPhysicsQuadTreeNode(
               new BoundingBox(new Vector3(-1000, -1000, -1000), new Vector3(1000, 1000, 1000)).xna());
            QuadTree.Split(root, 6);

            data.RootNode = root;

            factory = new MeshPhysicsElementFactory(TW.Physics, root);

            factory.Initialize();

            entityUpdater = new EntityPhysXUpdater(factory, root);

            DebugRenderer = new PhysicsDebugRenderer(TW.Graphics,TW.Physics.Scene);
            DebugRenderer.Initialize();
        }

        public void Simulate()
        {


            entityUpdater.Update();
            factory.Update();


            int length;
            Data.ModelContainer.ObjectChange[] array;
            TW.Data.GetObjectChanges(out array, out length);

            for (int i = 0; i < length; i++)
            {
                var change = array[i];
                if (change.ModelObject is Sphere)
                {
                    ((Sphere)change.ModelObject).ProcessPhysXChanges();
                }
            }

            DebugRenderer.Render();
        }
    }
}
