using System;
using System.Collections.Generic;
using System.IO;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering.Lod;
using MHGameWork.TheWizards.Scattered.Bindings;
using MHGameWork.TheWizards.Scattered.GameLogic.Objects;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using MHGameWork.TheWizards.Scattered.Simulation;
using MHGameWork.TheWizards.Simulation.Spatial;
using SlimDX;
using System.Linq;
using Castle.Core.Internal;
using MHGameWork.TheWizards.Scattered._Engine;

namespace MHGameWork.TheWizards.Scattered.Model
{
    /// <summary>
    /// Responsible for construction, lifetime and destruction of objects in the model.
    /// 
    /// TODO: split into gamestate + constructors?
    /// </summary>
    public class Level : IObjectHandle
    {
        private readonly OptimizedWorldOctree<IRenderable> worldOctree;
        private List<Island> islands = new List<Island>();

        public SceneGraphNode Node { get; private set; }

        public Level(OptimizedWorldOctree<IRenderable> worldOctree )
        {
            this.worldOctree = worldOctree;
            createItemTypes();
            Node = new SceneGraphNode();
            Node.AssociatedObject = this;
            LocalPlayer = new ScatteredPlayer(this, Node.CreateChild());
        }

        public Island CreateNewIsland(Vector3 position)
        {
            var ret = new Island(this, Node.CreateChild()) { Position = position };
            ret.Node.AssociatedObject = ret; //TODO: fix this dependency in a better way?
            islands.Add(ret);

            return ret;
        }

        public IEnumerable<Island> Islands { get { return islands; } }

        public void RemoveIsland(Island island)
        {
            islands.Remove(island);
        }


        public void ClearAll()
        {
            islands.ToArray().ForEach(RemoveIsland);
        }


        #region Item types

        private void createItemTypes()
        {
            UnitTier1Type = new ItemType() { Name = "Air Unit Tier 1" };
            AirCrystalType = new ItemType() { Name = "Air crystal" };
            AirEnergyType = new ItemType() { Name = "Air energy" };
            ScrapType = new ItemType() { Name = "Scrap" };
            CoalType = new ItemType() { Name = "Coal", TexturePath = "Scattered\\Models\\Items\\coal.jpg" };
            FriesType = new ItemType() { Name = "Fries", TexturePath = "Scattered\\Models\\Items\\fries.jpg" };

        }

        public ItemType UnitTier1Type { get; private set; }
        public ItemType AirCrystalType { get; private set; }
        public ItemType AirEnergyType { get; private set; }
        public ItemType ScrapType { get; private set; }
        public ItemType CoalType { get; private set; }
        public ItemType FriesType { get; private set; }

        #endregion

        public ScatteredPlayer LocalPlayer { get; set; }


        public List<EntityNode> EntityNodes = new List<EntityNode>();
        public List<TextPanelNode> TextPanelNodes = new List<TextPanelNode>();
        public List<EntityInteractableNode> InteractableNodes = new List<EntityInteractableNode>();
        /// <summary>
        /// TODO: fix for better DI
        /// </summary>
        /// <param name="sceneGraphNode"></param>
        /// <returns></returns>
        public EntityNode CreateEntityNode(SceneGraphNode node)
        {
            var ret = new EntityNode(this, node,worldOctree);
            EntityNodes.Add(ret);

            worldOctree.AddWorldObject(ret);
            node.ObserveDestroy(() =>
                {
                    EntityNodes.Remove(ret);
                    worldOctree.RemoveWorldObject(ret);
                    ret.Dispose();
                });
            return ret;
        }


        public TextPanelNode CreateTextPanelNode(SceneGraphNode node)
        {
            var ret = new TextPanelNode(this, node);
            TextPanelNodes.Add(ret);
            node.ObserveDestroy(() =>
                {
                    TextPanelNodes.Remove(ret);
                    ret.Dispose();
                });
            return ret;
        }

        public EntityInteractableNode CreateEntityInteractable(Entity entity, SceneGraphNode createChild, Action onInteract)
        {
            var ret = new EntityInteractableNode(entity, createChild, onInteract);
            InteractableNodes.Add(ret);
            createChild.ObserveDestroy(() =>
                {
                    InteractableNodes.Remove(ret);
                    ret.Dispose();
                });
            return ret;
        }

        public void DestroyNode(SceneGraphNode node)
        {
            if (node.Children == null) return; // already disposed
            foreach (var c in node.Children.ToArray())
            {
                DestroyNode(c);
            }

            node.Dispose();
        }

        #region SpatialService

        public IEnumerable<T> FindInRange<T>(SceneGraphNode node, int range, Func<T, bool> wherePredicate)
            where T : class, IHasNode
        {
            if (typeof (IIslandAddon).IsAssignableFrom(typeof (T)))
            {
                // Search the addons
                return Islands.SelectMany(i => i.Addons).OfType<T>()
                              .Where(r => Vector3.Distance(node.Position, r.Node.Position) < range);
            }
            if (typeof (ScatteredPlayer).IsAssignableFrom(typeof (T)))
            {
                // Search players (eg player)
                if (Vector3.Distance(node.Position, LocalPlayer.Position) < range) return new[] {LocalPlayer as T};
                return new T[0];

            }
            throw new NotImplementedException();
        }

        public T FindClosest<T>(SceneGraphNode node, int range, Func<T, bool> wherePredicate)
            where T : class, IHasNode
        {
            return FindInRange<T>(node, range, wherePredicate).FirstOrDefault();
        }

        #endregion



        #region ClockedBehaviourService

        private List<ClockedBehaviourSimulator> clockedBehaviourSimulators = new List<ClockedBehaviourSimulator>();


        public void AddBehaviour(SceneGraphNode node, Action update)
        {
            AddBehaviour(node,singleUpdate(update));
        }

        private IEnumerable<float> singleUpdate(Action update)
        {
            update();
            yield return 0;
        }

        /// <summary>
        /// Adds a behaviour simulator to this object. This behaviour is executed repeatedly, and the return value of the behaviour enumerable
        /// is used as a waiting period until the next frame.
        /// The behaviour is automatically stopped when the object is removed.
        /// </summary>
        /// <param name="islandAddon"></param>
        /// <param name="stepBehaviour"></param>
        public void AddBehaviour(SceneGraphNode node, IEnumerable<float> stepBehaviour)
        {
            var sim = new ClockedBehaviourSimulator(stepBehaviour);
            clockedBehaviourSimulators.Add(sim);
            node.ObserveDestroy(() => clockedBehaviourSimulators.Remove(sim));

        }

        public void SimulateBehaviours()
        {
            clockedBehaviourSimulators.ForEach(c => c.Update());
        }

        #endregion

    }

}