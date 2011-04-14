Public Class AdminClientMain
    Inherits ClientMain

    Public Sub New()
        MyBase.New()
        ACLMain = Me


    End Sub

    ' '' ''Public Shadows ReadOnly Property ServerComm() As AdminServerCommunication
    ' '' ''    Get
    ' '' ''        Return DirectCast(MyBase.ServerComm, AdminServerCommunication)
    ' '' ''    End Get
    ' '' ''End Property

    ' '' ''Protected Overrides Function CreateHoofdMenu() As Client.HoofdMenu
    ' '' ''    Return New HoofdMenuAdmin(Me)
    ' '' ''End Function

    ' '' ''Protected Overrides Function CreateServerCommunication(ByVal nServerAddress As System.Net.IPAddress, ByVal nServerUDPPort As Integer, ByVal nServerTCPPort As Integer) As Client.ServerCommunication
    ' '' ''    Return New AdminServerCommunication(Me, nServerAddress, nServerUDPPort, nServerTCPPort)
    ' '' ''End Function


    ' '' ''Protected Overrides Sub mProcessEventElement_Process(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ProcessElement.ProcessEventArgs)
    ' '' ''    MyBase.mProcessEventElement_Process(sender, e)

    ' '' ''End Sub



End Class
