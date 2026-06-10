<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmMain
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
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMain))
        PicMain = New PictureBox()
        BtnStart = New Button()
        BtnStop = New Button()
        GrpSource = New GroupBox()
        RdoRtsp = New RadioButton()
        RdoVideo = New RadioButton()
        RdoZwo = New RadioButton()
        RdoUsb = New RadioButton()
        BtnRefreshUsb = New Button()
        CmbUsbDeviceList = New ComboBox()
        PnlSettingsBase = New Panel()
        PnlVideoSettings = New Panel()
        TxtFileName = New TextBox()
        BtnOpenFile = New Button()
        PnlZwoSettings = New Panel()
        Label2 = New Label()
        CmbRoiSelect = New ComboBox()
        BtnZwoSettings = New Button()
        GrpWB = New GroupBox()
        Lbl_B = New Label()
        Lbl_R = New Label()
        LblWb = New Label()
        ChkWBAuto = New CheckBox()
        TrkbWbB = New TrackBar()
        TrkbWbR = New TrackBar()
        GrpExp = New GroupBox()
        LblExp = New Label()
        ChkExpAuto = New CheckBox()
        TrkbExp = New TrackBar()
        GrpGain = New GroupBox()
        LblGain = New Label()
        ChkGainAuto = New CheckBox()
        TrkbGain = New TrackBar()
        CmbBinning = New ComboBox()
        LblBinning = New Label()
        BtnConnect = New Button()
        BtnRefreshZwo = New Button()
        CmbZwoDeviceList = New ComboBox()
        PnlUsbSettings = New Panel()
        LblUsbResolution = New Label()
        CmbUsbResolution = New ComboBox()
        StsMain = New StatusStrip()
        LblStatusMessage = New ToolStripStatusLabel()
        LblStatusResolution = New ToolStripStatusLabel()
        LblStatusFps = New ToolStripStatusLabel()
        LblStatusTemp = New ToolStripStatusLabel()
        LstLogs = New ListBox()
        ChkShowSub = New CheckBox()
        BtnSnapshot = New Button()
        BtnSettings = New Button()
        ChkDualRecord = New CheckBox()
        BtnEditMaskDirect = New Button()
        ChkMuteSound = New CheckBox()
        TmrUIUpdate = New Timer(components)
        TrkbDisplayAlpha = New TrackBar()
        TrkbDisplayBeta = New TrackBar()
        LblDisplayAlpha = New Label()
        LblDisplayBeta = New Label()
        ChkContinuousRecord = New CheckBox()
        TrcGamma = New TrackBar()
        LblGamma = New Label()
        ChkEnableDetect = New CheckBox()
        PnlRtspSettings = New Panel()
        Label1 = New Label()
        TxtRtspUrl = New TextBox()
        ChkShowTimestamp = New CheckBox()
        CType(PicMain, ComponentModel.ISupportInitialize).BeginInit()
        GrpSource.SuspendLayout()
        PnlVideoSettings.SuspendLayout()
        PnlZwoSettings.SuspendLayout()
        GrpWB.SuspendLayout()
        CType(TrkbWbB, ComponentModel.ISupportInitialize).BeginInit()
        CType(TrkbWbR, ComponentModel.ISupportInitialize).BeginInit()
        GrpExp.SuspendLayout()
        CType(TrkbExp, ComponentModel.ISupportInitialize).BeginInit()
        GrpGain.SuspendLayout()
        CType(TrkbGain, ComponentModel.ISupportInitialize).BeginInit()
        PnlUsbSettings.SuspendLayout()
        StsMain.SuspendLayout()
        CType(TrkbDisplayAlpha, ComponentModel.ISupportInitialize).BeginInit()
        CType(TrkbDisplayBeta, ComponentModel.ISupportInitialize).BeginInit()
        CType(TrcGamma, ComponentModel.ISupportInitialize).BeginInit()
        PnlRtspSettings.SuspendLayout()
        SuspendLayout()
        ' 
        ' PicMain
        ' 
        PicMain.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        PicMain.BackColor = Color.Black
        PicMain.BorderStyle = BorderStyle.FixedSingle
        PicMain.Location = New Point(11, 11)
        PicMain.Margin = New Padding(2)
        PicMain.Name = "PicMain"
        PicMain.Size = New Size(960, 560)
        PicMain.SizeMode = PictureBoxSizeMode.Zoom
        PicMain.TabIndex = 0
        PicMain.TabStop = False
        ' 
        ' BtnStart
        ' 
        BtnStart.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        BtnStart.Location = New Point(975, 724)
        BtnStart.Margin = New Padding(2)
        BtnStart.Name = "BtnStart"
        BtnStart.Size = New Size(120, 34)
        BtnStart.TabIndex = 2
        BtnStart.Text = "開始"
        BtnStart.UseVisualStyleBackColor = True
        ' 
        ' BtnStop
        ' 
        BtnStop.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        BtnStop.Location = New Point(1133, 724)
        BtnStop.Margin = New Padding(2)
        BtnStop.Name = "BtnStop"
        BtnStop.Size = New Size(120, 34)
        BtnStop.TabIndex = 3
        BtnStop.Text = "停止"
        BtnStop.UseVisualStyleBackColor = True
        ' 
        ' GrpSource
        ' 
        GrpSource.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        GrpSource.Controls.Add(RdoRtsp)
        GrpSource.Controls.Add(RdoVideo)
        GrpSource.Controls.Add(RdoZwo)
        GrpSource.Controls.Add(RdoUsb)
        GrpSource.Location = New Point(975, 11)
        GrpSource.Margin = New Padding(2)
        GrpSource.Name = "GrpSource"
        GrpSource.Padding = New Padding(2)
        GrpSource.Size = New Size(278, 116)
        GrpSource.TabIndex = 4
        GrpSource.TabStop = False
        GrpSource.Text = "映像ソース"
        ' 
        ' RdoRtsp
        ' 
        RdoRtsp.AutoSize = True
        RdoRtsp.Location = New Point(4, 66)
        RdoRtsp.Margin = New Padding(2)
        RdoRtsp.Name = "RdoRtsp"
        RdoRtsp.Size = New Size(122, 19)
        RdoRtsp.TabIndex = 3
        RdoRtsp.Text = "RTSP (ATOM Cam)"
        RdoRtsp.UseVisualStyleBackColor = True
        ' 
        ' RdoVideo
        ' 
        RdoVideo.AutoSize = True
        RdoVideo.Location = New Point(4, 89)
        RdoVideo.Margin = New Padding(2)
        RdoVideo.Name = "RdoVideo"
        RdoVideo.Size = New Size(107, 19)
        RdoVideo.TabIndex = 2
        RdoVideo.Text = "動画ファイル再生"
        RdoVideo.UseVisualStyleBackColor = True
        ' 
        ' RdoZwo
        ' 
        RdoZwo.AutoSize = True
        RdoZwo.Location = New Point(4, 43)
        RdoZwo.Margin = New Padding(2)
        RdoZwo.Name = "RdoZwo"
        RdoZwo.Size = New Size(97, 19)
        RdoZwo.TabIndex = 1
        RdoZwo.Text = "ZWO ASIカメラ"
        RdoZwo.UseVisualStyleBackColor = True
        ' 
        ' RdoUsb
        ' 
        RdoUsb.AutoSize = True
        RdoUsb.Checked = True
        RdoUsb.Location = New Point(4, 20)
        RdoUsb.Margin = New Padding(2)
        RdoUsb.Name = "RdoUsb"
        RdoUsb.Size = New Size(188, 19)
        RdoUsb.TabIndex = 0
        RdoUsb.TabStop = True
        RdoUsb.Text = "USBストリーミング / HDMIキャプチャ"
        RdoUsb.UseVisualStyleBackColor = True
        ' 
        ' BtnRefreshUsb
        ' 
        BtnRefreshUsb.Location = New Point(216, 2)
        BtnRefreshUsb.Margin = New Padding(2)
        BtnRefreshUsb.Name = "BtnRefreshUsb"
        BtnRefreshUsb.Size = New Size(60, 23)
        BtnRefreshUsb.TabIndex = 9
        BtnRefreshUsb.Text = "更新"
        BtnRefreshUsb.UseVisualStyleBackColor = True
        ' 
        ' CmbUsbDeviceList
        ' 
        CmbUsbDeviceList.DropDownStyle = ComboBoxStyle.DropDownList
        CmbUsbDeviceList.FormattingEnabled = True
        CmbUsbDeviceList.Location = New Point(2, 2)
        CmbUsbDeviceList.Margin = New Padding(2)
        CmbUsbDeviceList.Name = "CmbUsbDeviceList"
        CmbUsbDeviceList.Size = New Size(210, 23)
        CmbUsbDeviceList.TabIndex = 2
        ' 
        ' PnlSettingsBase
        ' 
        PnlSettingsBase.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        PnlSettingsBase.Location = New Point(975, 131)
        PnlSettingsBase.Margin = New Padding(2)
        PnlSettingsBase.Name = "PnlSettingsBase"
        PnlSettingsBase.Size = New Size(278, 400)
        PnlSettingsBase.TabIndex = 5
        ' 
        ' PnlVideoSettings
        ' 
        PnlVideoSettings.Controls.Add(TxtFileName)
        PnlVideoSettings.Controls.Add(BtnOpenFile)
        PnlVideoSettings.Location = New Point(1346, 563)
        PnlVideoSettings.Margin = New Padding(2)
        PnlVideoSettings.Name = "PnlVideoSettings"
        PnlVideoSettings.Size = New Size(278, 86)
        PnlVideoSettings.TabIndex = 6
        ' 
        ' TxtFileName
        ' 
        TxtFileName.BackColor = SystemColors.Window
        TxtFileName.Location = New Point(2, 2)
        TxtFileName.Name = "TxtFileName"
        TxtFileName.PlaceholderText = "動画ファイルを選択してください..."
        TxtFileName.ReadOnly = True
        TxtFileName.Size = New Size(274, 23)
        TxtFileName.TabIndex = 1
        ' 
        ' BtnOpenFile
        ' 
        BtnOpenFile.Location = New Point(195, 31)
        BtnOpenFile.Name = "BtnOpenFile"
        BtnOpenFile.Size = New Size(80, 29)
        BtnOpenFile.TabIndex = 0
        BtnOpenFile.Text = "動画を開く"
        BtnOpenFile.UseVisualStyleBackColor = True
        ' 
        ' PnlZwoSettings
        ' 
        PnlZwoSettings.Controls.Add(Label2)
        PnlZwoSettings.Controls.Add(CmbRoiSelect)
        PnlZwoSettings.Controls.Add(BtnZwoSettings)
        PnlZwoSettings.Controls.Add(GrpWB)
        PnlZwoSettings.Controls.Add(GrpExp)
        PnlZwoSettings.Controls.Add(GrpGain)
        PnlZwoSettings.Controls.Add(CmbBinning)
        PnlZwoSettings.Controls.Add(LblBinning)
        PnlZwoSettings.Controls.Add(BtnConnect)
        PnlZwoSettings.Controls.Add(BtnRefreshZwo)
        PnlZwoSettings.Controls.Add(CmbZwoDeviceList)
        PnlZwoSettings.Location = New Point(1343, 129)
        PnlZwoSettings.Margin = New Padding(2)
        PnlZwoSettings.Name = "PnlZwoSettings"
        PnlZwoSettings.Size = New Size(278, 400)
        PnlZwoSettings.TabIndex = 6
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(4, 62)
        Label2.Name = "Label2"
        Label2.Size = New Size(26, 15)
        Label2.TabIndex = 30
        Label2.Text = "ROI"
        ' 
        ' CmbRoiSelect
        ' 
        CmbRoiSelect.FormattingEnabled = True
        CmbRoiSelect.Items.AddRange(New Object() {"0: 全画面 (100%)", "1: 中央 95% 切り出し", "2: 中央 90% 切り出し", "3: 中央 85% 切り出し", "4: 中央 80% 切り出し"})
        CmbRoiSelect.Location = New Point(51, 59)
        CmbRoiSelect.Name = "CmbRoiSelect"
        CmbRoiSelect.Size = New Size(223, 23)
        CmbRoiSelect.TabIndex = 29
        ' 
        ' BtnZwoSettings
        ' 
        BtnZwoSettings.Location = New Point(150, 30)
        BtnZwoSettings.Name = "BtnZwoSettings"
        BtnZwoSettings.Size = New Size(125, 23)
        BtnZwoSettings.TabIndex = 28
        BtnZwoSettings.Text = "ZWO詳細設定..."
        BtnZwoSettings.UseVisualStyleBackColor = True
        ' 
        ' GrpWB
        ' 
        GrpWB.Controls.Add(Lbl_B)
        GrpWB.Controls.Add(Lbl_R)
        GrpWB.Controls.Add(LblWb)
        GrpWB.Controls.Add(ChkWBAuto)
        GrpWB.Controls.Add(TrkbWbB)
        GrpWB.Controls.Add(TrkbWbR)
        GrpWB.Location = New Point(3, 269)
        GrpWB.Margin = New Padding(2)
        GrpWB.Name = "GrpWB"
        GrpWB.Padding = New Padding(2)
        GrpWB.Size = New Size(273, 129)
        GrpWB.TabIndex = 27
        GrpWB.TabStop = False
        GrpWB.Text = "ホワイトバランス"
        ' 
        ' Lbl_B
        ' 
        Lbl_B.AutoSize = True
        Lbl_B.Location = New Point(13, 83)
        Lbl_B.Margin = New Padding(2, 0, 2, 0)
        Lbl_B.Name = "Lbl_B"
        Lbl_B.Size = New Size(14, 15)
        Lbl_B.TabIndex = 17
        Lbl_B.Text = "B"
        ' 
        ' Lbl_R
        ' 
        Lbl_R.AutoSize = True
        Lbl_R.Location = New Point(13, 44)
        Lbl_R.Margin = New Padding(2, 0, 2, 0)
        Lbl_R.Name = "Lbl_R"
        Lbl_R.Size = New Size(14, 15)
        Lbl_R.TabIndex = 16
        Lbl_R.Text = "R"
        ' 
        ' LblWb
        ' 
        LblWb.AutoSize = True
        LblWb.Location = New Point(110, 18)
        LblWb.Margin = New Padding(2, 0, 2, 0)
        LblWb.Name = "LblWb"
        LblWb.Size = New Size(50, 15)
        LblWb.TabIndex = 15
        LblWb.Text = "WB(R:B)"
        ' 
        ' ChkWBAuto
        ' 
        ChkWBAuto.AutoSize = True
        ChkWBAuto.Location = New Point(9, 20)
        ChkWBAuto.Margin = New Padding(2)
        ChkWBAuto.Name = "ChkWBAuto"
        ChkWBAuto.Size = New Size(51, 19)
        ChkWBAuto.TabIndex = 14
        ChkWBAuto.Text = "オート"
        ChkWBAuto.UseVisualStyleBackColor = True
        ' 
        ' TrkbWbB
        ' 
        TrkbWbB.Location = New Point(32, 78)
        TrkbWbB.Margin = New Padding(2)
        TrkbWbB.Name = "TrkbWbB"
        TrkbWbB.Size = New Size(236, 45)
        TrkbWbB.TabIndex = 13
        ' 
        ' TrkbWbR
        ' 
        TrkbWbR.Location = New Point(31, 39)
        TrkbWbR.Margin = New Padding(2)
        TrkbWbR.Name = "TrkbWbR"
        TrkbWbR.Size = New Size(237, 45)
        TrkbWbR.TabIndex = 12
        ' 
        ' GrpExp
        ' 
        GrpExp.Controls.Add(LblExp)
        GrpExp.Controls.Add(ChkExpAuto)
        GrpExp.Controls.Add(TrkbExp)
        GrpExp.Location = New Point(3, 178)
        GrpExp.Margin = New Padding(2)
        GrpExp.Name = "GrpExp"
        GrpExp.Padding = New Padding(2)
        GrpExp.Size = New Size(273, 87)
        GrpExp.TabIndex = 26
        GrpExp.TabStop = False
        GrpExp.Text = "露出"
        ' 
        ' LblExp
        ' 
        LblExp.AutoSize = True
        LblExp.Location = New Point(130, 18)
        LblExp.Margin = New Padding(2, 0, 2, 0)
        LblExp.Name = "LblExp"
        LblExp.Size = New Size(26, 15)
        LblExp.TabIndex = 23
        LblExp.Text = "Exp"
        ' 
        ' ChkExpAuto
        ' 
        ChkExpAuto.AutoSize = True
        ChkExpAuto.Location = New Point(10, 17)
        ChkExpAuto.Margin = New Padding(2)
        ChkExpAuto.Name = "ChkExpAuto"
        ChkExpAuto.Size = New Size(51, 19)
        ChkExpAuto.TabIndex = 22
        ChkExpAuto.Text = "オート"
        ChkExpAuto.UseVisualStyleBackColor = True
        ' 
        ' TrkbExp
        ' 
        TrkbExp.Location = New Point(4, 40)
        TrkbExp.Margin = New Padding(2)
        TrkbExp.Name = "TrkbExp"
        TrkbExp.Size = New Size(265, 45)
        TrkbExp.TabIndex = 7
        ' 
        ' GrpGain
        ' 
        GrpGain.Controls.Add(LblGain)
        GrpGain.Controls.Add(ChkGainAuto)
        GrpGain.Controls.Add(TrkbGain)
        GrpGain.Location = New Point(2, 87)
        GrpGain.Margin = New Padding(2)
        GrpGain.Name = "GrpGain"
        GrpGain.Padding = New Padding(2)
        GrpGain.Size = New Size(273, 87)
        GrpGain.TabIndex = 25
        GrpGain.TabStop = False
        GrpGain.Text = "Gain"
        ' 
        ' LblGain
        ' 
        LblGain.AutoSize = True
        LblGain.Location = New Point(130, 18)
        LblGain.Margin = New Padding(2, 0, 2, 0)
        LblGain.Name = "LblGain"
        LblGain.Size = New Size(31, 15)
        LblGain.TabIndex = 22
        LblGain.Text = "Gain"
        ' 
        ' ChkGainAuto
        ' 
        ChkGainAuto.AutoSize = True
        ChkGainAuto.Location = New Point(10, 17)
        ChkGainAuto.Margin = New Padding(2)
        ChkGainAuto.Name = "ChkGainAuto"
        ChkGainAuto.Size = New Size(51, 19)
        ChkGainAuto.TabIndex = 21
        ChkGainAuto.Text = "オート"
        ChkGainAuto.UseVisualStyleBackColor = True
        ' 
        ' TrkbGain
        ' 
        TrkbGain.Location = New Point(4, 40)
        TrkbGain.Margin = New Padding(2)
        TrkbGain.Name = "TrkbGain"
        TrkbGain.Size = New Size(265, 45)
        TrkbGain.TabIndex = 6
        ' 
        ' CmbBinning
        ' 
        CmbBinning.DropDownStyle = ComboBoxStyle.DropDownList
        CmbBinning.FormattingEnabled = True
        CmbBinning.Location = New Point(50, 30)
        CmbBinning.Name = "CmbBinning"
        CmbBinning.Size = New Size(96, 23)
        CmbBinning.TabIndex = 14
        ' 
        ' LblBinning
        ' 
        LblBinning.AutoSize = True
        LblBinning.Location = New Point(3, 33)
        LblBinning.Name = "LblBinning"
        LblBinning.Size = New Size(41, 15)
        LblBinning.TabIndex = 13
        LblBinning.Text = "ビニング"
        ' 
        ' BtnConnect
        ' 
        BtnConnect.Location = New Point(215, 2)
        BtnConnect.Name = "BtnConnect"
        BtnConnect.Size = New Size(60, 23)
        BtnConnect.TabIndex = 12
        BtnConnect.Text = "接続"
        BtnConnect.UseVisualStyleBackColor = True
        ' 
        ' BtnRefreshZwo
        ' 
        BtnRefreshZwo.Location = New Point(150, 2)
        BtnRefreshZwo.Margin = New Padding(2)
        BtnRefreshZwo.Name = "BtnRefreshZwo"
        BtnRefreshZwo.Size = New Size(60, 23)
        BtnRefreshZwo.TabIndex = 11
        BtnRefreshZwo.Text = "更新"
        BtnRefreshZwo.UseVisualStyleBackColor = True
        ' 
        ' CmbZwoDeviceList
        ' 
        CmbZwoDeviceList.DropDownStyle = ComboBoxStyle.DropDownList
        CmbZwoDeviceList.FormattingEnabled = True
        CmbZwoDeviceList.Location = New Point(2, 2)
        CmbZwoDeviceList.Margin = New Padding(2)
        CmbZwoDeviceList.Name = "CmbZwoDeviceList"
        CmbZwoDeviceList.Size = New Size(144, 23)
        CmbZwoDeviceList.TabIndex = 10
        ' 
        ' PnlUsbSettings
        ' 
        PnlUsbSettings.Controls.Add(LblUsbResolution)
        PnlUsbSettings.Controls.Add(CmbUsbResolution)
        PnlUsbSettings.Controls.Add(BtnRefreshUsb)
        PnlUsbSettings.Controls.Add(CmbUsbDeviceList)
        PnlUsbSettings.Location = New Point(0, 60)
        PnlUsbSettings.Margin = New Padding(2)
        PnlUsbSettings.Name = "PnlUsbSettings"
        PnlUsbSettings.Size = New Size(278, 86)
        PnlUsbSettings.TabIndex = 6
        ' 
        ' LblUsbResolution
        ' 
        LblUsbResolution.AutoSize = True
        LblUsbResolution.Location = New Point(2, 36)
        LblUsbResolution.Margin = New Padding(2, 0, 2, 0)
        LblUsbResolution.Name = "LblUsbResolution"
        LblUsbResolution.Size = New Size(226, 15)
        LblUsbResolution.TabIndex = 13
        LblUsbResolution.Text = "解像度:(HDMIキャプチャ経由の時のみ有効）"
        ' 
        ' CmbUsbResolution
        ' 
        CmbUsbResolution.DropDownStyle = ComboBoxStyle.DropDownList
        CmbUsbResolution.FormattingEnabled = True
        CmbUsbResolution.Items.AddRange(New Object() {"3840 x 2160", "1920 x 1080", "1280 x   720", "  640 x   480"})
        CmbUsbResolution.Location = New Point(2, 53)
        CmbUsbResolution.Margin = New Padding(2)
        CmbUsbResolution.Name = "CmbUsbResolution"
        CmbUsbResolution.Size = New Size(210, 23)
        CmbUsbResolution.TabIndex = 10
        ' 
        ' StsMain
        ' 
        StsMain.ImageScalingSize = New Size(24, 24)
        StsMain.Items.AddRange(New ToolStripItem() {LblStatusMessage, LblStatusResolution, LblStatusFps, LblStatusTemp})
        StsMain.Location = New Point(0, 763)
        StsMain.Name = "StsMain"
        StsMain.Padding = New Padding(1, 0, 9, 0)
        StsMain.Size = New Size(1264, 22)
        StsMain.TabIndex = 6
        StsMain.Text = "StatusStrip1"
        ' 
        ' LblStatusMessage
        ' 
        LblStatusMessage.Name = "LblStatusMessage"
        LblStatusMessage.Size = New Size(55, 17)
        LblStatusMessage.Text = "準備完了"
        ' 
        ' LblStatusResolution
        ' 
        LblStatusResolution.Name = "LblStatusResolution"
        LblStatusResolution.Size = New Size(64, 17)
        LblStatusResolution.Text = "解像度: ---"
        ' 
        ' LblStatusFps
        ' 
        LblStatusFps.Name = "LblStatusFps"
        LblStatusFps.Size = New Size(47, 17)
        LblStatusFps.Text = "FPS: 0.0"
        ' 
        ' LblStatusTemp
        ' 
        LblStatusTemp.Name = "LblStatusTemp"
        LblStatusTemp.Size = New Size(52, 17)
        LblStatusTemp.Text = "温度: ---"
        ' 
        ' LstLogs
        ' 
        LstLogs.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        LstLogs.BackColor = Color.Black
        LstLogs.Font = New Font("ＭＳ ゴシック", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(128))
        LstLogs.ForeColor = Color.LimeGreen
        LstLogs.FormattingEnabled = True
        LstLogs.HorizontalScrollbar = True
        LstLogs.IntegralHeight = False
        LstLogs.Location = New Point(11, 575)
        LstLogs.Margin = New Padding(2)
        LstLogs.Name = "LstLogs"
        LstLogs.Size = New Size(600, 183)
        LstLogs.TabIndex = 7
        ' 
        ' ChkShowSub
        ' 
        ChkShowSub.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        ChkShowSub.AutoSize = True
        ChkShowSub.Location = New Point(615, 585)
        ChkShowSub.Margin = New Padding(2)
        ChkShowSub.Name = "ChkShowSub"
        ChkShowSub.Size = New Size(213, 19)
        ChkShowSub.TabIndex = 8
        ChkShowSub.Text = "解析モニタ表示 (リアルタイム検知ビュー)"
        ChkShowSub.UseVisualStyleBackColor = True
        ' 
        ' BtnSnapshot
        ' 
        BtnSnapshot.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        BtnSnapshot.Location = New Point(870, 576)
        BtnSnapshot.Name = "BtnSnapshot"
        BtnSnapshot.Size = New Size(100, 34)
        BtnSnapshot.TabIndex = 9
        BtnSnapshot.Text = "スナップショット"
        BtnSnapshot.UseVisualStyleBackColor = True
        ' 
        ' BtnSettings
        ' 
        BtnSettings.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        BtnSettings.Location = New Point(871, 656)
        BtnSettings.Name = "BtnSettings"
        BtnSettings.Size = New Size(100, 34)
        BtnSettings.TabIndex = 10
        BtnSettings.Text = "⚙ 設定"
        BtnSettings.UseVisualStyleBackColor = True
        ' 
        ' ChkDualRecord
        ' 
        ChkDualRecord.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        ChkDualRecord.AutoSize = True
        ChkDualRecord.Location = New Point(615, 607)
        ChkDualRecord.Margin = New Padding(2)
        ChkDualRecord.Name = "ChkDualRecord"
        ChkDualRecord.Size = New Size(229, 19)
        ChkDualRecord.TabIndex = 11
        ChkDualRecord.Text = "枠付き動画も同時に保存する (PC負荷増)"
        ChkDualRecord.UseVisualStyleBackColor = True
        ' 
        ' BtnEditMaskDirect
        ' 
        BtnEditMaskDirect.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        BtnEditMaskDirect.Location = New Point(870, 616)
        BtnEditMaskDirect.Name = "BtnEditMaskDirect"
        BtnEditMaskDirect.Size = New Size(100, 34)
        BtnEditMaskDirect.TabIndex = 12
        BtnEditMaskDirect.Text = "マスク領域の編集"
        BtnEditMaskDirect.UseVisualStyleBackColor = True
        ' 
        ' ChkMuteSound
        ' 
        ChkMuteSound.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        ChkMuteSound.AutoSize = True
        ChkMuteSound.Location = New Point(615, 653)
        ChkMuteSound.Margin = New Padding(2)
        ChkMuteSound.Name = "ChkMuteSound"
        ChkMuteSound.Size = New Size(121, 19)
        ChkMuteSound.TabIndex = 13
        ChkMuteSound.Text = "検知音をミュートする"
        ChkMuteSound.UseVisualStyleBackColor = True
        ' 
        ' TmrUIUpdate
        ' 
        TmrUIUpdate.Interval = 1000
        ' 
        ' TrkbDisplayAlpha
        ' 
        TrkbDisplayAlpha.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        TrkbDisplayAlpha.Location = New Point(975, 550)
        TrkbDisplayAlpha.Margin = New Padding(2)
        TrkbDisplayAlpha.Maximum = 50
        TrkbDisplayAlpha.Minimum = 10
        TrkbDisplayAlpha.Name = "TrkbDisplayAlpha"
        TrkbDisplayAlpha.Size = New Size(278, 45)
        TrkbDisplayAlpha.TabIndex = 14
        TrkbDisplayAlpha.Value = 10
        ' 
        ' TrkbDisplayBeta
        ' 
        TrkbDisplayBeta.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        TrkbDisplayBeta.Location = New Point(975, 614)
        TrkbDisplayBeta.Margin = New Padding(2)
        TrkbDisplayBeta.Maximum = 100
        TrkbDisplayBeta.Name = "TrkbDisplayBeta"
        TrkbDisplayBeta.Size = New Size(278, 45)
        TrkbDisplayBeta.TabIndex = 15
        ' 
        ' LblDisplayAlpha
        ' 
        LblDisplayAlpha.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        LblDisplayAlpha.AutoSize = True
        LblDisplayAlpha.Location = New Point(975, 533)
        LblDisplayAlpha.Margin = New Padding(2, 0, 2, 0)
        LblDisplayAlpha.Name = "LblDisplayAlpha"
        LblDisplayAlpha.Size = New Size(102, 15)
        LblDisplayAlpha.TabIndex = 16
        LblDisplayAlpha.Text = "表示コントラスト: 1.0"
        ' 
        ' LblDisplayBeta
        ' 
        LblDisplayBeta.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        LblDisplayBeta.AutoSize = True
        LblDisplayBeta.Location = New Point(975, 597)
        LblDisplayBeta.Margin = New Padding(2, 0, 2, 0)
        LblDisplayBeta.Name = "LblDisplayBeta"
        LblDisplayBeta.Size = New Size(72, 15)
        LblDisplayBeta.TabIndex = 17
        LblDisplayBeta.Text = "表示明るさ: 0"
        ' 
        ' ChkContinuousRecord
        ' 
        ChkContinuousRecord.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        ChkContinuousRecord.AutoSize = True
        ChkContinuousRecord.Location = New Point(615, 630)
        ChkContinuousRecord.Margin = New Padding(2)
        ChkContinuousRecord.Name = "ChkContinuousRecord"
        ChkContinuousRecord.Size = New Size(135, 19)
        ChkContinuousRecord.TabIndex = 18
        ChkContinuousRecord.Text = "常時録画を有効にする"
        ChkContinuousRecord.UseVisualStyleBackColor = True
        ' 
        ' TrcGamma
        ' 
        TrcGamma.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        TrcGamma.Location = New Point(975, 678)
        TrcGamma.Margin = New Padding(2)
        TrcGamma.Maximum = 30
        TrcGamma.Minimum = 10
        TrcGamma.Name = "TrcGamma"
        TrcGamma.Size = New Size(278, 45)
        TrcGamma.TabIndex = 19
        TrcGamma.Value = 12
        ' 
        ' LblGamma
        ' 
        LblGamma.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        LblGamma.AutoSize = True
        LblGamma.Location = New Point(975, 661)
        LblGamma.Margin = New Padding(2, 0, 2, 0)
        LblGamma.Name = "LblGamma"
        LblGamma.Size = New Size(68, 15)
        LblGamma.TabIndex = 20
        LblGamma.Text = "Gamma: 1.2"
        ' 
        ' ChkEnableDetect
        ' 
        ChkEnableDetect.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        ChkEnableDetect.AutoSize = True
        ChkEnableDetect.Checked = True
        ChkEnableDetect.CheckState = CheckState.Checked
        ChkEnableDetect.Font = New Font("Yu Gothic UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(128))
        ChkEnableDetect.Location = New Point(615, 704)
        ChkEnableDetect.Margin = New Padding(2)
        ChkEnableDetect.Name = "ChkEnableDetect"
        ChkEnableDetect.Size = New Size(167, 24)
        ChkEnableDetect.TabIndex = 21
        ChkEnableDetect.Text = "流星検知を有効にする"
        ChkEnableDetect.UseVisualStyleBackColor = True
        ' 
        ' PnlRtspSettings
        ' 
        PnlRtspSettings.Controls.Add(Label1)
        PnlRtspSettings.Controls.Add(TxtRtspUrl)
        PnlRtspSettings.Controls.Add(PnlUsbSettings)
        PnlRtspSettings.Location = New Point(1378, 11)
        PnlRtspSettings.Name = "PnlRtspSettings"
        PnlRtspSettings.Size = New Size(278, 100)
        PnlRtspSettings.TabIndex = 0
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(3, 3)
        Label1.Name = "Label1"
        Label1.Size = New Size(57, 15)
        Label1.TabIndex = 1
        Label1.Text = "RTSP URL"
        ' 
        ' TxtRtspUrl
        ' 
        TxtRtspUrl.Location = New Point(3, 21)
        TxtRtspUrl.Name = "TxtRtspUrl"
        TxtRtspUrl.Size = New Size(271, 23)
        TxtRtspUrl.TabIndex = 0
        TxtRtspUrl.Text = "rtsp://192.168.x.x:8554/live"
        ' 
        ' ChkShowTimestamp
        ' 
        ChkShowTimestamp.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        ChkShowTimestamp.AutoSize = True
        ChkShowTimestamp.Checked = True
        ChkShowTimestamp.CheckState = CheckState.Checked
        ChkShowTimestamp.Location = New Point(615, 677)
        ChkShowTimestamp.Name = "ChkShowTimestamp"
        ChkShowTimestamp.Size = New Size(135, 19)
        ChkShowTimestamp.TabIndex = 22
        ChkShowTimestamp.Text = "映像に時刻を表示する"
        ChkShowTimestamp.UseVisualStyleBackColor = True
        ' 
        ' FrmMain
        ' 
        AutoScaleDimensions = New SizeF(96F, 96F)
        AutoScaleMode = AutoScaleMode.Dpi
        ClientSize = New Size(1264, 785)
        Controls.Add(ChkShowTimestamp)
        Controls.Add(PnlVideoSettings)
        Controls.Add(PnlRtspSettings)
        Controls.Add(PnlZwoSettings)
        Controls.Add(ChkEnableDetect)
        Controls.Add(LblDisplayAlpha)
        Controls.Add(LblGamma)
        Controls.Add(TrcGamma)
        Controls.Add(ChkContinuousRecord)
        Controls.Add(LblDisplayBeta)
        Controls.Add(TrkbDisplayBeta)
        Controls.Add(PnlSettingsBase)
        Controls.Add(TrkbDisplayAlpha)
        Controls.Add(ChkMuteSound)
        Controls.Add(BtnEditMaskDirect)
        Controls.Add(ChkDualRecord)
        Controls.Add(BtnSettings)
        Controls.Add(BtnSnapshot)
        Controls.Add(ChkShowSub)
        Controls.Add(LstLogs)
        Controls.Add(StsMain)
        Controls.Add(GrpSource)
        Controls.Add(BtnStop)
        Controls.Add(BtnStart)
        Controls.Add(PicMain)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Margin = New Padding(2)
        MinimumSize = New Size(1280, 824)
        Name = "FrmMain"
        Text = "流星自動観測システム - Meteor Detector"
        CType(PicMain, ComponentModel.ISupportInitialize).EndInit()
        GrpSource.ResumeLayout(False)
        GrpSource.PerformLayout()
        PnlVideoSettings.ResumeLayout(False)
        PnlVideoSettings.PerformLayout()
        PnlZwoSettings.ResumeLayout(False)
        PnlZwoSettings.PerformLayout()
        GrpWB.ResumeLayout(False)
        GrpWB.PerformLayout()
        CType(TrkbWbB, ComponentModel.ISupportInitialize).EndInit()
        CType(TrkbWbR, ComponentModel.ISupportInitialize).EndInit()
        GrpExp.ResumeLayout(False)
        GrpExp.PerformLayout()
        CType(TrkbExp, ComponentModel.ISupportInitialize).EndInit()
        GrpGain.ResumeLayout(False)
        GrpGain.PerformLayout()
        CType(TrkbGain, ComponentModel.ISupportInitialize).EndInit()
        PnlUsbSettings.ResumeLayout(False)
        PnlUsbSettings.PerformLayout()
        StsMain.ResumeLayout(False)
        StsMain.PerformLayout()
        CType(TrkbDisplayAlpha, ComponentModel.ISupportInitialize).EndInit()
        CType(TrkbDisplayBeta, ComponentModel.ISupportInitialize).EndInit()
        CType(TrcGamma, ComponentModel.ISupportInitialize).EndInit()
        PnlRtspSettings.ResumeLayout(False)
        PnlRtspSettings.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents PicMain As PictureBox
    Friend WithEvents BtnStart As Button
    Friend WithEvents BtnStop As Button
    Friend WithEvents GrpSource As GroupBox
    Friend WithEvents RdoVideo As RadioButton
    Friend WithEvents RdoZwo As RadioButton
    Friend WithEvents RdoUsb As RadioButton
    Friend WithEvents PnlSettingsBase As Panel
    Friend WithEvents PnlVideoSettings As Panel
    Friend WithEvents PnlZwoSettings As Panel
    Friend WithEvents PnlUsbSettings As Panel
    Friend WithEvents StsMain As StatusStrip
    Friend WithEvents LblStatusMessage As ToolStripStatusLabel
    Friend WithEvents LblStatusResolution As ToolStripStatusLabel
    Friend WithEvents LblStatusFps As ToolStripStatusLabel
    Friend WithEvents LstLogs As ListBox
    Friend WithEvents BtnRefreshUsb As Button
    Friend WithEvents CmbUsbDeviceList As ComboBox
    Friend WithEvents BtnRefreshZwo As Button
    Friend WithEvents CmbZwoDeviceList As ComboBox
    Friend WithEvents ChkShowSub As CheckBox
    Friend WithEvents TxtFileName As TextBox
    Friend WithEvents BtnOpenFile As Button
    Friend WithEvents BtnSnapshot As Button
    Friend WithEvents BtnSettings As Button
    Friend WithEvents CmbUsbResolution As ComboBox
    Friend WithEvents LblUsbResolution As Label
    Friend WithEvents ChkDualRecord As CheckBox
    Friend WithEvents BtnEditMaskDirect As Button
    Friend WithEvents ChkMuteSound As CheckBox
    Friend WithEvents BtnConnect As Button
    Friend WithEvents LblBinning As Label
    Friend WithEvents GrpExp As GroupBox
    Friend WithEvents LblExp As Label
    Friend WithEvents ChkExpAuto As CheckBox
    Friend WithEvents TrkbExp As TrackBar
    Friend WithEvents GrpGain As GroupBox
    Friend WithEvents LblGain As Label
    Friend WithEvents ChkGainAuto As CheckBox
    Friend WithEvents TrkbGain As TrackBar
    Friend WithEvents CmbBinning As ComboBox
    Friend WithEvents GrpWB As GroupBox
    Friend WithEvents Lbl_B As Label
    Friend WithEvents Lbl_R As Label
    Friend WithEvents LblWb As Label
    Friend WithEvents ChkWBAuto As CheckBox
    Friend WithEvents TrkbWbB As TrackBar
    Friend WithEvents TrkbWbR As TrackBar
    Friend WithEvents TmrUIUpdate As Timer
    Friend WithEvents TrkbDisplayAlpha As TrackBar
    Friend WithEvents TrkbDisplayBeta As TrackBar
    Friend WithEvents LblDisplayAlpha As Label
    Friend WithEvents LblDisplayBeta As Label
    Friend WithEvents ChkContinuousRecord As CheckBox
    Friend WithEvents TrcGamma As TrackBar
    Friend WithEvents LblGamma As Label
    Friend WithEvents ChkEnableDetect As CheckBox
    Friend WithEvents BtnZwoSettings As Button
    Friend WithEvents LblStatusTemp As ToolStripStatusLabel
    Friend WithEvents RdoRtsp As RadioButton
    Friend WithEvents PnlRtspSettings As Panel
    Friend WithEvents TxtRtspUrl As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents ChkShowTimestamp As CheckBox
    Friend WithEvents CmbRoiSelect As ComboBox
    Friend WithEvents Label2 As Label
End Class
