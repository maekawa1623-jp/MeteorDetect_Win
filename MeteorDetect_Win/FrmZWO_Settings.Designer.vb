<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmZWO_Settings
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        ChkCoolerOn = New CheckBox()
        GroupBox1 = New GroupBox()
        LblTargetTemp = New Label()
        TrcTargetTemp = New TrackBar()
        GroupBox2 = New GroupBox()
        Label1 = New Label()
        ChkHighSpeed = New CheckBox()
        LblUsbBandwidth = New Label()
        TrcUsbBandwidth = New TrackBar()
        BtnOK = New Button()
        BtnCancel = New Button()
        Label2 = New Label()
        CmbFlip = New ComboBox()
        GroupBox1.SuspendLayout()
        CType(TrcTargetTemp, ComponentModel.ISupportInitialize).BeginInit()
        GroupBox2.SuspendLayout()
        CType(TrcUsbBandwidth, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' ChkCoolerOn
        ' 
        ChkCoolerOn.AutoSize = True
        ChkCoolerOn.Location = New Point(6, 0)
        ChkCoolerOn.Name = "ChkCoolerOn"
        ChkCoolerOn.Size = New Size(119, 19)
        ChkCoolerOn.TabIndex = 3
        ChkCoolerOn.Text = "冷却機能 (Cooler)"
        ChkCoolerOn.UseVisualStyleBackColor = True
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(LblTargetTemp)
        GroupBox1.Controls.Add(ChkCoolerOn)
        GroupBox1.Controls.Add(TrcTargetTemp)
        GroupBox1.Location = New Point(12, 148)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(360, 72)
        GroupBox1.TabIndex = 6
        GroupBox1.TabStop = False
        ' 
        ' LblTargetTemp
        ' 
        LblTargetTemp.AutoSize = True
        LblTargetTemp.Location = New Point(6, 32)
        LblTargetTemp.Name = "LblTargetTemp"
        LblTargetTemp.Size = New Size(79, 15)
        LblTargetTemp.TabIndex = 6
        LblTargetTemp.Text = "目標温度: 0℃"
        ' 
        ' TrcTargetTemp
        ' 
        TrcTargetTemp.Location = New Point(124, 22)
        TrcTargetTemp.Maximum = 20
        TrcTargetTemp.Minimum = -20
        TrcTargetTemp.Name = "TrcTargetTemp"
        TrcTargetTemp.Size = New Size(230, 45)
        TrcTargetTemp.TabIndex = 5
        ' 
        ' GroupBox2
        ' 
        GroupBox2.Controls.Add(Label1)
        GroupBox2.Controls.Add(ChkHighSpeed)
        GroupBox2.Controls.Add(LblUsbBandwidth)
        GroupBox2.Controls.Add(TrcUsbBandwidth)
        GroupBox2.Location = New Point(12, 12)
        GroupBox2.Name = "GroupBox2"
        GroupBox2.Size = New Size(360, 130)
        GroupBox2.TabIndex = 7
        GroupBox2.TabStop = False
        GroupBox2.Text = "USB設定"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(6, 95)
        Label1.Name = "Label1"
        Label1.Size = New Size(159, 15)
        Label1.TabIndex = 6
        Label1.Text = "※ONにすると画質が低下します"
        ' 
        ' ChkHighSpeed
        ' 
        ChkHighSpeed.AutoSize = True
        ChkHighSpeed.Location = New Point(6, 73)
        ChkHighSpeed.Name = "ChkHighSpeed"
        ChkHighSpeed.Size = New Size(158, 19)
        ChkHighSpeed.TabIndex = 5
        ChkHighSpeed.Text = "High Speed Mode (10bit)"
        ChkHighSpeed.UseVisualStyleBackColor = True
        ' 
        ' LblUsbBandwidth
        ' 
        LblUsbBandwidth.AutoSize = True
        LblUsbBandwidth.Location = New Point(6, 32)
        LblUsbBandwidth.Name = "LblUsbBandwidth"
        LblUsbBandwidth.Size = New Size(86, 15)
        LblUsbBandwidth.TabIndex = 4
        LblUsbBandwidth.Text = "USB帯域: 100%"
        ' 
        ' TrcUsbBandwidth
        ' 
        TrcUsbBandwidth.Location = New Point(124, 22)
        TrcUsbBandwidth.Maximum = 100
        TrcUsbBandwidth.Minimum = 40
        TrcUsbBandwidth.Name = "TrcUsbBandwidth"
        TrcUsbBandwidth.Size = New Size(230, 45)
        TrcUsbBandwidth.TabIndex = 3
        TrcUsbBandwidth.TickFrequency = 5
        TrcUsbBandwidth.Value = 100
        ' 
        ' BtnOK
        ' 
        BtnOK.Location = New Point(272, 266)
        BtnOK.Name = "BtnOK"
        BtnOK.Size = New Size(100, 30)
        BtnOK.TabIndex = 8
        BtnOK.Text = "OK"
        BtnOK.UseVisualStyleBackColor = True
        ' 
        ' BtnCancel
        ' 
        BtnCancel.Location = New Point(166, 266)
        BtnCancel.Name = "BtnCancel"
        BtnCancel.Size = New Size(100, 30)
        BtnCancel.TabIndex = 9
        BtnCancel.Text = "キャンセル"
        BtnCancel.UseVisualStyleBackColor = True
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(12, 229)
        Label2.Name = "Label2"
        Label2.Size = New Size(65, 15)
        Label2.TabIndex = 10
        Label2.Text = "映像の向き:"
        ' 
        ' CmbFlip
        ' 
        CmbFlip.DropDownStyle = ComboBoxStyle.DropDownList
        CmbFlip.FormattingEnabled = True
        CmbFlip.Location = New Point(83, 226)
        CmbFlip.Name = "CmbFlip"
        CmbFlip.Size = New Size(150, 23)
        CmbFlip.TabIndex = 11
        ' 
        ' FrmZWO_Settings
        ' 
        AutoScaleDimensions = New SizeF(96F, 96F)
        AutoScaleMode = AutoScaleMode.Dpi
        ClientSize = New Size(383, 305)
        Controls.Add(CmbFlip)
        Controls.Add(Label2)
        Controls.Add(BtnCancel)
        Controls.Add(BtnOK)
        Controls.Add(GroupBox2)
        Controls.Add(GroupBox1)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "FrmZWO_Settings"
        Text = "ZWO ASIカメラ詳細設定"
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        CType(TrcTargetTemp, ComponentModel.ISupportInitialize).EndInit()
        GroupBox2.ResumeLayout(False)
        GroupBox2.PerformLayout()
        CType(TrcUsbBandwidth, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents ChkCoolerOn As CheckBox
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents LblTargetTemp As Label
    Friend WithEvents TrcTargetTemp As TrackBar
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents ChkHighSpeed As CheckBox
    Friend WithEvents LblUsbBandwidth As Label
    Friend WithEvents TrcUsbBandwidth As TrackBar
    Friend WithEvents Label1 As Label
    Friend WithEvents BtnOK As Button
    Friend WithEvents BtnCancel As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents CmbFlip As ComboBox
End Class
