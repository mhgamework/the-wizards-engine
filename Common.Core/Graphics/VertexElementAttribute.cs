using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Graphics
{
    [AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
    public class VertexElementAttribute : Attribute
    {
        private int stream;

        public int Stream
        {
            get { return stream; }
            set { stream = value; }
        }

        private VertexElementMethod method;

        public VertexElementMethod Method
        {
            get { return method; }
            set { method = value; }
        }

        private VertexElementFormat format;

        public VertexElementFormat Format
        {
            get { return format; }
            set { format = value; }
        }

        private VertexElementUsage usage;

        public VertexElementUsage Usage
        {
            get { return usage; }
            private set { usage = value; }
        }

        private int offset;

        internal int Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        public VertexElementAttribute( VertexElementUsage usage )
        {
            Usage = usage;
            Format = VertexElementFormat.Unused;
        }

        public VertexElementAttribute( VertexElementUsage usage, VertexElementFormat format )
        {
            Usage = usage;
            Format = format;
        }
    }
}
