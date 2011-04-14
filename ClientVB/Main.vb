#Const IsRelease = False
#Const CatchExceptions = IsRelease
#Const AutoLogin = Not IsRelease

Public Module Main
    Public CLMain As ClientMain

    Public Sub Main()
        System.Threading.Thread.Sleep(1000)
#If CatchExceptions Then
        Try


            Init()

        Catch ex As Exception

            MsgBox(ex.ToString)

            IO.File.AppendAllText(Forms.Application.StartupPath & "\Output.log", "-----------------------------------------")
            IO.File.AppendAllText(Forms.Application.StartupPath & "\Output.log", ex.ToString)

            For Each I As Object In ex.Data.Keys
                MsgBox(I.ToString & " = " & ex.Data(I).ToString)
            Next
            If CLMain IsNot Nothing Then
                CLMain.Dispose()
            End If

        End Try

#Else
        Init()
#End If

    End Sub

    Public Sub Init()
        Dim H As New ClientMain
        CLMain = H
        ' '' ''#If AutoLogin Then
        ' '' ''                CLMain.ServerComm.LoginAsync("MHGW", "pass")
        ' '' ''#End If
        H.Run()


        ' ''Using HoofdObj As ClientMain = New ClientMain()

        ' ''    Dim RM As SpelObjecten.Ruimteschip = New SpelObjecten.Ruimteschip(HoofdObj)
        ' ''    'RM.Positie = New Vector3(0, 0, 0)


        ' ''    'RM = New SpelObjecten.Ruimteschip(HoofdObj)
        ' ''    RM.Positie = New Vector3(2000, 0, 0)



        ' ''    Dim Spec As SpelObjecten.Spectater = New SpelObjecten.Spectater(HoofdObj)

        ' ''    HoofdObj.SetCamera(Spec)


        ' ''    HoofdObj.Run()
        ' ''End Using

    End Sub



End Module
