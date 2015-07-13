using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Client
{
    /// <summary>
    /// This interface represents an object that has physics and interacts with other IClientPhysics objects. This interface is used by the ClientPhysicsQuadTreeNode. These objects have a dynamic/active status, that can change during runtime
    /// </summary>
    public interface IClientPhysicsObject
    {

        void EnablePhysics();
        void DisablePhysics();

        ClientPhysicsQuadTreeNode Node { get; set; }
        //bool IsDynamic { get;}

        /// <summary>
        /// Returns whether this object is entirely in the node, partially, or not at all
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        Microsoft.Xna.Framework.ContainmentType ContainedInNode(ClientPhysicsQuadTreeNode _node);
    }
}
