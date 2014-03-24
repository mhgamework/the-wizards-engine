using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.SceneGraphing
{
    /// <summary>
    /// Represents a 3D positioned thing in the world, organized into a tree structure.
    /// 
    /// The SceneGraph consolidates 3 things in the program conceptual structure:
    ///     - The fact that there is a hierarchy within objects in the game state
    ///     - The fact that there is a position concept.
    ///     - The fact that position can be relative to other things with a position concept
    /// 
    /// TODO: Add RelativePosition and RelativeOrientation etc.
    /// 
    /// IDEA: make the parent reference to itself, so that there is no 'null' exception in the parent tree? 
    ///     (remove the corner case checks for the parent from user code)
    /// </summary>
    public class SceneGraphNode
    {
        /// <summary>
        /// This is the object that is the owner of this node.
        /// </summary>
        public object AssociatedObject { get; set; }

        public int ID { get; private set; }
        private static int nextID = 0;

        public SceneGraphNode()
        {
            Relative = Matrix.Identity;
            Absolute = Matrix.Identity;
            ID = nextID++;
        }

        public Matrix Relative { get; set; }
        public Matrix Absolute
        {
            get
            {
                if (Parent == null) return Relative;
                return Relative * Parent.Absolute;
            }
            set
            {
                if (Parent == null)
                {
                    Relative = value;
                    return;
                }
                Relative = value * Matrix.Invert(Parent.Absolute);
            }
        }

        public SceneGraphNode Parent
        {
            get { return parent; }
            private set
            {
                parent = value;
                if (children.Contains(parent)) throw new InvalidOperationException();
            }
        }

        private List<SceneGraphNode> children = new List<SceneGraphNode>();
        private SceneGraphNode parent;

        public IEnumerable<SceneGraphNode> Children
        {
            get { return children; }
        }

        public SceneGraphNode CreateChild()
        {
            var ret = new SceneGraphNode();
            ret.Parent = this;
            children.Add(ret);

            return ret;
        }

        //public void ChangeParent(SceneGraphNode newParent)
        //{
        //    if (Parent != null)
        //    {
        //        Parent.children.Remove(this);
        //    }

        //    if (children.Contains(newParent)) throw new InvalidOperationException();

        //    Parent = newParent;
        //    newParent.children.Add(this);
        //}

        public void Dispose()
        {
            Parent.children.Remove(this);
            Parent = null;
            children = null;
            AssociatedObject = null;
        }

        public override string ToString()
        {
            return string.Format("ID: {0}, Obj: {1}", ID, AssociatedObject);
        }
    }
}