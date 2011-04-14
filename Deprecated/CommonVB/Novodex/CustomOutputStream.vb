Imports Microsoft.VisualBasic

Public Class CustomOutputStream
    Inherits NxUserOutputStream


    Private mBaseMain As BaseMain
    Public ReadOnly Property BaseMain() As BaseMain
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mBaseMain
        End Get
    End Property
    Private Sub setBaseMain(ByVal value As BaseMain)
        Me.mBaseMain = value
    End Sub


    Public Sub New(ByVal nBaseMain As BaseMain)
        Me.setBaseMain(nBaseMain)
    End Sub
    ' Methods
    Public Overrides Sub print(ByVal message As String)
        Me.BaseMain.WriteLine(("Message=" & message))
        Stop
    End Sub

    Public Overrides Function reportAssertViolation(ByVal message As String, ByVal fileName As String, ByVal lineNumber As Integer) As NxAssertResponse
        Me.BaseMain.WriteLine("ReportAssertViolation:")
        Me.BaseMain.WriteLine((ChrW(9) & "Message=" & message))
        Me.BaseMain.WriteLine((ChrW(9) & "FileName=" & fileName))
        Me.BaseMain.WriteLine((ChrW(9) & "LineNumber=" & lineNumber))
        Stop
        Return NxAssertResponse.NX_AR_IGNORE
    End Function

    Public Overrides Sub reportError(ByVal errorCode As NxErrorCode, ByVal message As String, ByVal fileName As String, ByVal lineNumber As Integer)
        Me.BaseMain.WriteLine("ReportError:")
        Me.BaseMain.WriteLine((ChrW(9) & "ErrorCode=" & errorCode))
        Me.BaseMain.WriteLine((ChrW(9) & "Message=" & message))
        Me.BaseMain.WriteLine((ChrW(9) & "FileName=" & fileName))
        Me.BaseMain.WriteLine((ChrW(9) & "LineNumber=" & lineNumber))
        'Stop
    End Sub

End Class


