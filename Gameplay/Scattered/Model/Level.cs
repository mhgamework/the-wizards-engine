using System;
using System.Collections.Generic;
using System.IO;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Scattered.Core;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using MHGameWork.TheWizards.Scattered.Simulation;
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
    public class Level
    {
        private List<Island> islands = new List<Island>();
        private List<Traveller> travellers = new List<Traveller>();

        public SceneGraphNode Node { get; private set; }

        public Level()
        {
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
        public Traveller CreateNewTraveller(Island start, Func<Island> destinationAction)
        {
            var ret = new Traveller()
                          {
                              BridgePosition = new BridgePosition(start, start, 0),
                              PlannedPath = new[] { start },
                              DetermineDestinationAction = destinationAction
                          };
            travellers.Add(ret);

            return ret;

        }

        public IEnumerable<Island> Islands { get { return islands; } }

        public IEnumerable<Traveller> Travellers { get { return travellers; } }


        public void RemoveTraveller(Traveller traveller)
        {
            travellers.Remove(traveller);
        }

        public void RemoveIsland(Island island)
        {
            islands.Remove(island);
        }


        public void ClearAll()
        {
            islands.ToArray().ForEach(RemoveIsland);
            travellers.ToArray().ForEach(RemoveTraveller);
        }


        #region Item types

        private void createItemTypes()
        {
            UnitTier1Type = new ItemType() { Name = "Air Unit Tier 1" };
            AirCrystalType = new ItemType() { Name = "Air crystal" };
            AirEnergyType = new ItemType() { Name = "Air energy" };
            ScrapType = new ItemType() { Name = "Scrap" };

            WorkshopCartType = new TravellerType() { IsEnemy = false, Name = "Workshop Cart" };
            DeliveryCartType = new TravellerType() { IsEnemy = false, Name = "Delivery Cart" };
        }

        public ItemType UnitTier1Type { get; private set; }
        public ItemType AirCrystalType { get; private set; }
        public ItemType AirEnergyType { get; private set; }
        public ItemType ScrapType { get; private set; }

        #endregion

        #region Cart Types

        public TravellerType WorkshopCartType { get; private set; }
        public TravellerType DeliveryCartType { get; private set; }

        public ScatteredPlayer LocalPlayer { get; set; }

        #endregion


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
            var ret = new EntityNode(this, node);
            EntityNodes.Add(ret);
            return ret;
        }


        public TextPanelNode CreateTextPanelNode(SceneGraphNode node)
        {
            var ret = new TextPanelNode(this, node);
            TextPanelNodes.Add(ret);
            return ret;
        }

        public EntityInteractableNode CreateEntityInteractable(Entity entity, SceneGraphNode createChild, Action onInteract)
        {
            var ret = new EntityInteractableNode(entity, createChild, onInteract);
            InteractableNodes.Add(ret);
            return ret;
        }

        public void DestroyNode(SceneGraphNode node)
        {
            if (node.Children == null) return; // already disposed
            foreach (var c in node.Children.ToArray())
            {
                DestroyNode(c);
            }


            EntityNodes.Where(k => k.Node == node).ToArray()
                .ForEach(e =>
                    {
                        e.Dispose();
                        EntityNodes.Remove(e);
                    });

            TextPanelNodes.Where(k => k.Node == node).ToArray()
              .ForEach(e =>
              {
                  e.Dispose();
                  TextPanelNodes.Remove(e);
              });


            InteractableNodes.Where(k => k.Node == node).ToArray()
               .ForEach(e =>
               {
                   e.Dispose();
                   InteractableNodes.Remove(e);
               });

            node.Dispose();
        }

    }

}