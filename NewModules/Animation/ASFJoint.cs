using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Animation
{
    public class ASFJoint
    {
        public int id;
        public string name;
        public Vector3 direction;
        public float length;
        public Vector3 axis;
        public string axisOrder;

        public string[] dof;

        public ASFJoint parent;
        public List<ASFJoint> children = new List<ASFJoint>();

        public override string ToString()
        {
            return "ASFJoint - name:" + name.ToString();
        }
    }
}
