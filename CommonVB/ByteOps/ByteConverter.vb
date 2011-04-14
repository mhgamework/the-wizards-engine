Public Class ByteConverter
    Public Shared Function GetBytes(ByVal V As Vector3) As Byte()
        Dim B As Byte() = New Byte(12 - 1) {}
        BitConverter.GetBytes(V.X).CopyTo(B, 0)
        BitConverter.GetBytes(V.Y).CopyTo(B, 4)
        BitConverter.GetBytes(V.Z).CopyTo(B, 8)
        Return B
    End Function

    Public Shared Function GetBytes(ByVal M As Matrix) As Byte()
        Dim B As Byte() = New Byte(4 * 4 * 4 - 1) {}
        BitConverter.GetBytes(M.M11).CopyTo(B, 0)
        BitConverter.GetBytes(M.M12).CopyTo(B, 4)
        BitConverter.GetBytes(M.M13).CopyTo(B, 8)
        BitConverter.GetBytes(M.M14).CopyTo(B, 12)
        BitConverter.GetBytes(M.M21).CopyTo(B, 16)
        BitConverter.GetBytes(M.M22).CopyTo(B, 20)
        BitConverter.GetBytes(M.M23).CopyTo(B, 24)
        BitConverter.GetBytes(M.M24).CopyTo(B, 28)
        BitConverter.GetBytes(M.M31).CopyTo(B, 32)
        BitConverter.GetBytes(M.M32).CopyTo(B, 36)
        BitConverter.GetBytes(M.M33).CopyTo(B, 40)
        BitConverter.GetBytes(M.M34).CopyTo(B, 44)
        BitConverter.GetBytes(M.M41).CopyTo(B, 48)
        BitConverter.GetBytes(M.M42).CopyTo(B, 52)
        BitConverter.GetBytes(M.M43).CopyTo(B, 56)
        BitConverter.GetBytes(M.M44).CopyTo(B, 60)
        Return B
    End Function

    Public Shared Function GetBytes(ByVal Q As Quaternion) As Byte()
        Dim B As Byte() = New Byte(4 * 4 - 1) {}
        BitConverter.GetBytes(Q.X).CopyTo(B, 0)
        BitConverter.GetBytes(Q.Y).CopyTo(B, 4)
        BitConverter.GetBytes(Q.Z).CopyTo(B, 8)
        BitConverter.GetBytes(Q.W).CopyTo(B, 12)
        Return B
    End Function

End Class
