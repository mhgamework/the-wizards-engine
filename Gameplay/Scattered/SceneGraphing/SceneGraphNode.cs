﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using DirectX11;
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

        public Matrix Relative
        {
            get { return relative; }
            set
            {
                var change = relative != value;
                relative = value;
                if (change) onChange();
            }
        }

       

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
                var change = parent != value;
                parent = value;
                if (children.Contains(parent)) throw new InvalidOperationException();
                if (change) onChange(); // Can be extended to check position changes
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
            destroyObservers.ForEach(a => a());
            Parent.children.Remove(this);
            Parent = null;
            children = null;
            AssociatedObject = null;
        }

        public override string ToString()
        {
            return string.Format("ID: {0}, Obj: {1}", ID, AssociatedObject);
        }





        #region Helper methods

        public Vector3 Forward { get { return Absolute.xna().Forward.dx(); } }
        public Vector3 Position
        {
            get { return Absolute.GetTranslation(); }
            set
            {
                var newAbs = Absolute * Matrix.Translation(value - Position);
                Relative = newAbs * Matrix.Invert(parent.Absolute);
            }
        }


        #endregion

        private List<Action> destroyObservers = new List<Action>();
        public void ObserveDestroy(Action func)
        {
            destroyObservers.Add(func);
        }

        private List<Action> changeObservers = new List<Action>();
        private Matrix relative;

        public void ObserveChange(Action func)
        {
            changeObservers.Add(func);
        }

        /// <summary>
        /// Raises the change event
        /// </summary>
        private void onChange()
        {
            children.ForEach(c => c.onChange());
            changeObservers.ForEach(a => a());
        }
    }
}