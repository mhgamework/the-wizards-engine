Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Namespace Networking
    Public MustInherit Class BaseConnection
        Implements IDisposable

        Public Sub New()
            MyBase.New()

        End Sub

        Private mReceiveThread As Thread
        Public ReadOnly Property ReceiveThread() As Thread
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mReceiveThread
            End Get
        End Property
        Private Sub setReceiveThread(ByVal value As Thread)
            Me.mReceiveThread = value
        End Sub

        Private mReceiving As Boolean
        Public Property Receiving() As Boolean
            <System.Diagnostics.DebuggerStepThrough()> Get
                SyncLock Me
                    Return Me.mReceiving
                End SyncLock
            End Get
            Set(ByVal value As Boolean)
                SyncLock Me
                    Me.mReceiving = value

                    If Me.Receiving Then
                        If Me.ReceiveThread Is Nothing Then
                            Me.setReceiveThread(New Thread(AddressOf Me.ReceiveMessages))
                            ReceiveThread.Name = "ReceiveThread"
                            Me.ReceiveThread.IsBackground = True
                            Me.ReceiveThread.Start()

                        End If
                    End If
                End SyncLock
            End Set
        End Property



        ''' <summary>
        ''' THIS METHOD RUNS NOT IN MAIN THREAD
        ''' </summary>
        ''' <remarks></remarks>
        Protected MustOverride Sub ReceiveMessageJob()


        ''' <summary>
        ''' Note: goes into infinite failure loop when trying to close the tcp connection locally
        ''' </summary>
        ''' <remarks>Not Thread safe! draait niet in main thread</remarks>
        Private Sub ReceiveMessages()
            Do While Receiving
                Try
                    Me.ReceiveMessageJob()
                    If Me.Receiving = False Then Exit Sub

                Catch se As SocketException
                    Select Case se.SocketErrorCode
                        Case SocketError.Interrupted
                            If Me.Receiving = False Then
                                Exit Sub
                            End If
                        Case SocketError.ConnectionAborted
                            If (Me.Receiving = False) Then
                                Exit Sub
                            End If
                        Case SocketError.ConnectionReset
                            Me.Receiving = False
                            Exit Sub


                    End Select
                    Me.OnNetworkErrorAsync(Nothing, New NetworkErrorEventArgs(NetworkErrorEventArgs.ErrorType.ReceiveError, se))
                    'MsgBox(se.ToString)
                Catch ex As Exception
                    'MsgBox(ex.ToString)
                    Me.OnNetworkErrorAsync(Nothing, New NetworkErrorEventArgs(NetworkErrorEventArgs.ErrorType.ReceiveError, ex))
                Finally


                End Try
            Loop
            Me.Receiving = False
        End Sub


        Public Event PacketRecievedAsync(ByVal sender As Object, ByVal e As PacketRecievedEventArgs)
        Protected Overridable Sub OnPacketRecievedAsync(ByVal sender As Object, ByVal e As PacketRecievedEventArgs)
            RaiseEvent PacketRecievedAsync(sender, e)
        End Sub


        Private mSendQueue As New Queue(Of QueuedSendPacket)


        Private mSendThread As Thread
        Public ReadOnly Property SendThread() As Thread
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mSendThread
            End Get
        End Property
        Private Sub setSendThread(ByVal value As Thread)
            Me.mSendThread = value
        End Sub


        Public Sub SendPacket(ByVal dgram As Byte(), ByVal nEndPoint As IPEndPoint)
            If Me.SendThread Is Nothing Then
                Me.setSendThread(New Thread(New ThreadStart(AddressOf SendMessages)))
                Me.SendThread.IsBackground = True
                Me.SendThread.Name = "SendThread"
                Me.SendThread.Start()
            End If

            SyncLock Me.mSendQueue
                Me.mSendQueue.Enqueue(New QueuedSendPacket(dgram, nEndPoint))

                Monitor.Pulse(Me.mSendQueue)
            End SyncLock

        End Sub


        ''' <summary>
        ''' THIS METHOD RUNS NOT IN MAIN THREAD
        ''' </summary>
        ''' <param name="nPacket"></param>
        ''' <remarks></remarks>
        Protected MustOverride Sub SendMessageJob(ByVal nPacket As QueuedSendPacket)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>Not Thread safe! draait niet in main thread</remarks>
        Private Sub SendMessages()
            Dim Packet As QueuedSendPacket

            Do

                Packet = Nothing


                SyncLock Me.mSendQueue

                    Do While Me.mSendQueue.Count = 0

                        If Me.DisposedValue = True Then Exit Sub

                        Monitor.Wait(Me.mSendQueue)

                    Loop

                    Packet = Me.mSendQueue.Dequeue()

                End SyncLock


                Try

                    Me.SendMessageJob(Packet)


                Catch se As SocketException
                    'TODO: geen error sturen als de socket de verzending gewoon heeft geannuleerd


                    Me.OnNetworkErrorAsync(Nothing, New NetworkErrorEventArgs(NetworkErrorEventArgs.ErrorType.SendError, se))
                Catch ex As Exception
                    Me.OnNetworkErrorAsync(Nothing, New NetworkErrorEventArgs(NetworkErrorEventArgs.ErrorType.SendError, ex))
                Finally


                End Try




            Loop

        End Sub



        Public Sub OnNetworkErrorAsync(ByVal sender As Object, ByVal e As NetworkErrorEventArgs)
            RaiseEvent NetworkErrorAsync(sender, e)
        End Sub

        Public Event NetworkErrorAsync As EventHandler(Of NetworkErrorEventArgs)




        Private mDisposedValueLock As New Object()
        Private mDisposedValue As Boolean = False ' To detect redundant calls 
        Public ReadOnly Property DisposedValue() As Boolean
            Get
                SyncLock Me.mDisposedValueLock
                    Return Me.mDisposedValue
                End SyncLock
            End Get
        End Property
        Private Sub setDisposedValue(ByVal value As Boolean)
            SyncLock Me.mDisposedValueLock
                Me.mDisposedValue = value
            End SyncLock
        End Sub

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.DisposedValue Then
                If disposing Then
                    ' TODO: free unmanaged resources when explicitly called
                End If

                ' TODO: free shared unmanaged resources
            End If

            Me.Receiving = False
            Me.CloseSocket()
            Me.setDisposedValue(True)
        End Sub

        Protected Overridable Sub CloseSocket()

        End Sub

#Region " IDisposable Support "
        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region







        Public Class PacketRecievedEventArgs
            Inherits System.EventArgs

            Public Sub New(ByVal dgram As Byte(), ByVal EP As IPEndPoint)
                Me.setDgram(dgram)
                Me.setEP(EP)
            End Sub


            Private mDgram As Byte()
            Public ReadOnly Property Dgram() As Byte()
                <System.Diagnostics.DebuggerStepThrough()> Get
                    Return Me.mDgram
                End Get
            End Property
            Private Sub setDgram(ByVal value As Byte())
                Me.mDgram = value
            End Sub


            Private mEP As IPEndPoint
            Public ReadOnly Property EP() As IPEndPoint
                <System.Diagnostics.DebuggerStepThrough()> Get
                    Return Me.mEP
                End Get
            End Property
            Private Sub setEP(ByVal value As IPEndPoint)
                Me.mEP = value
            End Sub



        End Class

        Public Class QueuedSendPacket

            Public Sub New(ByVal dgram As Byte(), ByVal EP As IPEndPoint)
                Me.setDgram(dgram)
                Me.setEP(EP)
            End Sub


            Private mDgram As Byte()
            Public ReadOnly Property Dgram() As Byte()
                <System.Diagnostics.DebuggerStepThrough()> Get
                    Return Me.mDgram
                End Get
            End Property
            Private Sub setDgram(ByVal value As Byte())
                Me.mDgram = value
            End Sub


            Private mEP As IPEndPoint
            Public ReadOnly Property EP() As IPEndPoint
                <System.Diagnostics.DebuggerStepThrough()> Get
                    Return Me.mEP
                End Get
            End Property
            Private Sub setEP(ByVal value As IPEndPoint)
                Me.mEP = value
            End Sub



        End Class



        Public Class NetworkErrorEventArgs
            Inherits EventArgs

            Public Enum ErrorType
                ReceiveError
                SendError

            End Enum

            Public Sub New(ByVal nType As ErrorType, ByVal nEx As Exception)
                Me.setType(nType)
                Me.setEx(nEx)
            End Sub


            Private mType As ErrorType
            Public ReadOnly Property Type() As ErrorType
                <System.Diagnostics.DebuggerStepThrough()> Get
                    Return Me.mType
                End Get
            End Property
            Private Sub setType(ByVal value As ErrorType)
                Me.mType = value
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

    End Class
End Namespace