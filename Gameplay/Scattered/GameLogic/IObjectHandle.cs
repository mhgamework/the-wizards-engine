using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Scattered.GameLogic.Objects;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;

namespace MHGameWork.TheWizards.Scattered.Bindings
{
    /// <summary>
    /// Provides a game object access to the underlying engine
    /// </summary>
    public interface IObjectHandle
    {
        ItemType CoalType { get; }
        ItemType FriesType { get; }

        ScatteredPlayer LocalPlayer { get;  }

        EntityNode CreateEntityNode(SceneGraphNode node);
        TextPanelNode CreateTextPanelNode(SceneGraphNode node);
        EntityInteractableNode CreateEntityInteractable(Entity entity, SceneGraphNode createChild, Action onInteract);

        void DestroyNode(SceneGraphNode node);
        void AddBehaviour(SceneGraphNode node, Action update);
        /// <summary>
        /// Adds a behaviour simulator to this object. This behaviour is executed repeatedly, and the return value of the behaviour enumerable
        /// is used as a waiting period until the next frame.
        /// The behaviour is automatically stopped when the object is removed.
        /// </summary>
        /// <param name="islandAddon"></param>
        /// <param name="stepBehaviour"></param>
        void AddBehaviour(SceneGraphNode node, IEnumerable<float> stepBehaviour);
        

    }
}