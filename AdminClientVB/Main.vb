#Const IsRelease = True
#Const CatchExceptions = IsRelease
'#Const AutoLogin = True 'Not IsRelease

Module Main
    Public ACLMain As AdminClientMain

    Public Sub Main()
        System.Threading.Thread.Sleep(1000)
#If CatchExceptions Then
        Try


            Init()

        Catch ex As Exception
            'Dim FS As IO.FileStream
            'Try
            'FS = New IO.FileStream(Application.StartupPath & "\Output.log", IO.FileMode.Append, IO.FileAccess.Write, IO.FileShare.Delete)
            MsgBox(ex.ToString)

            IO.File.AppendAllText(Forms.Application.StartupPath & "\Output.log", "-----------------------------------------")
            IO.File.AppendAllText(Forms.Application.StartupPath & "\Output.log", ex.ToString)

            'FS.Write()
            For Each I As Object In ex.Data.Keys
                MsgBox(I.ToString & " = " & ex.Data(I).ToString)
            Next
            If ACLMain IsNot Nothing Then
                ACLMain.Dispose()
            End If
            Exit Sub
            'Finally

            'End Try
        End Try

#Else
        Init()
#End If

    End Sub

    Public Sub Init()
        Dim H As New AdminClientMain
        ACLMain = H


        ' '' ''#If AutoLogin Then
        ' '' ''        ACLMain.ServerComm.LoginAsync("MHGW", "pass")
        ' '' ''#End If
        H.Run()
    End Sub
End Module
