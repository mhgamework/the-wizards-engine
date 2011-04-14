Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Public Class TCPConnectionListener
    Implements IDisposable

    Public Sub New(ByVal nListenerPort As Integer)
        Me.setListener(New TcpListener(New IPEndPoint(IPAddress.Any, nListenerPort)))
        'Me.Listening = True
        'Throw New Exception("This is an old version of this class, and is disabled to prevent wierd things happening") enabled again for old tests
    End Sub


    Private mListener As TcpListener
    Public ReadOnly Property Listener() As TcpListener
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mListener
        End Get
    End Property
    Private Sub setListener(ByVal value As TcpListener)
        Me.mListener = value
    End Sub

    Private mListenThread As Thread
    Public ReadOnly Property ListenThread() As Thread
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mListenThread
        End Get
    End Property
    Private Sub setListenThread(ByVal value As Thread)
        Me.mListenThread = value
    End Sub

    Private mListening As Boolean
    Public Property Listening() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            SyncLock Me
                Return Me.mListening
            End SyncLock
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Boolean)
            Me.mListening = value
            If Me.Listening Then
                If Me.ListenThread Is Nothing Then
                    Me.setListenThread(New Thread(AddressOf Me.ListenForClients))
                    Me.ListenThread.Start()
                End If
            End If
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Not Thread safe! draait niet in main thread</remarks>
    Private Sub ListenForClients()
        Me.Listener.Start()

        Try
            Do While Me.Listening
                Dim CL As TcpClient
                CL = Me.Listener.AcceptTcpClient()

                If Me.Listening = False Then Exit Sub

                Me.OnClientConnected(New ClientConnectedEventArgs(CL))
                CL = Nothing
            Loop

        Catch se As SocketException
            Select Case se.SocketErrorCode
                Case SocketError.Interrupted
                    If Me.Listening = False Then
                        Exit Sub
                    End If

            End Select
            Me.OnListenerError(New ListenerErrorEventArgs(se))

        Catch ex As Exception
            Me.OnListenerError(New ListenerErrorEventArgs(ex))
        Finally

            If Me.Listener IsNot Nothing Then Me.Listener.Stop()
        End Try

    End Sub

    Public Class ListenerErrorEventArgs
        Inherits System.EventArgs
        Public Sub New(ByVal nEx As Exception)
            Me.setEx(Ex)
        End Sub

        Private mEx As Exception
        Public ReadOnly Property Ex() As Exception
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mEx
            End Get
        End Property
        Private Sub setEx(ByVal value As Exception)
            Me.mEx = value
        End Sub

    End Class
    Public Sub OnListenerError(ByVal e As ListenerErrorEventArgs)
        RaiseEvent ListenerError(Me, e)
    End Sub
    Public Event ListenerError(ByVal sender As Object, ByVal e As ListenerErrorEventArgs)


    Public Class ClientConnectedEventArgs
        Inherits System.EventArgs
        Public Sub New(ByVal nCL As TcpClient)
            Me.setCL(nCL)
        End Sub


        Private mCL As TcpClient
        Public ReadOnly Property CL() As TcpClient
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mCL
            End Get
        End Property
        Private Sub setCL(ByVal value As TcpClient)
            Me.mCL = value
        End Sub

    End Class
    'Make private?
    Public Sub OnClientConnected(ByVal e As ClientConnectedEventArgs)
        RaiseEvent ClientConnected(Me, e)
    End Sub
    Public Event ClientConnected(ByVal sender As Object, ByVal e As ClientConnectedEventArgs)


    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: free unmanaged resources when explicitly called
            End If

            ' TODO: free shared unmanaged resources
        End If

        Me.Listening = False
        Me.Listener.Stop() 'TODO
        'Me.Listener.Close() 'TODO

        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
