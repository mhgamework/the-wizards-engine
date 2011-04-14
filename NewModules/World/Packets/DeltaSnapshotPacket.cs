using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MHGameWork.TheWizards.Common;
using MHGameWork.TheWizards.Networking;

namespace MHGameWork.TheWizards.World.Packets
{
    public class DeltaSnapshotParser
    {
        public int Tick;
        //public int EntityCount;
        private MemoryStream writeMemory;
        private MemoryStream readMemory;
        private ByteWriter dataWriter;
        public ByteReader dataReader;

        public DeltaSnapshotParser()
        {
            writeMemory = new MemoryStream();
            dataWriter = new ByteWriter(writeMemory);
        }

        public void StartWrite(int tick)
        {
            writeMemory.SetLength(0);
            dataWriter.Seek(0, System.IO.SeekOrigin.Begin);
            dataWriter.Write(tick);
            
            //bw.Write( EntityCount );
            //bw.Write( (int)dataWriter.MemStrm.Position );
            
        }
        public byte[] EndWrite()
        {
            return writeMemory.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityID"></param>
        /// <param name="p"></param>
        /// <remarks>entityID is a ushort!! dit is om zo maar 2 bytes te moeten gebruiken ipv 4 bytes
        /// voor de entityID. Enkel de entities met id 0 -> ushort.MaxValue kunnen entityupdatepackets
        /// versturen. De statische objecten moeten dus een id hebben van meer dan ushort.MaxValue</remarks>
        public void WriteEntityUpdatePacket(ushort entityID, UpdateEntityPacket p)
        {
            //if (entityID < 0 || entityID > ushort.MaxValue ) throw new Exception("De entityID is out of range. See remarks of this method for more info");

            byte[] dgram = p.ToNetworkBytes();
            dataWriter.Write(entityID);
            //dataWriter.WriteCompressed(dgram.Length);
            dataWriter.Write(dgram);

        }


        //public byte[] ReadEntityUpdatePacket(out int entityID)
        //{
        //    if ( dataReader.BytesLeft == 0 ) return null;

        //    entityID = dataReader.ReadInt32();
        //    return dataReader.ReadBytes( dataReader.ReadInt32() );
        //}

        public void StartRead(byte[] data, out int tick)
        {
            //TODO: Warning: might be NOT GOOD
            readMemory = new MemoryStream(data);
            dataReader = new ByteReader(readMemory);

            tick = dataReader.ReadInt32();


        }


        public UpdateEntityPacket ReadEntityUpdate(out ushort entityID)
        {
            entityID = 0;
            
            if (dataReader.BytesLeft == 0) return UpdateEntityPacket.Empty;

            entityID = dataReader.ReadUInt16();
            //length = dataReader.ReadCompressedInt32();
            return UpdateEntityPacket.FromNetworkBytes(dataReader);
        }


    }
}
