Imports MHGameWork.TheWizards.Common.Networking

Public Class ByteWriter
    Inherits BaseByteWriter


    Public Sub New()
        MyBase.New()

    End Sub
    Public Sub New(ByVal Bytes As Byte())
        MyBase.New(Bytes)

    End Sub
    Public Sub New(ByVal nBaseStream As IO.Stream)
        MyBase.New(nBaseStream)
    End Sub

    Public Overloads Sub Write(ByVal nV As Vector3)
        Me.Write(nV.X)
        Me.Write(nV.Y)
        Me.Write(nV.Z)
    End Sub
    Public Overloads Sub Write(ByVal nV As Vector2)
        Me.Write(nV.X)
        Me.Write(nV.Y)
    End Sub
    Public Overloads Sub Write(ByVal nV As Vector4)
        Me.Write(nV.X)
        Me.Write(nV.Y)
        Me.Write(nV.Z)
        Me.Write(nV.W)
    End Sub

    Public Overloads Sub Write(ByVal nQ As Quaternion)
        Me.Write(nQ.X)
        Me.Write(nQ.Y)
        Me.Write(nQ.Z)
        Me.Write(nQ.W)
    End Sub

    Public Overloads Sub Write(ByVal nM As Matrix)
        Me.Write(nM.M11)
        Me.Write(nM.M12)
        Me.Write(nM.M13)
        Me.Write(nM.M14)
        Me.Write(nM.M21)
        Me.Write(nM.M22)
        Me.Write(nM.M23)
        Me.Write(nM.M24)
        Me.Write(nM.M31)
        Me.Write(nM.M32)
        Me.Write(nM.M33)
        Me.Write(nM.M34)
        Me.Write(nM.M41)
        Me.Write(nM.M42)
        Me.Write(nM.M43)
        Me.Write(nM.M44)
    End Sub

End Class
