Imports MHGameWork.Game3DPlay.Gui
Public Class LoginMenu
    Inherits Panel

    '''''Private WithEvents mServerComm As ServerCommunication

    Public Sub New(ByVal nParent As SpelObject)
        MyBase.New(nParent)

        'Me.knp = New Knop001(Me)
        'Me.knp.Positie = New Vector2(10, 10)
        'Me.knp.Size = New Vector2(294, 110)
        'Me.knp.Text = "Exit"





        Dim M As New DesignTestMenu
        M.FillPanel(Me)

        ' '' ''Me.Align = AlignType.None
        ' '' ''Me.Anchor = AnchorType.Top Or AnchorType.Right Or AnchorType.Left Or AnchorType.Bottom
        Me.Size = New Vector2(800, 600) 'New Vector2(Me.HoofdObj.XNAGame.Graphics.GraphicsDevice.DisplayMode.Width, Me.HoofdObj.XNAGame.Graphics.GraphicsDevice.DisplayMode.Height)

        ''''''''''Me.settxtUsername(M.txtUsername.GetTextBox2D)
        '' '' ''Me.settxtUsername(New TextBox2D(Me))

        '' '' ''Me.txtUsername.Size = New Vector2(200, 30)
        '' '' ''Me.txtUsername.Positie = New Vector2(Me.Size.X / 2 - Me.txtUsername.Size.X / 2, 100)
        '' '' ''Me.txtUsername.BackgroundColor = Color.FromArgb(255, 0, 102, 0)
        '' '' ''Me.txtUsername.Text = "(username)" '"MHGW"

        ''''''''''Me.settxtPassword(M.txtPassword.GetTextBox2D)
        '' '' ''Me.settxtPassword(New TextBox2D(Me))
        '' '' ''Me.txtPassword.Size = Me.txtUsername.Size
        '' '' ''Me.txtPassword.Positie = Me.txtUsername.Positie + New Vector2(0, Me.txtUsername.Size.Y + 20)
        '' '' ''Me.txtPassword.BackgroundColor = Me.txtUsername.BackgroundColor
        '' '' ''Me.txtPassword.Text = "(password)" '"pass"

        ' '' ''Me.setknpConnect(M.knpConnect.GetKnop001)
        '' '' ''Me.setknpConnect(New Knop001(Me))
        '' '' ''Me.knpConnect.Size = New Vector2(150, 50)
        '' '' ''Me.knpConnect.Positie = New Vector2(Me.Size.X / 2 - Me.knpConnect.Size.X / 2, 200)
        '' '' ''Me.knpConnect.Text = "Connect"

        ' '' '' ''Me.setknpPing(New Knop001(Me))
        ' '' '' ''Me.knpPing.Positie = New Vector2(200, 0)
        ' '' '' ''Me.knpPing.Size = New Vector2(150, 50)
        ' '' '' ''Me.knpPing.Text = "Ping"

        ' '' ''Me.setKnpQuit(M.knpQuit.GetKnop001)

        ''''''''''Me.setOutput(M.TextList2D.GetTextList2D)
        '' '' ''Me.setOutput(New TextList2D(Me))
        '' '' ''Me.Output.BackgroundColor = Color.Red
        '' '' ''Me.Output.Positie = New Vector2(100, 280)
        '' '' ''Me.Output.Size = New Vector2(Me.Size.X - 100 * 2, 300)
        ''''''''''Me.Output.WriteLine("Welcome to The Wizards.")
        '' '' ''Me.Output.Anchor = AnchorType.Left Or AnchorType.Right Or AnchorType.Bottom



        ''''''''''Me.mServerComm = CType(Me.HoofdObj, ClientMain).ServerComm

    End Sub

    Dim knp As Knop001



    '''''Private WithEvents Scheduler As New SchedulerElement(Me)


    ' '' ''Private WithEvents mknpConnect As Knop
    ' '' ''Public ReadOnly Property knpConnect() As Knop
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mknpConnect
    ' '' ''    End Get
    ' '' ''End Property
    ' '' ''Private Sub setknpConnect(ByVal value As Knop)
    ' '' ''    Me.mknpConnect = value
    ' '' ''End Sub


    ' '' ''Private WithEvents mKnpQuit As Knop
    ' '' ''Public ReadOnly Property KnpQuit() As Knop
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mKnpQuit
    ' '' ''    End Get
    ' '' ''End Property
    ' '' ''Private Sub setKnpQuit(ByVal value As Knop)
    ' '' ''    Me.mKnpQuit = value
    ' '' ''End Sub


    ' '' ''Private WithEvents mknpPing As Knop
    ' '' ''Public ReadOnly Property knpPing() As Knop
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mknpPing
    ' '' ''    End Get
    ' '' ''End Property
    ' '' ''Private Sub setknpPing(ByVal value As Knop)
    ' '' ''    Me.mknpPing = value
    ' '' ''End Sub


    ' '' ''Private mOutput As TextList2D
    ' '' ''Public ReadOnly Property Output() As TextList2D
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mOutput
    ' '' ''    End Get
    ' '' ''End Property
    ' '' ''Private Sub setOutput(ByVal value As TextList2D)
    ' '' ''    Me.mOutput = value
    ' '' ''End Sub


    ' '' ''Private mtxtUsername As TextBox2D
    ' '' ''Public ReadOnly Property txtUsername() As TextBox2D
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mtxtUsername
    ' '' ''    End Get
    ' '' ''End Property
    ' '' ''Private Sub settxtUsername(ByVal value As TextBox2D)
    ' '' ''    Me.mtxtUsername = value
    ' '' ''End Sub



    ' '' ''Private mtxtPassword As TextBox2D
    ' '' ''Public ReadOnly Property txtPassword() As TextBox2D
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mtxtPassword
    ' '' ''    End Get
    ' '' ''End Property
    ' '' ''Private Sub settxtPassword(ByVal value As TextBox2D)
    ' '' ''    Me.mtxtPassword = value
    ' '' ''End Sub



    ' '' ''Public Sub DoConnect()
    ' '' ''    CLMain.ServerComm.LoginAsync(Me.txtUsername.Text, Me.txtPassword.Text)
    ' '' ''End Sub

    ' '' ''Private Sub mknpConnect_Clicked(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ClickedElement.ClickedEventArgs) Handles mknpConnect.Clicked
    ' '' ''    If e.Button = MouseOffset.Button0 And e.State = ClickedEvent.MouseState.Down Then
    ' '' ''        Me.DoConnect()
    ' '' ''    End If
    ' '' ''End Sub

    ' '' ''Private Sub mknpPing_Clicked(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ClickedElement.ClickedEventArgs) Handles mknpPing.Clicked
    ' '' ''    If e.Button = MouseOffset.Button0 And e.State = ClickedEvent.MouseState.Down Then
    ' '' ''        CLMain.ServerComm.PingAsync() 'New Net.IPEndPoint(Net.IPAddress.Parse("127.0.0.1"), 5012))
    ' '' ''    End If

    ' '' ''End Sub

    ' '' ''Private Sub mServerComm_LinkUDPConnectionCompleted(ByVal sender As Object, ByVal e As System.EventArgs) Handles mServerComm.LinkUDPConnectionCompleted
    ' '' ''    CLMain.ServerComm.GetEntityAsync(CLMain.ServerComm.Client.LinkedPlayerEntityID)
    ' '' ''    Me.Output.WriteLine("UDP Connection Established.")
    ' '' ''    Me.Output.WriteLine("Starting The Wizards...")
    ' '' ''    Me.Scheduler.ScheduleAction(AddressOf Me.CloseAndActivateWereld, 0)
    ' '' ''End Sub

    ' '' ''Private Sub mServerComm_LoginCompleted(ByVal sender As Object, ByVal e As ServerCommunication.LoginCompletedEventArgs) Handles mServerComm.LoginCompleted
    ' '' ''    Select Case e.Result
    ' '' ''        Case LoginResult.Succes
    ' '' ''            Me.Output.WriteLine("Username and password were veryfied.")
    ' '' ''            Me.Output.WriteLine("Attempting to establish an UDP Connection...")
    ' '' ''            CLMain.ServerComm.Client.SetPlayerData(True, e.PlayerEntityID)
    ' '' ''            CLMain.ServerComm.LinkUDPConnectionAsync(e.LoginAttemptID)


    ' '' ''        Case LoginResult.InvalidUsername
    ' '' ''            Me.Output.WriteLine("Invalid username.")
    ' '' ''            CLMain.ServerComm.Client.SetPlayerData(False, -1)

    ' '' ''        Case LoginResult.UsernamePasswordDontMatch
    ' '' ''            Me.Output.WriteLine("Username or password doesn't match.")
    ' '' ''            CLMain.ServerComm.Client.SetPlayerData(False, -1)

    ' '' ''        Case LoginResult.PlayerEntityNotFound
    ' '' ''            Me.Output.WriteLine("Your PlayerEntity was not found!")
    ' '' ''            CLMain.ServerComm.Client.SetPlayerData(False, -1)

    ' '' ''    End Select
    ' '' ''End Sub

    ' '' ''Private Sub mServerComm_PingCompleted(ByVal sender As Object, ByVal e As ServerCommunication.PingCompletedEventArgs) Handles mServerComm.PingCompleted
    ' '' ''    Me.Output.WriteLine("Pinged the server in {0}ms.", e.PingTime)
    ' '' ''End Sub

    ' '' ''Protected Overrides Sub OnParentSizeChanged(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.Size2DChangedEventArgs)
    ' '' ''    MyBase.OnParentSizeChanged(sender, e)
    ' '' ''End Sub

    ' '' ''Public Sub CloseAndActivateWereld()
    ' '' ''    Me.mServerComm.LaadWereldAsync()
    ' '' ''    Me.Enabled = False
    ' '' ''    CLMain.Wereld.Enabled = True
    ' '' ''End Sub

    ' '' ''Private Sub mKnpQuit_Clicked(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ClickedElement.ClickedEventArgs) Handles mKnpQuit.Clicked
    ' '' ''    If e.State <> Game3DPlay.ClickedElement.ClickedEventArgs.MouseState.Down Or e.Button <> DirectInput.MouseOffset.Button0 Then Exit Sub
    ' '' ''    CLMain.StopSpel()
    ' '' ''End Sub
End Class
