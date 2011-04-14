using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Common.Network
{
    [Obsolete("This class has been remade")]
    public struct DeltaSnapshotPacket : Common.Networking.INetworkSerializable
    {
        public int Tick;
        //public int EntityCount;
        private ByteWriter dataWriter;
        public ByteReader dataReader;


        public void StartWriting( ByteWriter nBR )
        {
            dataWriter = nBR;
            dataWriter.Seek( 0, System.IO.SeekOrigin.Begin );

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityID"></param>
        /// <param name="p"></param>
        /// <remarks>entityID is a ushort!! dit is om zo maar 2 bytes te moeten gebruiken ipv 4 bytes
        /// voor de entityID. Enkel de entities met id 0 -> ushort.MaxValue kunnen entityupdatepackets
        /// versturen. De statische objecten moeten dus een id hebben van meer dan ushort.MaxValue</remarks>
        public void AddEntityUpdatePacket( ushort entityID, Common.Networking.INetworkSerializable p )
        {
            //if (entityID < 0 || entityID > ushort.MaxValue ) throw new Exception("De entityID is out of range. See remarks of this method for more info");

            byte[] dgram = p.ToNetworkBytes();
            dataWriter.Write( entityID );
            dataWriter.WriteCompressed( dgram.Length );
            dataWriter.Write( dgram );

        }


        //public byte[] ReadEntityUpdatePacket(out int entityID)
        //{
        //    if ( dataReader.BytesLeft == 0 ) return null;

        //    entityID = dataReader.ReadInt32();
        //    return dataReader.ReadBytes( dataReader.ReadInt32() );
        //}

        public int ReadEntityUpdate( out int length )
        {
            length = 0;

            if ( dataReader.BytesLeft == 0 ) return -1;

            ushort entityID;

            entityID = dataReader.ReadUInt16();
            length = dataReader.ReadCompressedInt32();
            return entityID;
        }


        #region INetworkSerializable Members

        public byte[] ToNetworkBytes()
        {
            ByteWriter bw = new ByteWriter();

            bw.Write( Tick );
            //bw.Write( EntityCount );

            //bw.Write( (int)dataWriter.MemStrm.Position );
            bw.Write( dataWriter.ToBytes(), 0, (int)dataWriter.MemStrm.Position );


            return bw.ToBytesAndClose();
        }
        public static DeltaSnapshotPacket FromNetworkBytes( ByteReader br )
        {
            DeltaSnapshotPacket p = new DeltaSnapshotPacket();

            p.Tick = br.ReadInt32();
            //p.EntityCount = br.ReadInt32();

            //p.dataReader = new ByteReader( br.ReadBytes( br.ReadInt32() ) );
            p.dataReader = new ByteReader( br.ReadBytes( (int)br.BytesLeft ) );


            return p;
        }

        #endregion

    }
}
