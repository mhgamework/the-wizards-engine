Namespace Networking
    Public Class BaseByteReader
        Inherits System.IO.BinaryReader


        Private mMemStrm As IO.MemoryStream
        Private ReadOnly Property MemStrm() As IO.MemoryStream
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mMemStrm
            End Get
        End Property
        Private Sub setMemStrm(ByVal value As IO.MemoryStream)
            Me.mMemStrm = value
        End Sub


        Protected Sub New(ByVal nMemStrm As IO.MemoryStream)
            MyBase.New(nMemStrm)
            Me.setMemStrm(nMemStrm)
        End Sub
        Public Sub New(ByVal Bytes As Byte())
            Me.New(New IO.MemoryStream(Bytes))
        End Sub
        Public Sub New(ByVal nBaseStream As IO.Stream)
            MyBase.New(nBaseStream)
            Me.setMemStrm(Nothing)
        End Sub

        Public Overrides Sub Close()
            MyBase.Close()
            If Me.MemStrm IsNot Nothing Then
                Me.MemStrm.Close()
                Me.setMemStrm(Nothing)
            End If
        End Sub

        Public ReadOnly Property BytesLeft() As Long
            Get
                If Me.MemStrm Is Nothing Then Throw New Exception("Can Bytesleft niet bepalen omdat de onderliggende stream dit niet ondersteund!")
                Return Me.MemStrm.Length - Me.MemStrm.Position
            End Get
        End Property


        Public Function ReadCompressedInt32() As Integer
            Return Me.Read7BitEncodedInt

        End Function


    End Class
End Namespace