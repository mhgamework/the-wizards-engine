using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MHGameWork.TheWizards.ServerClient
{
    /// <summary>
    /// This class is used to create serialized Byte arrays in an easy and fast way.
    /// Currently it inherits from IO.BinaryWriter, but this might be removed later on,
    /// since a manual code of the ByteWriter could be faster than BinaryWriter.
    /// </summary>
    /// <remarks>
    /// This class can be optimized by removing the BinaryWriter an dthe MemoryStream, 
    /// and using a plain byte[] for holding the data and writing specific functions to write datatypes.
    /// </remarks>
    public class TWByteWriter : BinaryWriter
    {
        MemoryStream memoryStream;

        protected TWByteWriter( MemoryStream nMemStrm )
            : base( nMemStrm )
        {
            memoryStream = nMemStrm;
        }

        public TWByteWriter()
            : base( new MemoryStream() )
        {
            memoryStream = (MemoryStream)OutStream;
        }


        //Public Sub New(ByVal Bytes As Byte())
        //    Me.New()
        //    Me.Write(Bytes)
        //End Sub
        //Public Sub New(ByVal nBaseStream As IO.Stream)
        //    MyBase.New(nBaseStream)
        //    Me.setMemStrm(Nothing)
        //End Sub

        public byte[] ToBytes()
        {
            return memoryStream.ToArray();
        }

        protected override void Dispose( bool disposing )
        {
            base.Dispose( disposing );
            if ( memoryStream != null )
            {
                memoryStream.Close();
                memoryStream = null;
            }
        }

        public byte[] ToBytesAndClose()
        {
            Byte[] data;
            data = ToBytes();
            Close();

            return data;
        }

        //Public Overloads Sub Write(ByVal nINS As INetworkSerializable)
        //    Me.Write(nINS.ToNetworkBytes)
        //End Sub

        //Public Overloads Sub WriteCompressed(ByVal value As Integer)
        //    Me.Write7BitEncodedInt(value)
        //End Sub
    }
}
