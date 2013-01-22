using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTS.Commands;
using SlimDX;

namespace MHGameWork.TheWizards.RTS
{
    [ModelObjectChanged]
    public class Factory : EngineModelObject
    {
        public Factory()
        {
            BuildInterval = 3;
        }

        public Vector3 Position { get; set; }
        public ResourceType InputType { get; set; }
        public ResourceType OutputType { get; set; }

        public float BuildInterval { get; set; }


        public BoundingBox GetInputArea()
        {
            var offset = new Vector3(0, 0, -2.1f);
            offset += Position;
            return new BoundingBox(new Vector3(-1,0,-1)+offset,new Vector3(1,1,1) + offset );
        }
        public BoundingBox GetOutputArea()
        {
            var offset = new Vector3(0, 0, 2.1f);
            offset += Position;
            return new BoundingBox(new Vector3(-1, 0, -1) + offset, new Vector3(1, 1, 1) + offset);
        }
    }
}
