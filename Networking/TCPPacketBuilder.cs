using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Common.Networking;

namespace MHGameWork.TheWizards.Networking
{
    public class TCPPacketBuilder
    {

        public TCPPacketBuilder()
        {
            //ByVal nHeaderLength As Integer)
            this.SetState(PacketState.Empty);
            this.setHeaderLength(5);


        }

        private PacketState mState;
        public PacketState State
        {
            get { return this.mState; }
        }
        protected void SetState(PacketState nState)
        {
            this.mState = nState;
        }
        public enum PacketState
        {
            Empty = 0,
            ReadingHeader = 1,
            ReadingContent = 2,
            Complete = 3
        }


        private int mHeaderLength;
        public int HeaderLength
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return this.mHeaderLength; }
        }
        private void setHeaderLength(int value)
        {
            this.mHeaderLength = value;
        }


        private int mContentLength;
        public virtual int ContentLength
        {
            get { return this.mContentLength; }
        }
        protected virtual void setContentLength(int value)
        {
            this.mContentLength = value;
        }


        private DataStream mHeader;
        public DataStream Header
        {
            get { return this.mHeader; }
        }
        private void setHeader(DataStream value)
        {
            this.mHeader = value;
        }


        private DataStream mContent;
        public DataStream Content
        {
            get { return this.mContent; }
        }
        private void setContent(DataStream value)
        {
            this.mContent = value;
        }

        public void AppendBytes(DataStream Buffer)
        {
            if (this.State == PacketState.Complete)
            {
                throw new Exception("Cannot append more bytes because the packet is allready complete.");
            }
            var exit = false;
            while (!exit && Buffer.Position < Buffer.Length)
            {
                switch (this.State)
                {
                    case PacketState.Empty:
                        this.SetState(PacketState.ReadingHeader);
                        this.setHeader(new DataStream(this.HeaderLength));

                        break;
                    case PacketState.ReadingHeader:
                        if (Buffer.BytesLeft + this.Header.Length < this.HeaderLength)
                        {
                            Buffer.WriteToDataStream(this.Header, (int)Buffer.BytesLeft);
                        }
                        else
                        {
                            Buffer.WriteToDataStream(this.Header, HeaderLength - (int)Header.Position);
                            this.ProcessHeader();

                            this.SetState(PacketState.ReadingContent);
                            this.setContent(new DataStream((int)this.ContentLength));
                        }


                        break;
                    case PacketState.ReadingContent:
                        if (Buffer.BytesLeft + this.Content.Length < this.ContentLength)
                        {
                            Buffer.WriteToDataStream(Content, (int)Buffer.BytesLeft);
                        }
                        else
                        {
                            Buffer.WriteToDataStream(Content, ContentLength - (int)Content.Position);
                            this.ProcessContent();

                            this.SetState(PacketState.Complete);
                        }


                        break;
                    case PacketState.Complete:
                        exit = true; //Was: Exit Do
                        break;



                }

            }
        }

        protected virtual void ProcessHeader()
        {
            this.Header.Seek(0, System.IO.SeekOrigin.Begin);
            this.setContentLength(this.Header.BR.ReadInt32());
        }
        protected virtual void ProcessContent()
        {
            //Me.setData(Me.Content.BR.ReadBytes(CInt(Me.Content.Length)))
            this.Content.Seek(0, System.IO.SeekOrigin.Begin);
        }




        //Protected Overrides Sub BuildHeader()
        //    MyBase.BuildHeader()
        //    Me.setContentLength(Me.Data.Length)
        //    Me.Header.BW.Write(CInt(Me.ContentLength))
        //End Sub

        //Protected Overrides Sub BuildContent()
        //    MyBase.BuildContent()
        //    Me.Content.BW.Write(Me.Data)
        //End Sub



        //Public Sub BuildPacket()
        //    Me.BuildHeader()
        //    Me.BuildContent()
        //    Me.SetState(PacketState.Complete)
        //End Sub
        //Protected Overridable Sub BuildHeader()
        //    Me.setHeader(New DataStream(Me.HeaderLength))

        //End Sub
        //Protected Overridable Sub BuildContent()
        //    Me.setContent(New DataStream(CInt(Me.ContentLength)))
        //End Sub


        //Public Function ToBytes() As Byte()
        //    Dim Bs As Byte() = New Byte(CInt(Me.Header.Length + Me.Content.Length - 1)) {}
        //    Me.Header.ToArray.CopyTo(Bs, 0)
        //    Me.Content.ToArray.CopyTo(Bs, Me.Header.Length)
        //    Return Bs
        //End Function


        //Public Overrides Function ToString() As String
        //    Return Me.GetType.Name

        //End Function








        //Private mData As Byte()
        //Public ReadOnly Property Data() As Byte()
        //    Get
        //        Return Me.mData
        //    End Get
        //End Property
        //Protected Sub setData(ByVal value As Byte())
        //    Me.mData = value
        //    Me.setContentLength(Me.Data.Length)
        //End Sub

        [Flags]
        public enum TCPPacketFlags : byte
        {
            None = 0,
            GZipCompressed = 1
        }

        public byte[] BuildPacket(byte[] dgram, TCPPacketFlags Flags)
        {
            DataStream OutDS = null;
            DataStream InputDS = null;
            byte[] ret = null;
            try
            {
                OutDS = new DataStream();
                InputDS = new DataStream();

                //build input dgram
                if ((Flags & TCPPacketFlags.GZipCompressed) > 0)
                {
                    InputDS.BW.Write((int)dgram.Length);

                    System.IO.Compression.GZipStream GZip = new System.IO.Compression.GZipStream(InputDS, System.IO.Compression.CompressionMode.Compress, true);
                    GZip.Write(dgram, 0, dgram.Length);
                    GZip.Close();
                    //versneltruck: kijk of de gecompressed versie wel echt kleiner is
                    if (InputDS.Length > dgram.Length)
                    {
                        //groter ==> geen compressie gebruiken

                        return this.BuildPacket(dgram, Flags ^ TCPPacketFlags.GZipCompressed);

                    }
                }
                else
                {
                    InputDS.Write(dgram, 0, dgram.Length);
                }

                //header:
                OutDS.BW.Write((int)InputDS.Length);
                OutDS.BW.Write((byte)Flags);

                //content:
                InputDS.WriteTo(OutDS);

                ret = OutDS.ToArray();

                InputDS.Close();

                OutDS.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (InputDS != null)
                {
                    InputDS.Close();
                    InputDS = null;
                }
                if (OutDS != null)
                {
                    OutDS.Close();
                    OutDS = null;
                }
            }



            return ret;
        }

        public TCPPacketFlags GetPacketFlags()
        {
            TCPPacketFlags F = default(TCPPacketFlags);
            this.Header.Seek(4, System.IO.SeekOrigin.Begin);
            F = (TCPPacketFlags)this.Header.BR.ReadByte();

            return F;
        }

        public byte[] GetPacketDgram()
        {
            TCPPacketFlags Flags = this.GetPacketFlags();
            byte[] Dgram = null;

            try
            {



                if ((Flags & TCPPacketFlags.GZipCompressed) > 0)
                {

                    this.Content.Seek(0, System.IO.SeekOrigin.Begin);
                    int OriginalLength = this.Content.BR.ReadInt32();

                    System.IO.Compression.GZipStream GZip = new System.IO.Compression.GZipStream(this.Content, System.IO.Compression.CompressionMode.Decompress);
                    //Dim DecompressedBuffer As Byte() = New Byte(OriginalLength - 1) {}
                    //Dim totalCount As Integer = TCPPacketBuilder.ReadAllBytesFromStream(GZip, DecompressedBuffer)
                    Dgram = new byte[OriginalLength];
                    GZip.Read(Dgram, 0, OriginalLength);

                    GZip.Close();
                }
                else
                {
                    //Dgram = New Byte(totalCount - 1) {}
                    //Array.Copy(DecompressedBuffer, 0, Dgram, 0, totalCount)
                    Dgram = this.Content.ToArray();
                }












                this.Header.Close();
                this.Content.Close();
                this.SetState(PacketState.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Dgram;
        }


        private static int ReadAllBytesFromStream(System.IO.Stream stream, byte[] buffer)
        {
            // Use this method is used to read all bytes from a stream.
            int offset = 0;
            int totalCount = 0;
            while (true)
            {
                int bytesRead = stream.Read(buffer, offset, 100);
                if (bytesRead == 0)
                {
                    break; // TODO: might not be correct. Was : Exit While
                }
                offset += bytesRead;
                totalCount += bytesRead;
            }
            return totalCount;
        }
        //ReadAllBytesFromStream




    }

}
