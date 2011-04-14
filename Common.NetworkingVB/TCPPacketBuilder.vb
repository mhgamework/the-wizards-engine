<Obsolete()>
Public Class TCPPacketBuilder

    Public Sub New() 'ByVal nHeaderLength As Integer)
        Me.SetState(PacketState.Empty)
        Me.setHeaderLength(5)
        Console.WriteLine("WARNING: Using old TCPPacketBuilder!!!!")
        'Throw New Exception("This is an old version of this class, and is disabled to prevent wierd things happening")

    End Sub

    Private mState As PacketState
    Public ReadOnly Property State() As PacketState
        Get
            Return Me.mState
        End Get
    End Property
    Protected Sub SetState(ByVal nState As PacketState)
        Me.mState = nState
    End Sub
    Public Enum PacketState
        Empty = 0
        ReadingHeader = 1
        ReadingContent = 2
        Complete = 3
    End Enum


    Private mHeaderLength As Integer
    Public ReadOnly Property HeaderLength() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mHeaderLength
        End Get
    End Property
    Private Sub setHeaderLength(ByVal value As Integer)
        Me.mHeaderLength = value
    End Sub


    Private mContentLength As Integer
    Public Overridable ReadOnly Property ContentLength() As Integer
        Get
            Return Me.mContentLength
        End Get
    End Property
    Protected Overridable Sub setContentLength(ByVal value As Integer)
        Me.mContentLength = value
    End Sub


    Private mHeader As DataStream
    Public ReadOnly Property Header() As DataStream
        Get
            Return Me.mHeader
        End Get
    End Property
    Private Sub setHeader(ByVal value As DataStream)
        Me.mHeader = value
    End Sub


    Private mContent As DataStream
    Public ReadOnly Property Content() As DataStream
        Get
            Return Me.mContent
        End Get
    End Property
    Private Sub setContent(ByVal value As DataStream)
        Me.mContent = value
    End Sub

    Public Sub AppendBytes(ByVal Buffer As DataStream)
        If Me.State = PacketState.Complete Then
            Throw New Exception("Cannot append more bytes because the packet is allready complete.")
        End If

        Do While Buffer.Position < Buffer.Length
            Select Case Me.State
                Case PacketState.Empty
                    Me.SetState(PacketState.ReadingHeader)
                    Me.setHeader(New DataStream(Me.HeaderLength))

                Case PacketState.ReadingHeader
                    If Buffer.BytesLeft + Me.Header.Length < Me.HeaderLength Then
                        Buffer.WriteToDataStream(Me.Header, CInt(Buffer.BytesLeft))
                    Else
                        Buffer.WriteToDataStream(Me.Header, CInt(Me.HeaderLength - Me.Header.Position))
                        Me.ProcessHeader()

                        Me.SetState(PacketState.ReadingContent)
                        Me.setContent(New DataStream(CInt(Me.ContentLength)))
                    End If

                Case PacketState.ReadingContent
                    If Buffer.BytesLeft + Me.Content.Length < Me.ContentLength Then
                        Buffer.WriteToDataStream(Me.Content, CInt(Buffer.BytesLeft))
                    Else
                        Buffer.WriteToDataStream(Me.Content, CInt(Me.ContentLength - Me.Content.Position))
                        Me.ProcessContent()

                        Me.SetState(PacketState.Complete)
                    End If

                Case PacketState.Complete
                    Exit Do
            End Select

        Loop

    End Sub

    Protected Overridable Sub ProcessHeader()
        Me.Header.Seek(0, IO.SeekOrigin.Begin)
        Me.setContentLength(Me.Header.BR.ReadInt32)
    End Sub
    Protected Overridable Sub ProcessContent()
        Me.Content.Seek(0, IO.SeekOrigin.Begin)
        'Me.setData(Me.Content.BR.ReadBytes(CInt(Me.Content.Length)))
    End Sub




    'Protected Overrides Sub BuildHeader()
    '    MyBase.BuildHeader()
    '    Me.setContentLength(Me.Data.Length)
    '    Me.Header.BW.Write(CInt(Me.ContentLength))
    'End Sub

    'Protected Overrides Sub BuildContent()
    '    MyBase.BuildContent()
    '    Me.Content.BW.Write(Me.Data)
    'End Sub



    'Public Sub BuildPacket()
    '    Me.BuildHeader()
    '    Me.BuildContent()
    '    Me.SetState(PacketState.Complete)
    'End Sub
    'Protected Overridable Sub BuildHeader()
    '    Me.setHeader(New DataStream(Me.HeaderLength))

    'End Sub
    'Protected Overridable Sub BuildContent()
    '    Me.setContent(New DataStream(CInt(Me.ContentLength)))
    'End Sub


    'Public Function ToBytes() As Byte()
    '    Dim Bs As Byte() = New Byte(CInt(Me.Header.Length + Me.Content.Length - 1)) {}
    '    Me.Header.ToArray.CopyTo(Bs, 0)
    '    Me.Content.ToArray.CopyTo(Bs, Me.Header.Length)
    '    Return Bs
    'End Function


    'Public Overrides Function ToString() As String
    '    Return Me.GetType.Name

    'End Function








    'Private mData As Byte()
    'Public ReadOnly Property Data() As Byte()
    '    Get
    '        Return Me.mData
    '    End Get
    'End Property
    'Protected Sub setData(ByVal value As Byte())
    '    Me.mData = value
    '    Me.setContentLength(Me.Data.Length)
    'End Sub

    Public Enum TCPPacketFlags As Byte
        None = 0
        GZipCompressed = 1
    End Enum

    Public Function BuildPacket(ByVal dgram As Byte(), ByVal Flags As TCPPacketFlags) As Byte()
        Dim OutDS As DataStream = Nothing
        Dim InputDS As DataStream = Nothing
        Dim ret As Byte()
        Try
            OutDS = New DataStream
            InputDS = New DataStream

            'build input dgram
            If CByte(Flags And TCPPacketFlags.GZipCompressed) > 0 Then
                InputDS.BW.Write(CInt(dgram.Length))

                Dim GZip As New IO.Compression.GZipStream(InputDS, IO.Compression.CompressionMode.Compress, True)
                GZip.Write(dgram, 0, dgram.Length)
                GZip.Close()
                'versneltruck: kijk of de gecompressed versie wel echt kleiner is
                If InputDS.Length > dgram.Length Then
                    'groter ==> geen compressie gebruiken
                    Return Me.BuildPacket(dgram, Flags Xor TCPPacketFlags.GZipCompressed)

                End If

            Else
                InputDS.Write(dgram, 0, dgram.Length)
            End If

            'header:
            OutDS.BW.Write(CInt(InputDS.Length))
            OutDS.BW.Write(CByte(Flags))

            'content:
            InputDS.WriteTo(OutDS)

            ret = OutDS.ToArray

            InputDS.Close()
            OutDS.Close()

        Catch ex As Exception
            Throw ex
        Finally
            If InputDS IsNot Nothing Then
                InputDS.Close()
                InputDS = Nothing
            End If
            If OutDS IsNot Nothing Then
                OutDS.Close()
                OutDS = Nothing
            End If
        End Try

        Return ret


    End Function

    Public Function GetPacketFlags() As TCPPacketFlags
        Dim F As TCPPacketFlags
        Me.Header.Seek(4, IO.SeekOrigin.Begin)
        F = CType(Me.Header.BR.ReadByte, TCPPacketFlags)

        Return F
    End Function

    Public Function GetPacketDgram() As Byte()
        Dim Flags As TCPPacketFlags = Me.GetPacketFlags
        Dim Dgram As Byte()

        Try



            If CByte(Flags And TCPPacketFlags.GZipCompressed) > 0 Then

                Me.Content.Seek(0, IO.SeekOrigin.Begin)
                Dim OriginalLength As Integer = Me.Content.BR.ReadInt32

                Dim GZip As New IO.Compression.GZipStream(Me.Content, IO.Compression.CompressionMode.Decompress)
                'Dim DecompressedBuffer As Byte() = New Byte(OriginalLength - 1) {}
                'Dim totalCount As Integer = TCPPacketBuilder.ReadAllBytesFromStream(GZip, DecompressedBuffer)
                Dgram = New Byte(OriginalLength - 1) {}
                GZip.Read(Dgram, 0, OriginalLength)
                GZip.Close()

                'Dgram = New Byte(totalCount - 1) {}
                'Array.Copy(DecompressedBuffer, 0, Dgram, 0, totalCount)
            Else
                Dgram = Me.Content.ToArray
            End If












            Me.Header.Close()
            Me.Content.Close()
            Me.SetState(PacketState.Empty)
        Catch ex As Exception
            Throw ex
        End Try

        Return Dgram
    End Function


    Private Shared Function ReadAllBytesFromStream(ByVal stream As IO.Stream, ByVal buffer() As Byte) As Integer
        ' Use this method is used to read all bytes from a stream.
        Dim offset As Integer = 0
        Dim totalCount As Integer = 0
        While True
            Dim bytesRead As Integer = stream.Read(buffer, offset, 100)
            If bytesRead = 0 Then
                Exit While
            End If
            offset += bytesRead
            totalCount += bytesRead
        End While
        Return totalCount
    End Function 'ReadAllBytesFromStream




End Class
