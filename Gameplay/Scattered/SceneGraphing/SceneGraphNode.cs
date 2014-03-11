using System.Collections.Generic;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.SceneGraphing
{
    /// <summary>
    /// Represents a 3D positioned thing in the world, organized into a tree structure.
    /// 
    /// IDEA: make the parent reference to itself, so that there is no 'null' exception in the parent tree? 
    ///     (remove the corner case checks for the parent from user code)
    /// </summary>
    public class SceneGraphNode
    {
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

        public SceneGraphNode Parent { get; private set; }

        private List<SceneGraphNode> children = new List<SceneGraphNode>();
        public IEnumerable<SceneGraphNode> Children
        {
            get { return children; }
        }

        public SceneGraphNode CreateChild()
        {
            var ret = new SceneGraphNode();
            ret.Parent = this;
            ret.children.Add(ret);

            return ret;
        }

        public void ChangeParent(SceneGraphNode newParent)
        {
            if (Parent != null)
            {
                Parent.children.Remove(this);
            }

            Parent = newParent;
            newParent.children.Add(this);
        }
    }
}