<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmSettings
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
        TabControl = New TabControl()
        TabSave = New TabPage()
        Label4 = New Label()
        Label3 = New Label()
        BtnSelectContinuousDir = New Button()
        TxtContinuousDir = New TextBox()
        BtnBrowse = New Button()
        TxtSavePath = New TextBox()
        Label1 = New Label()
        TabDetect = New TabPage()
        GroupBox1 = New GroupBox()
        LblClaheClip = New Label()
        TrcClaheClip = New TrackBar()
        ChkEnableClahe = New CheckBox()
        Label12 = New Label()
        TrcHough = New TrackBar()
        TrcLineGap = New TrackBar()
        Label9 = New Label()
        LblHough = New Label()
        Label8 = New Label()
        LblLineGap = New Label()
        Label7 = New Label()
        LblAccSec = New Label()
        TrcAccSec = New TrackBar()
        LblMinLength = New Label()
        Label6 = New Label()
        Label5 = New Label()
        TrcMinLength = New TrackBar()
        TrcThreshold = New TrackBar()
        LblThreshold = New Label()
        BtnTestSound = New Button()
        Label2 = New Label()
        BtnBrowseSound = New Button()
        TxtAlertSound = New TextBox()
        TabRecord = New TabPage()
        Label11 = New Label()
        TrcPostRoll = New TrackBar()
        LblPostRoll = New Label()
        LblPreRoll = New Label()
        Label10 = New Label()
        TrcPreRoll = New TrackBar()
        TabSNS = New TabPage()
        BtnCancel = New Button()
        BtnOK = New Button()
        Label13 = New Label()
        TabControl.SuspendLayout()
        TabSave.SuspendLayout()
        TabDetect.SuspendLayout()
        GroupBox1.SuspendLayout()
        CType(TrcClaheClip, ComponentModel.ISupportInitialize).BeginInit()
        CType(TrcHough, ComponentModel.ISupportInitialize).BeginInit()
        CType(TrcLineGap, ComponentModel.ISupportInitialize).BeginInit()
        CType(TrcAccSec, ComponentModel.ISupportInitialize).BeginInit()
        CType(TrcMinLength, ComponentModel.ISupportInitialize).BeginInit()
        CType(TrcThreshold, ComponentModel.ISupportInitialize).BeginInit()
        TabRecord.SuspendLayout()
        CType(TrcPostRoll, ComponentModel.ISupportInitialize).BeginInit()
        CType(TrcPreRoll, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' TabControl
        ' 
        TabControl.Controls.Add(TabSave)
        TabControl.Controls.Add(TabDetect)
        TabControl.Controls.Add(TabRecord)
        TabControl.Controls.Add(TabSNS)
        TabControl.Dock = DockStyle.Top
        TabControl.Location = New Point(0, 0)
        TabControl.Name = "TabControl"
        TabControl.SelectedIndex = 0
        TabControl.Size = New Size(586, 438)
        TabControl.TabIndex = 5
        ' 
        ' TabSave
        ' 
        TabSave.Controls.Add(Label4)
        TabSave.Controls.Add(Label3)
        TabSave.Controls.Add(BtnSelectContinuousDir)
        TabSave.Controls.Add(TxtContinuousDir)
        TabSave.Controls.Add(BtnBrowse)
        TabSave.Controls.Add(TxtSavePath)
        TabSave.Controls.Add(Label1)
        TabSave.Location = New Point(4, 24)
        TabSave.Name = "TabSave"
        TabSave.Padding = New Padding(3)
        TabSave.Size = New Size(578, 410)
        TabSave.TabIndex = 0
        TabSave.Text = "保存設定"
        TabSave.UseVisualStyleBackColor = True
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(8, 100)
        Label4.Name = "Label4"
        Label4.Size = New Size(299, 15)
        Label4.TabIndex = 11
        Label4.Text = "※未指定の場合は、検知動画と同じフォルダに保存されます。"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(8, 56)
        Label3.Name = "Label3"
        Label3.Size = New Size(104, 15)
        Label3.TabIndex = 10
        Label3.Text = "常時録画の保存先:"
        ' 
        ' BtnSelectContinuousDir
        ' 
        BtnSelectContinuousDir.Location = New Point(312, 74)
        BtnSelectContinuousDir.Name = "BtnSelectContinuousDir"
        BtnSelectContinuousDir.Size = New Size(80, 23)
        BtnSelectContinuousDir.TabIndex = 9
        BtnSelectContinuousDir.Text = "参照..."
        BtnSelectContinuousDir.UseVisualStyleBackColor = True
        ' 
        ' TxtContinuousDir
        ' 
        TxtContinuousDir.Location = New Point(8, 74)
        TxtContinuousDir.Name = "TxtContinuousDir"
        TxtContinuousDir.Size = New Size(298, 23)
        TxtContinuousDir.TabIndex = 8
        ' 
        ' BtnBrowse
        ' 
        BtnBrowse.Location = New Point(312, 21)
        BtnBrowse.Name = "BtnBrowse"
        BtnBrowse.Size = New Size(80, 23)
        BtnBrowse.TabIndex = 7
        BtnBrowse.Text = "参照..."
        BtnBrowse.UseVisualStyleBackColor = True
        ' 
        ' TxtSavePath
        ' 
        TxtSavePath.Location = New Point(6, 21)
        TxtSavePath.Name = "TxtSavePath"
        TxtSavePath.Size = New Size(300, 23)
        TxtSavePath.TabIndex = 6
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(8, 3)
        Label1.Name = "Label1"
        Label1.Size = New Size(129, 15)
        Label1.TabIndex = 5
        Label1.Text = "検出動画保存先フォルダ:"
        ' 
        ' TabDetect
        ' 
        TabDetect.Controls.Add(Label13)
        TabDetect.Controls.Add(GroupBox1)
        TabDetect.Controls.Add(Label12)
        TabDetect.Controls.Add(TrcHough)
        TabDetect.Controls.Add(TrcLineGap)
        TabDetect.Controls.Add(Label9)
        TabDetect.Controls.Add(LblHough)
        TabDetect.Controls.Add(Label8)
        TabDetect.Controls.Add(LblLineGap)
        TabDetect.Controls.Add(Label7)
        TabDetect.Controls.Add(LblAccSec)
        TabDetect.Controls.Add(TrcAccSec)
        TabDetect.Controls.Add(LblMinLength)
        TabDetect.Controls.Add(Label6)
        TabDetect.Controls.Add(Label5)
        TabDetect.Controls.Add(TrcMinLength)
        TabDetect.Controls.Add(TrcThreshold)
        TabDetect.Controls.Add(LblThreshold)
        TabDetect.Controls.Add(BtnTestSound)
        TabDetect.Controls.Add(Label2)
        TabDetect.Controls.Add(BtnBrowseSound)
        TabDetect.Controls.Add(TxtAlertSound)
        TabDetect.Location = New Point(4, 24)
        TabDetect.Name = "TabDetect"
        TabDetect.Padding = New Padding(3)
        TabDetect.Size = New Size(578, 410)
        TabDetect.TabIndex = 1
        TabDetect.Text = "検知設定"
        TabDetect.UseVisualStyleBackColor = True
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(LblClaheClip)
        GroupBox1.Controls.Add(TrcClaheClip)
        GroupBox1.Controls.Add(ChkEnableClahe)
        GroupBox1.Location = New Point(6, 261)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(322, 90)
        GroupBox1.TabIndex = 34
        GroupBox1.TabStop = False
        GroupBox1.Text = "     CLAHE（明暗差補正）を有効にする"
        ' 
        ' LblClaheClip
        ' 
        LblClaheClip.AutoSize = True
        LblClaheClip.Location = New Point(6, 18)
        LblClaheClip.Name = "LblClaheClip"
        LblClaheClip.Size = New Size(87, 15)
        LblClaheClip.TabIndex = 35
        LblClaheClip.Text = "設定値(Def:3.0)"
        ' 
        ' TrcClaheClip
        ' 
        TrcClaheClip.Location = New Point(117, 39)
        TrcClaheClip.Maximum = 50
        TrcClaheClip.Minimum = 10
        TrcClaheClip.Name = "TrcClaheClip"
        TrcClaheClip.Size = New Size(200, 45)
        TrcClaheClip.TabIndex = 34
        TrcClaheClip.TickFrequency = 5
        TrcClaheClip.Value = 30
        ' 
        ' ChkEnableClahe
        ' 
        ChkEnableClahe.AutoSize = True
        ChkEnableClahe.Checked = True
        ChkEnableClahe.CheckState = CheckState.Checked
        ChkEnableClahe.Location = New Point(6, 1)
        ChkEnableClahe.Name = "ChkEnableClahe"
        ChkEnableClahe.Size = New Size(15, 14)
        ChkEnableClahe.TabIndex = 33
        ChkEnableClahe.UseVisualStyleBackColor = True
        ' 
        ' Label12
        ' 
        Label12.AutoSize = True
        Label12.Location = New Point(329, 240)
        Label12.Name = "Label12"
        Label12.Size = New Size(202, 15)
        Label12.TabIndex = 30
        Label12.Text = "直線検出アルゴリズム「直線形状の確かさ"
        ' 
        ' TrcHough
        ' 
        TrcHough.Location = New Point(123, 210)
        TrcHough.Maximum = 50
        TrcHough.Minimum = 5
        TrcHough.Name = "TrcHough"
        TrcHough.Size = New Size(200, 45)
        TrcHough.TabIndex = 29
        TrcHough.Value = 25
        ' 
        ' TrcLineGap
        ' 
        TrcLineGap.Location = New Point(123, 159)
        TrcLineGap.Maximum = 30
        TrcLineGap.Minimum = 1
        TrcLineGap.Name = "TrcLineGap"
        TrcLineGap.Size = New Size(200, 45)
        TrcLineGap.TabIndex = 28
        TrcLineGap.Value = 5
        ' 
        ' Label9
        ' 
        Label9.AutoSize = True
        Label9.Location = New Point(329, 225)
        Label9.Name = "Label9"
        Label9.Size = New Size(175, 15)
        Label9.TabIndex = 27
        Label9.Text = "ハフ変換の厳しさ（低いほど敏感）"
        ' 
        ' LblHough
        ' 
        LblHough.AutoSize = True
        LblHough.Location = New Point(6, 225)
        LblHough.Name = "LblHough"
        LblHough.Size = New Size(105, 15)
        LblHough.TabIndex = 26
        LblHough.Text = "判定数 (Def:25): 25"
        ' 
        ' Label8
        ' 
        Label8.AutoSize = True
        Label8.Location = New Point(329, 173)
        Label8.Name = "Label8"
        Label8.Size = New Size(235, 15)
        Label8.TabIndex = 24
        Label8.Text = "途切れた直線を1本に繋ぎ合わせる許容ピクセル"
        ' 
        ' LblLineGap
        ' 
        LblLineGap.AutoSize = True
        LblLineGap.Location = New Point(6, 173)
        LblLineGap.Name = "LblLineGap"
        LblLineGap.Size = New Size(105, 15)
        LblLineGap.TabIndex = 23
        LblLineGap.Text = "連結許容 (Def:5): 5"
        ' 
        ' Label7
        ' 
        Label7.AutoSize = True
        Label7.Location = New Point(329, 123)
        Label7.Name = "Label7"
        Label7.Size = New Size(186, 15)
        Label7.TabIndex = 21
        Label7.Text = "比較明合成の蓄積時間(10 = 1.0秒)"
        ' 
        ' LblAccSec
        ' 
        LblAccSec.AutoSize = True
        LblAccSec.Location = New Point(6, 123)
        LblAccSec.Name = "LblAccSec"
        LblAccSec.Size = New Size(111, 15)
        LblAccSec.TabIndex = 20
        LblAccSec.Text = "蓄積秒 (Def:1.0): 1.0"
        ' 
        ' TrcAccSec
        ' 
        TrcAccSec.Location = New Point(123, 108)
        TrcAccSec.Maximum = 30
        TrcAccSec.Minimum = 1
        TrcAccSec.Name = "TrcAccSec"
        TrcAccSec.Size = New Size(200, 45)
        TrcAccSec.TabIndex = 19
        TrcAccSec.Value = 10
        ' 
        ' LblMinLength
        ' 
        LblMinLength.AutoSize = True
        LblMinLength.Location = New Point(8, 73)
        LblMinLength.Name = "LblMinLength"
        LblMinLength.Size = New Size(105, 15)
        LblMinLength.TabIndex = 18
        LblMinLength.Text = "最小長 (Def:20): 20"
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.Location = New Point(329, 73)
        Label6.Name = "Label6"
        Label6.Size = New Size(226, 15)
        Label6.TabIndex = 17
        Label6.Text = "流星の「直線の長さ」の最低条件（ピクセル）"
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Location = New Point(329, 20)
        Label5.Name = "Label5"
        Label5.Size = New Size(190, 15)
        Label5.TabIndex = 16
        Label5.Text = "動体検知の感度（低いほど高感度）"
        ' 
        ' TrcMinLength
        ' 
        TrcMinLength.Location = New Point(123, 57)
        TrcMinLength.Maximum = 100
        TrcMinLength.Minimum = 5
        TrcMinLength.Name = "TrcMinLength"
        TrcMinLength.Size = New Size(200, 45)
        TrcMinLength.TabIndex = 15
        TrcMinLength.TickFrequency = 5
        TrcMinLength.Value = 20
        ' 
        ' TrcThreshold
        ' 
        TrcThreshold.Location = New Point(123, 6)
        TrcThreshold.Maximum = 50
        TrcThreshold.Minimum = 5
        TrcThreshold.Name = "TrcThreshold"
        TrcThreshold.Size = New Size(200, 45)
        TrcThreshold.TabIndex = 14
        TrcThreshold.TickFrequency = 5
        TrcThreshold.Value = 15
        ' 
        ' LblThreshold
        ' 
        LblThreshold.AutoSize = True
        LblThreshold.Location = New Point(8, 20)
        LblThreshold.Name = "LblThreshold"
        LblThreshold.Size = New Size(93, 15)
        LblThreshold.TabIndex = 13
        LblThreshold.Text = "感度 (Def:15): 15"
        ' 
        ' BtnTestSound
        ' 
        BtnTestSound.Location = New Point(401, 381)
        BtnTestSound.Name = "BtnTestSound"
        BtnTestSound.Size = New Size(100, 25)
        BtnTestSound.TabIndex = 9
        BtnTestSound.Text = "テスト再生"
        BtnTestSound.UseVisualStyleBackColor = True
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(8, 363)
        Label2.Name = "Label2"
        Label2.Size = New Size(143, 15)
        Label2.TabIndex = 2
        Label2.Text = "検知時の音声ファイル(.wav)"
        ' 
        ' BtnBrowseSound
        ' 
        BtnBrowseSound.Location = New Point(302, 380)
        BtnBrowseSound.Name = "BtnBrowseSound"
        BtnBrowseSound.Size = New Size(93, 26)
        BtnBrowseSound.TabIndex = 1
        BtnBrowseSound.Text = "参照..."
        BtnBrowseSound.UseVisualStyleBackColor = True
        ' 
        ' TxtAlertSound
        ' 
        TxtAlertSound.Location = New Point(8, 381)
        TxtAlertSound.Name = "TxtAlertSound"
        TxtAlertSound.Size = New Size(288, 23)
        TxtAlertSound.TabIndex = 0
        ' 
        ' TabRecord
        ' 
        TabRecord.Controls.Add(Label11)
        TabRecord.Controls.Add(TrcPostRoll)
        TabRecord.Controls.Add(LblPostRoll)
        TabRecord.Controls.Add(LblPreRoll)
        TabRecord.Controls.Add(Label10)
        TabRecord.Controls.Add(TrcPreRoll)
        TabRecord.Location = New Point(4, 24)
        TabRecord.Name = "TabRecord"
        TabRecord.Size = New Size(578, 410)
        TabRecord.TabIndex = 2
        TabRecord.Text = "録画設定"
        TabRecord.UseVisualStyleBackColor = True
        ' 
        ' Label11
        ' 
        Label11.AutoSize = True
        Label11.Location = New Point(323, 67)
        Label11.Name = "Label11"
        Label11.Size = New Size(238, 15)
        Label11.TabIndex = 5
        Label11.Text = "流星が消えてから「余韻」として録画し続ける秒数"
        ' 
        ' TrcPostRoll
        ' 
        TrcPostRoll.Location = New Point(117, 54)
        TrcPostRoll.Minimum = 1
        TrcPostRoll.Name = "TrcPostRoll"
        TrcPostRoll.Size = New Size(200, 45)
        TrcPostRoll.TabIndex = 4
        TrcPostRoll.Value = 4
        ' 
        ' LblPostRoll
        ' 
        LblPostRoll.AutoSize = True
        LblPostRoll.Location = New Point(8, 67)
        LblPostRoll.Name = "LblPostRoll"
        LblPostRoll.Size = New Size(103, 15)
        LblPostRoll.TabIndex = 3
        LblPostRoll.Text = "録画後 (Def:4s): 4s"
        ' 
        ' LblPreRoll
        ' 
        LblPreRoll.AutoSize = True
        LblPreRoll.Location = New Point(8, 17)
        LblPreRoll.Name = "LblPreRoll"
        LblPreRoll.Size = New Size(103, 15)
        LblPreRoll.TabIndex = 2
        LblPreRoll.Text = "録画前 (Def:3s): 3s"
        ' 
        ' Label10
        ' 
        Label10.AutoSize = True
        Label10.Location = New Point(323, 17)
        Label10.Name = "Label10"
        Label10.Size = New Size(216, 15)
        Label10.TabIndex = 1
        Label10.Text = "流星検知から「過去」へ遡って録画する秒数"
        ' 
        ' TrcPreRoll
        ' 
        TrcPreRoll.Location = New Point(117, 3)
        TrcPreRoll.Minimum = 1
        TrcPreRoll.Name = "TrcPreRoll"
        TrcPreRoll.Size = New Size(200, 45)
        TrcPreRoll.TabIndex = 0
        TrcPreRoll.Value = 3
        ' 
        ' TabSNS
        ' 
        TabSNS.Location = New Point(4, 24)
        TabSNS.Name = "TabSNS"
        TabSNS.Padding = New Padding(3)
        TabSNS.Size = New Size(578, 410)
        TabSNS.TabIndex = 3
        TabSNS.Text = "SNS設定"
        TabSNS.UseVisualStyleBackColor = True
        ' 
        ' BtnCancel
        ' 
        BtnCancel.Location = New Point(376, 444)
        BtnCancel.Name = "BtnCancel"
        BtnCancel.Size = New Size(100, 30)
        BtnCancel.TabIndex = 11
        BtnCancel.Text = "キャンセル"
        BtnCancel.UseVisualStyleBackColor = True
        ' 
        ' BtnOK
        ' 
        BtnOK.Location = New Point(482, 444)
        BtnOK.Name = "BtnOK"
        BtnOK.Size = New Size(100, 30)
        BtnOK.TabIndex = 10
        BtnOK.Text = "OK"
        BtnOK.UseVisualStyleBackColor = True
        ' 
        ' Label13
        ' 
        Label13.Location = New Point(334, 288)
        Label13.Name = "Label13"
        Label13.Size = New Size(230, 48)
        Label13.TabIndex = 35
        Label13.Text = "空の明るさ（光害・月明かり）を自動補正し、星や淡い流星をくっきりと浮き上がらせます"
        ' 
        ' FrmSettings
        ' 
        AutoScaleDimensions = New SizeF(96F, 96F)
        AutoScaleMode = AutoScaleMode.Dpi
        ClientSize = New Size(586, 486)
        Controls.Add(BtnCancel)
        Controls.Add(BtnOK)
        Controls.Add(TabControl)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "FrmSettings"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "システム設定"
        TabControl.ResumeLayout(False)
        TabSave.ResumeLayout(False)
        TabSave.PerformLayout()
        TabDetect.ResumeLayout(False)
        TabDetect.PerformLayout()
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        CType(TrcClaheClip, ComponentModel.ISupportInitialize).EndInit()
        CType(TrcHough, ComponentModel.ISupportInitialize).EndInit()
        CType(TrcLineGap, ComponentModel.ISupportInitialize).EndInit()
        CType(TrcAccSec, ComponentModel.ISupportInitialize).EndInit()
        CType(TrcMinLength, ComponentModel.ISupportInitialize).EndInit()
        CType(TrcThreshold, ComponentModel.ISupportInitialize).EndInit()
        TabRecord.ResumeLayout(False)
        TabRecord.PerformLayout()
        CType(TrcPostRoll, ComponentModel.ISupportInitialize).EndInit()
        CType(TrcPreRoll, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub
    Friend WithEvents TabControl As TabControl
    Friend WithEvents TabSave As TabPage
    Friend WithEvents TabDetect As TabPage
    Friend WithEvents TabRecord As TabPage
    Friend WithEvents BtnBrowse As Button
    Friend WithEvents TxtSavePath As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents BtnCancel As Button
    Friend WithEvents BtnOK As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents BtnBrowseSound As Button
    Friend WithEvents TxtAlertSound As TextBox
    Friend WithEvents BtnTestSound As Button
    Friend WithEvents BtnSelectContinuousDir As Button
    Friend WithEvents TxtContinuousDir As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents LblThreshold As Label
    Friend WithEvents TrcThreshold As TrackBar
    Friend WithEvents Label5 As Label
    Friend WithEvents TrcMinLength As TrackBar
    Friend WithEvents LblAccSec As Label
    Friend WithEvents TrcAccSec As TrackBar
    Friend WithEvents LblMinLength As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents LblLineGap As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents LblHough As Label
    Friend WithEvents TrcPreRoll As TrackBar
    Friend WithEvents Label11 As Label
    Friend WithEvents TrcPostRoll As TrackBar
    Friend WithEvents LblPostRoll As Label
    Friend WithEvents LblPreRoll As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents TrcHough As TrackBar
    Friend WithEvents TrcLineGap As TrackBar
    Friend WithEvents Label12 As Label
    Friend WithEvents TabSNS As TabPage
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents LblClaheClip As Label
    Friend WithEvents TrcClaheClip As TrackBar
    Friend WithEvents ChkEnableClahe As CheckBox
    Friend WithEvents Label13 As Label
End Class
