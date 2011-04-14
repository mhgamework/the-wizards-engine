<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DesignLoginMenu
    Inherits DesignMainPanel

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
        Me.DesignPanel1 = New MHGameWork.Game3DPlay.DesignPanel
        Me.TextList2D = New Common.DesignTextList2D
        Me.txtPassword = New Common.DesignTextBox2D
        Me.txtUsername = New Common.DesignTextBox2D
        Me.knpQuit = New Common.DesignKnop001
        Me.knpConnect = New Common.DesignKnop001
        Me.DesignPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'DesignPanel1
        '
        Me.DesignPanel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DesignPanel1.BackColor = System.Drawing.Color.Yellow
        Me.DesignPanel1.Controls.Add(Me.TextList2D)
        Me.DesignPanel1.Controls.Add(Me.txtPassword)
        Me.DesignPanel1.Controls.Add(Me.txtUsername)
        Me.DesignPanel1.Controls.Add(Me.knpQuit)
        Me.DesignPanel1.Controls.Add(Me.knpConnect)
        Me.DesignPanel1.Location = New System.Drawing.Point(100, 100)
        Me.DesignPanel1.Name = "DesignPanel1"
        Me.DesignPanel1.Size = New System.Drawing.Size(600, 400)
        Me.DesignPanel1.TabIndex = 0
        '
        'TextList2D
        '
        Me.TextList2D.BackColor = System.Drawing.Color.Gold
        Me.TextList2D.Location = New System.Drawing.Point(3, 3)
        Me.TextList2D.Name = "TextList2D"
        Me.TextList2D.Size = New System.Drawing.Size(594, 114)
        Me.TextList2D.TabIndex = 4
        '
        'txtPassword
        '
        Me.txtPassword.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.txtPassword.Location = New System.Drawing.Point(216, 175)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.Size = New System.Drawing.Size(170, 35)
        Me.txtPassword.TabIndex = 3
        Me.txtPassword.TextBoxText = ""
        '
        'txtUsername
        '
        Me.txtUsername.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.txtUsername.Location = New System.Drawing.Point(216, 134)
        Me.txtUsername.Name = "txtUsername"
        Me.txtUsername.Size = New System.Drawing.Size(170, 35)
        Me.txtUsername.TabIndex = 2
        Me.txtUsername.TextBoxText = ""
        '
        'knpQuit
        '
        Me.knpQuit.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.knpQuit.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.knpQuit.KnopText = "Quit"
        Me.knpQuit.Location = New System.Drawing.Point(322, 269)
        Me.knpQuit.Name = "knpQuit"
        Me.knpQuit.Size = New System.Drawing.Size(150, 52)
        Me.knpQuit.TabIndex = 1
        '
        'knpConnect
        '
        Me.knpConnect.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.knpConnect.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.knpConnect.KnopText = "Connect"
        Me.knpConnect.Location = New System.Drawing.Point(117, 269)
        Me.knpConnect.Name = "knpConnect"
        Me.knpConnect.Size = New System.Drawing.Size(150, 52)
        Me.knpConnect.TabIndex = 0
        '
        'DesignLoginMenu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.BackColor = System.Drawing.Color.Black
        Me.Controls.Add(Me.DesignPanel1)
        Me.Name = "DesignLoginMenu"
        Me.Size = New System.Drawing.Size(800, 600)
        Me.DesignPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents DesignPanel1 As MHGameWork.Game3DPlay.DesignPanel
    Friend WithEvents knpConnect As Common.DesignKnop001
    Friend WithEvents knpQuit As Common.DesignKnop001
    Friend WithEvents TextList2D As Common.DesignTextList2D
    Friend WithEvents txtPassword As Common.DesignTextBox2D
    Friend WithEvents txtUsername As Common.DesignTextBox2D


End Class

