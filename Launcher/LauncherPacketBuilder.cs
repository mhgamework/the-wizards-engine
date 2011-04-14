using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Launcher
{
    /// <summary>
    /// CreateS stands for server packets (sent from server)
    /// CreateC stands for client packets
    /// ParseS  stands for packets received on client from server
    /// ParseC
    /// </summary>
    public class LauncherPacketParser
    {
        public byte[] CreateCRequestFileList()
        {
            return new[] { (byte)LauncherClientPacketTypes.RequestFileList };
        }

        public byte[] CreateCRequestFile(string relativePath)
        {
            var mem = new MemoryStream();
            var bw = new BinaryWriter(mem);
            bw.Write((byte)LauncherClientPacketTypes.RequestFile);
            bw.Write(relativePath);
            return mem.ToArray();
        }
        public string ParseCRequestFile(byte[] dgram)
        {
            var br = new BinaryReader(new MemoryStream(dgram));

            br.ReadByte();

            return br.ReadString();
        }

        public byte[] CreateSPacketList(HashedFileList list)
        {

            XmlSerializer serializer = new XmlSerializer(typeof(HashedFileList));

            using (var fs = new MemoryStream())
            {
                fs.WriteByte((byte)LauncherServerPacketTypes.FileList);
                serializer.Serialize(fs, list);
                return fs.ToArray();
            }
        }
        public HashedFileList ParseSFileList(byte[] dgram)
        {

            XmlSerializer serializer = new XmlSerializer(typeof(HashedFileList));

            using (var fs = new MemoryStream(dgram))
            {
                fs.ReadByte();
                return serializer.Deserialize(fs) as HashedFileList;
            }

        }

        public byte[] CreateSFilePart(byte[] filepart)
        {
            var ret = new byte[filepart.Length + 1];
            ret[0] = (byte)LauncherServerPacketTypes.FilePart;
            filepart.CopyTo(ret, 1);

            return ret;
        }
        public byte[] ParseSFilePart(byte[] dgram)
        {
            var ret = new byte[dgram.Length-1];
            Array.Copy(dgram, 1, ret, 0, ret.Length);
            return ret;
        }

        public byte[] CreateSFileComplete()
        {
            return new[] { (byte)LauncherServerPacketTypes.FileComplete };
        }

        public LauncherServerPacketTypes ParseServerPacketType(byte[] dgram)
        {
            return (LauncherServerPacketTypes)dgram[0];
        }
        public LauncherClientPacketTypes ParseClientPacketType(byte[] dgram)
        {
            return (LauncherClientPacketTypes)dgram[0];
        }
    }
}
