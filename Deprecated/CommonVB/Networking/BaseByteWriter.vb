Namespace Networking
    Public Class BaseByteWriter
        Inherits System.IO.BinaryWriter


        Private mMemStrm As IO.MemoryStream
        Public ReadOnly Property MemStrm() As IO.MemoryStream
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


        Public Sub New()
            Me.New(New IO.MemoryStream)

        End Sub
        Public Sub New(ByVal Bytes As Byte())
            Me.New()
            Me.Write(Bytes)
        End Sub
        Public Sub New(ByVal nBaseStream As IO.Stream)
            MyBase.New(nBaseStream)
            Me.setMemStrm(Nothing)
        End Sub

        Public Function ToBytes() As Byte()
            Return Me.MemStrm.ToArray
            'Return Me.MemStrm.GetBuffer
        End Function

        Public Overrides Sub Close()
            MyBase.Close()
            If Me.MemStrm IsNot Nothing Then
                Me.MemStrm.Close()
                Me.setMemStrm(Nothing)
            End If

        End Sub

        Public Function ToBytesAndClose() As Byte()
            Dim B As Byte()
            B = Me.ToBytes
            Me.Close()

            Return B
        End Function

        Public Overloads Sub Write(ByVal nINS As INetworkSerializable)
            Me.Write(nINS.ToNetworkBytes)
        End Sub

        Public Overloads Sub WriteCompressed(ByVal value As Integer)
            Me.Write7BitEncodedInt(value)
        End Sub
    End Class
End Namespace