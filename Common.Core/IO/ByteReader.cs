using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient
{
    public class TWByteReader : BinaryReader
    {
        public TWByteReader( byte[] Bytes )
            : base( new MemoryStream( Bytes ) )
        {
        }
        public TWByteReader( System.IO.Stream nBaseStream )
            : base( nBaseStream )
        {
        }


        public Vector3 ReadVector3()
        {
            return new Vector3( this.ReadSingle(), this.ReadSingle(), this.ReadSingle() );
        }
        public Vector2 ReadVector2()
        {
            return new Vector2( this.ReadSingle(), this.ReadSingle() );
        }
        public Vector4 ReadVector4()
        {
            return new Vector4( this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle() );
        }
        public Quaternion ReadQuaternion()
        {
            return new Quaternion( this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle() );
        }
        public Matrix ReadMatrix()
        {
            Matrix M = default( Matrix );
            M.M11 = this.ReadSingle();
            M.M12 = this.ReadSingle();
            M.M13 = this.ReadSingle();
            M.M14 = this.ReadSingle();
            M.M21 = this.ReadSingle();
            M.M22 = this.ReadSingle();
            M.M23 = this.ReadSingle();
            M.M24 = this.ReadSingle();
            M.M31 = this.ReadSingle();
            M.M32 = this.ReadSingle();
            M.M33 = this.ReadSingle();
            M.M34 = this.ReadSingle();
            M.M41 = this.ReadSingle();
            M.M42 = this.ReadSingle();
            M.M43 = this.ReadSingle();
            M.M44 = this.ReadSingle();

            return M;
        }




    }
}
