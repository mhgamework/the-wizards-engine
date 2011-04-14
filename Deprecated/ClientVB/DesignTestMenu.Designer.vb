<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DesignTestMenu
    Inherits MHGameWork.Game3DPlay.Gui.Design.DesignMainPanel


    'Control overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Control Designer
    Private components As System.ComponentModel.IContainer

    ' NOTE: The following procedure is required by the Component Designer
    ' It can be modified using the Component Designer.  Do not modify it
    ' using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.DesignPanel1 = New MHGameWork.Game3DPlay.Gui.Design.DesignPanel
        Me.knpQuit = New MHGameWork.TheWizards.Common.DesignKnop001
        Me.DesignPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'DesignPanel1
        '
        Me.DesignPanel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DesignPanel1.BackColor = System.Drawing.Color.Yellow
        Me.DesignPanel1.Controls.Add(Me.knpQuit)
        Me.DesignPanel1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.DesignPanel1.Location = New System.Drawing.Point(72, 78)
        Me.DesignPanel1.Name = "DesignPanel1"
        Me.DesignPanel1.Size = New System.Drawing.Size(589, 281)
        Me.DesignPanel1.TabIndex = 0
        '
        'knpQuit
        '
        Me.knpQuit.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.knpQuit.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.knpQuit.KnopText = "Quit"
        Me.knpQuit.Location = New System.Drawing.Point(352, 177)
        Me.knpQuit.Name = "knpQuit"
        Me.knpQuit.Size = New System.Drawing.Size(150, 52)
        Me.knpQuit.TabIndex = 1
        '
        'DesignTestMenu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.BackColor = System.Drawing.Color.Black
        Me.Controls.Add(Me.DesignPanel1)
        Me.Name = "DesignTestMenu"
        Me.Size = New System.Drawing.Size(729, 445)
        Me.DesignPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents DesignPanel1 As MHGameWork.Game3DPlay.Gui.Design.DesignPanel
    Friend WithEvents knpQuit As Common.DesignKnop001
    '''''Friend WithEvents TextList2D As Common.DesignTextList2D
    '''''Friend WithEvents txtPassword As Common.DesignTextBox2D
    '''''Friend WithEvents txtUsername As Common.DesignTextBox2D


End Class

