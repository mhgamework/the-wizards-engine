<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DesignHoofdMenu
    Inherits DesignMainPanel

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.knpQuit = New Common.DesignKnop001
        Me.SuspendLayout()
        '
        'knpQuit
        '
        Me.knpQuit.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.knpQuit.KnopText = "Quit game"
        Me.knpQuit.Location = New System.Drawing.Point(131, 122)
        Me.knpQuit.Name = "knpQuit"
        Me.knpQuit.Size = New System.Drawing.Size(182, 55)
        Me.knpQuit.TabIndex = 0
        '
        'DesignHoofdMenu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.Controls.Add(Me.knpQuit)
        Me.Name = "DesignHoofdMenu"
        Me.Size = New System.Drawing.Size(468, 554)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents knpQuit As Common.DesignKnop001


End Class
