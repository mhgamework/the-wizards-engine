
Namespace Networking
    Public Class DataStream
        Inherits IO.MemoryStream


        Private mBR As IO.BinaryReader
        Public ReadOnly Property BR() As IO.BinaryReader
            Get
                Return Me.mBR
            End Get
        End Property
        Private Sub setBR(ByVal value As IO.BinaryReader)
            Me.mBR = value
        End Sub


        Private mBW As IO.BinaryWriter
        Public ReadOnly Property BW() As IO.BinaryWriter
            Get
                Return Me.mBW
            End Get
        End Property
        Private Sub setBW(ByVal value As IO.BinaryWriter)
            Me.mBW = value
        End Sub


        Public Sub New(ByVal nBuffer As Byte(), ByVal Index As Integer, ByVal Count As Integer)
            MyBase.New(nBuffer, Index, Count)
            Me.CreateBinaryStreams()
        End Sub
        Public Sub New(ByVal nBuffer As Byte())
            Me.New(nBuffer, 0, nBuffer.Length)

        End Sub
        Public Sub New(ByVal Capacity As Integer)
            MyBase.New(Capacity)
            Me.CreateBinaryStreams()
        End Sub
        Public Sub New()
            MyBase.New()
            Me.CreateBinaryStreams()
        End Sub
        Protected Sub CreateBinaryStreams()
            Me.setBR(New IO.BinaryReader(Me))
            Me.setBW(New IO.BinaryWriter(Me))
        End Sub

        Public ReadOnly Property BytesLeft() As Long
            Get
                Return Me.Length - Me.Position
            End Get
        End Property

        Public Sub WriteToDataStream(ByVal nDataStream As DataStream, ByVal nLength As Integer)
            'Dim Buffer As Byte() = New Byte(nLength) {}
            'nDataStream.Read(Buffer, 0, nLength)
            nDataStream.Write(Me.BR.ReadBytes(nLength), 0, nLength)

        End Sub



    End Class
End Namespace