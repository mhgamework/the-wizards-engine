using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.ModelContainer;
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


        private PhysicsDebugRenderer debugRenderer;

        public PhysXSimulator()
        {
            root = new ClientPhysicsQuadTreeNode(
               new BoundingBox(new Vector3(-1000, -1000, -1000), new Vector3(1000, 1000, 1000)).xna());
            factory = new MeshPhysicsElementFactory(TW.PhysX, root);

            factory.Initialize();

            entityUpdater = new EntityPhysXUpdater(factory, root);

            debugRenderer = new PhysicsDebugRenderer(TW.Game,TW.Scene);
            debugRenderer.Initialize();
        }

        public void Simulate()
        {


            entityUpdater.Update();
            factory.Update();


            int length;
            ModelContainer.ModelContainer.ObjectChange[] array;
            TW.Model.GetObjectChanges(out array, out length);

            for (int i = 0; i < length; i++)
            {
                var change = array[i];
                if (change.ModelObject is Sphere)
                {
                    ((Sphere)change.ModelObject).ProcessPhysXChanges();
                }
            }

            debugRenderer.Render();
        }
    }
}
