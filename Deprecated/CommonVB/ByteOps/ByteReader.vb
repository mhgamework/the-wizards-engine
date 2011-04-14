Imports System.IO

Public Class ByteReader
    Inherits BaseByteReader

    Public Sub New(ByVal Bytes As Byte())
        MyBase.New(Bytes)
    End Sub
    Public Sub New(ByVal nBaseStream As IO.Stream)
        MyBase.New(nBaseStream)
    End Sub
    Public Sub New(ByVal nMem As MemoryStream)
        MyBase.New(nMem)
    End Sub


    Public Function ReadVector3() As Vector3
        Return New Vector3(Me.ReadSingle, Me.ReadSingle, Me.ReadSingle)
    End Function
    Public Function ReadVector2() As Vector2
        Return New Vector2(Me.ReadSingle, Me.ReadSingle)
    End Function
    Public Function ReadVector4() As Vector4
        Return New Vector4(Me.ReadSingle, Me.ReadSingle, Me.ReadSingle, Me.ReadSingle)
    End Function
    Public Function ReadQuaternion() As Quaternion
        Return New Quaternion(Me.ReadSingle, Me.ReadSingle, Me.ReadSingle, Me.ReadSingle)
    End Function
    Public Function ReadMatrix() As Matrix
        Dim M As Matrix
        M.M11 = Me.ReadSingle
        M.M12 = Me.ReadSingle
        M.M13 = Me.ReadSingle
        M.M14 = Me.ReadSingle
        M.M21 = Me.ReadSingle
        M.M22 = Me.ReadSingle
        M.M23 = Me.ReadSingle
        M.M24 = Me.ReadSingle
        M.M31 = Me.ReadSingle
        M.M32 = Me.ReadSingle
        M.M33 = Me.ReadSingle
        M.M34 = Me.ReadSingle
        M.M41 = Me.ReadSingle
        M.M42 = Me.ReadSingle
        M.M43 = Me.ReadSingle
        M.M44 = Me.ReadSingle

        Return M
    End Function



End Class
