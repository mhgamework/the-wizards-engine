<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DesignAddGameFileMenu
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
        Me.knpCancel = New MHGameWork.TheWizards.Common.DesignKnop001
        Me.knpAddFile = New MHGameWork.TheWizards.Common.DesignKnop001
        Me.txtLocalFile = New MHGameWork.TheWizards.Common.DesignTextBox2D
        Me.txtEnabled = New MHGameWork.TheWizards.Common.DesignTextBox2D
        Me.txtType = New MHGameWork.TheWizards.Common.DesignTextBox2D
        Me.txtRelativePath = New MHGameWork.TheWizards.Common.DesignTextBox2D
        Me.txtDescription = New MHGameWork.TheWizards.Common.DesignTextBox2D
        Me.DesignLabel7 = New MHGameWork.TheWizards.Common.DesignLabel
        Me.DesignLabel6 = New MHGameWork.TheWizards.Common.DesignLabel
        Me.DesignLabel5 = New MHGameWork.TheWizards.Common.DesignLabel
        Me.DesignLabel4 = New MHGameWork.TheWizards.Common.DesignLabel
        Me.txtFileName = New MHGameWork.TheWizards.Common.DesignTextBox2D
        Me.DesignLabel3 = New MHGameWork.TheWizards.Common.DesignLabel
        Me.DesignLabel1 = New MHGameWork.TheWizards.Common.DesignLabel
        Me.knpBladeren = New MHGameWork.TheWizards.Common.DesignKnop001
        Me.SuspendLayout()
        '
        'knpCancel
        '
        Me.knpCancel.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.knpCancel.KnopText = "Cancel"
        Me.knpCancel.Location = New System.Drawing.Point(446, 415)
        Me.knpCancel.Name = "knpCancel"
        Me.knpCancel.Size = New System.Drawing.Size(176, 48)
        Me.knpCancel.TabIndex = 40
        '
        'knpAddFile
        '
        Me.knpAddFile.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.knpAddFile.KnopText = "Upload/Add File"
        Me.knpAddFile.Location = New System.Drawing.Point(166, 415)
        Me.knpAddFile.Name = "knpAddFile"
        Me.knpAddFile.Size = New System.Drawing.Size(176, 48)
        Me.knpAddFile.TabIndex = 39
        '
        'txtLocalFile
        '
        Me.txtLocalFile.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.txtLocalFile.Location = New System.Drawing.Point(133, 67)
        Me.txtLocalFile.Name = "txtLocalFile"
        Me.txtLocalFile.Size = New System.Drawing.Size(579, 36)
        Me.txtLocalFile.TabIndex = 38
        Me.txtLocalFile.TextBoxText = ""
        '
        'txtEnabled
        '
        Me.txtEnabled.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.txtEnabled.Location = New System.Drawing.Point(133, 345)
        Me.txtEnabled.Name = "txtEnabled"
        Me.txtEnabled.Size = New System.Drawing.Size(334, 36)
        Me.txtEnabled.TabIndex = 36
        Me.txtEnabled.TextBoxText = ""
        '
        'txtType
        '
        Me.txtType.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.txtType.Location = New System.Drawing.Point(133, 303)
        Me.txtType.Name = "txtType"
        Me.txtType.Size = New System.Drawing.Size(334, 36)
        Me.txtType.TabIndex = 35
        Me.txtType.TextBoxText = ""
        '
        'txtRelativePath
        '
        Me.txtRelativePath.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.txtRelativePath.Location = New System.Drawing.Point(133, 261)
        Me.txtRelativePath.Name = "txtRelativePath"
        Me.txtRelativePath.Size = New System.Drawing.Size(334, 36)
        Me.txtRelativePath.TabIndex = 32
        Me.txtRelativePath.TextBoxText = ""
        '
        'txtDescription
        '
        Me.txtDescription.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.txtDescription.Location = New System.Drawing.Point(133, 219)
        Me.txtDescription.Name = "txtDescription"
        Me.txtDescription.Size = New System.Drawing.Size(334, 36)
        Me.txtDescription.TabIndex = 31
        Me.txtDescription.TextBoxText = ""
        '
        'DesignLabel7
        '
        Me.DesignLabel7.LabelText = "Type"
        Me.DesignLabel7.Location = New System.Drawing.Point(19, 303)
        Me.DesignLabel7.Name = "DesignLabel7"
        Me.DesignLabel7.Size = New System.Drawing.Size(108, 36)
        Me.DesignLabel7.TabIndex = 27
        Me.DesignLabel7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'DesignLabel6
        '
        Me.DesignLabel6.LabelText = "RelativePath"
        Me.DesignLabel6.Location = New System.Drawing.Point(19, 261)
        Me.DesignLabel6.Name = "DesignLabel6"
        Me.DesignLabel6.Size = New System.Drawing.Size(108, 36)
        Me.DesignLabel6.TabIndex = 26
        Me.DesignLabel6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'DesignLabel5
        '
        Me.DesignLabel5.LabelText = "Description"
        Me.DesignLabel5.Location = New System.Drawing.Point(19, 219)
        Me.DesignLabel5.Name = "DesignLabel5"
        Me.DesignLabel5.Size = New System.Drawing.Size(108, 36)
        Me.DesignLabel5.TabIndex = 25
        Me.DesignLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'DesignLabel4
        '
        Me.DesignLabel4.LabelText = "Local File:"
        Me.DesignLabel4.Location = New System.Drawing.Point(19, 67)
        Me.DesignLabel4.Name = "DesignLabel4"
        Me.DesignLabel4.Size = New System.Drawing.Size(108, 36)
        Me.DesignLabel4.TabIndex = 24
        Me.DesignLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtFileName
        '
        Me.txtFileName.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.txtFileName.Location = New System.Drawing.Point(133, 177)
        Me.txtFileName.Name = "txtFileName"
        Me.txtFileName.Size = New System.Drawing.Size(334, 36)
        Me.txtFileName.TabIndex = 23
        Me.txtFileName.TextBoxText = ""
        '
        'DesignLabel3
        '
        Me.DesignLabel3.LabelText = "Enabled"
        Me.DesignLabel3.Location = New System.Drawing.Point(19, 345)
        Me.DesignLabel3.Name = "DesignLabel3"
        Me.DesignLabel3.Size = New System.Drawing.Size(108, 36)
        Me.DesignLabel3.TabIndex = 22
        Me.DesignLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'DesignLabel1
        '
        Me.DesignLabel1.LabelText = "Filename"
        Me.DesignLabel1.Location = New System.Drawing.Point(19, 177)
        Me.DesignLabel1.Name = "DesignLabel1"
        Me.DesignLabel1.Size = New System.Drawing.Size(108, 36)
        Me.DesignLabel1.TabIndex = 20
        Me.DesignLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'knpBladeren
        '
        Me.knpBladeren.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.knpBladeren.KnopText = "Bladeren..."
        Me.knpBladeren.Location = New System.Drawing.Point(536, 109)
        Me.knpBladeren.Name = "knpBladeren"
        Me.knpBladeren.Size = New System.Drawing.Size(176, 48)
        Me.knpBladeren.TabIndex = 41
        '
        'DesignAddGameFileMenu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.Controls.Add(Me.knpBladeren)
        Me.Controls.Add(Me.knpCancel)
        Me.Controls.Add(Me.knpAddFile)
        Me.Controls.Add(Me.txtLocalFile)
        Me.Controls.Add(Me.txtEnabled)
        Me.Controls.Add(Me.txtType)
        Me.Controls.Add(Me.txtRelativePath)
        Me.Controls.Add(Me.txtDescription)
        Me.Controls.Add(Me.DesignLabel7)
        Me.Controls.Add(Me.DesignLabel6)
        Me.Controls.Add(Me.DesignLabel5)
        Me.Controls.Add(Me.DesignLabel4)
        Me.Controls.Add(Me.txtFileName)
        Me.Controls.Add(Me.DesignLabel3)
        Me.Controls.Add(Me.DesignLabel1)
        Me.Name = "DesignAddGameFileMenu"
        Me.Size = New System.Drawing.Size(792, 548)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents txtEnabled As Common.DesignTextBox2D
    Friend WithEvents txtType As Common.DesignTextBox2D
    Friend WithEvents txtRelativePath As Common.DesignTextBox2D
    Friend WithEvents txtDescription As Common.DesignTextBox2D
    Friend WithEvents DesignLabel7 As Common.DesignLabel
    Friend WithEvents DesignLabel6 As Common.DesignLabel
    Friend WithEvents DesignLabel5 As Common.DesignLabel
    Friend WithEvents DesignLabel4 As Common.DesignLabel
    Friend WithEvents txtFileName As Common.DesignTextBox2D
    Friend WithEvents DesignLabel3 As Common.DesignLabel
    Friend WithEvents DesignLabel1 As Common.DesignLabel
    Friend WithEvents txtLocalFile As Common.DesignTextBox2D
    Friend WithEvents knpAddFile As Common.DesignKnop001
    Friend WithEvents knpCancel As Common.DesignKnop001
    Friend WithEvents knpBladeren As MHGameWork.TheWizards.Common.DesignKnop001


End Class
