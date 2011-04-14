using System.IO;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
namespace MHGameWork.TheWizards.Common.Core
{
    public class DataStream : MemoryStream
    {


        private System.IO.BinaryReader mBR;
        public BinaryReader BR
        {
            get { return this.mBR; }
        }
        private void setBR(System.IO.BinaryReader value)
        {
            this.mBR = value;
        }


        private System.IO.BinaryWriter mBW;
        public BinaryWriter BW
        {
            get { return this.mBW; }
        }
        private void setBW(System.IO.BinaryWriter value)
        {
            this.mBW = value;
        }


        public DataStream(byte[] nBuffer, int Index, int Count)
            : base(nBuffer, Index, Count)
        {
            this.CreateBinaryStreams();
        }
        public DataStream(byte[] nBuffer)
            : this(nBuffer, 0, nBuffer.Length)
        {

        }
        public DataStream(int Capacity)
            : base(Capacity)
        {
            this.CreateBinaryStreams();
        }
        public DataStream()
            : base()
        {
            this.CreateBinaryStreams();
        }
        protected void CreateBinaryStreams()
        {
            this.setBR(new System.IO.BinaryReader(this));
            this.setBW(new System.IO.BinaryWriter(this));
        }

        public long BytesLeft
        {
            get { return this.Length - this.Position; }
        }

        public void WriteToDataStream(DataStream nDataStream, int nLength)
        {
            //Dim Buffer As Byte() = New Byte(nLength) {}
            //nDataStream.Read(Buffer, 0, nLength)
            nDataStream.Write(this.BR.ReadBytes(nLength), 0, nLength);

        }
    }
}