Public Class FrmZWO_Settings
    ' ==========================================================
    ' ★追加：画面を開いている最中の「イベント暴発」を防ぐバリア
    ' ==========================================================
    Private _isInitializing As Boolean = True

    Private Sub FrmZWO_Settings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' 初期化スタート（通信をブロック）
        _isInitializing = True

        ' 1. 一旦すべてのUIを「無効（操作不可）」にする
        TrcUsbBandwidth.Enabled = False
        ChkHighSpeed.Enabled = False
        ChkCoolerOn.Enabled = False
        TrcTargetTemp.Enabled = False

        ' カメラが接続されていない場合はここで終了
        If FrmMain.CamID = -1 Then
            LblTargetTemp.Text = "カメラ未接続"
            Return
        End If

        ' 2. カメラに「持っている機能と限界値」を自己申告させる
        Dim numCtrl As Integer = 0
        ASI_SDK.ASIGetNumOfControls(FrmMain.CamID, numCtrl)

        For i As Integer = 0 To numCtrl - 1
            Dim caps As New ASI_SDK.ASI_CONTROL_CAPS()
            ASI_SDK.ASIGetControlCaps(FrmMain.CamID, i, caps)

            Select Case caps.ControlType
                Case ASI_SDK.ASI_CONTROL_TYPE.ASI_BANDWIDTHOVERLOAD
                    TrcUsbBandwidth.Minimum = caps.MinValue
                    TrcUsbBandwidth.Maximum = caps.MaxValue
                    TrcUsbBandwidth.Enabled = True
                    ' ★USB帯域のデフォルト値をラベルに表示（例: USB帯域 (Def: 50%):）
                    LblUsbBandwidth.Tag = caps.DefaultValue ' あとで使うためにTagに保存
                    LblUsbBandwidth.Text = $"USB帯域 (Def: {caps.DefaultValue}%):"

                Case ASI_SDK.ASI_CONTROL_TYPE.ASI_HIGH_SPEED_MODE
                    ChkHighSpeed.Enabled = True
                    ' デフォルト値をテキストに追記
                    ChkHighSpeed.Text = $"High Speed Mode (Def: {If(caps.DefaultValue = 1, "ON", "OFF")})"

                Case ASI_SDK.ASI_CONTROL_TYPE.ASI_COOLER_ON
                    ChkCoolerOn.Enabled = True
                    ' デフォルト値をテキストに追記
                    ChkCoolerOn.Text = $"冷却機能を有効にする (Def: {If(caps.DefaultValue = 1, "ON", "OFF")})"

                Case ASI_SDK.ASI_CONTROL_TYPE.ASI_TARGET_TEMP
                    TrcTargetTemp.Minimum = caps.MinValue
                    TrcTargetTemp.Maximum = caps.MaxValue
                    TrcTargetTemp.Enabled = True
                    ' ★目標温度のデフォルト値をラベルに表示
                    LblTargetTemp.Tag = caps.DefaultValue
                    LblTargetTemp.Text = $"目標温度 (Def: {caps.DefaultValue}℃):"
            End Select
        Next

        ' 3. FrmMainで取得済みのフラグに基づき、冷却UIを最終決定する
        If FrmMain.HasCooler Then
            ChkCoolerOn.Enabled = True
            TrcTargetTemp.Enabled = True

            If TrcTargetTemp.Maximum = 0 Then
                TrcTargetTemp.Minimum = -50
                TrcTargetTemp.Maximum = 40
            End If
        End If

        ' 4. 有効になったUIにだけ、My.Settings から保存されていた値を流し込む
        If TrcUsbBandwidth.Enabled Then
            TrcUsbBandwidth.Value = Math.Max(TrcUsbBandwidth.Minimum, Math.Min(TrcUsbBandwidth.Maximum, My.Settings.ZwoUsbBandwidth))
            ' デフォルト値を維持しつつ、現在の選択値を表示
            LblUsbBandwidth.Text = $"USB帯域 (Def: {LblUsbBandwidth.Tag}%): {TrcUsbBandwidth.Value}%"
        Else
            LblUsbBandwidth.Text = "USB帯域調整: 非対応"
        End If

        If ChkHighSpeed.Enabled Then
            ChkHighSpeed.Checked = My.Settings.ZwoHighSpeed
        Else
            ChkHighSpeed.Text = "High Speed Mode (非対応)"
        End If

        ' ★ここで CheckedChanged が暴発するが、バリアがあるので通信はされない
        If ChkCoolerOn.Enabled Then
            ChkCoolerOn.Checked = My.Settings.ZwoCoolerOn
        End If

        If TrcTargetTemp.Enabled Then
            TrcTargetTemp.Value = Math.Max(TrcTargetTemp.Minimum, Math.Min(TrcTargetTemp.Maximum, My.Settings.ZwoTargetTemp))
            ' デフォルト値を維持しつつ、現在の選択値を表示
            LblTargetTemp.Text = $"目標温度 (Def: {LblTargetTemp.Tag}℃): {TrcTargetTemp.Value}℃"
        Else
            LblTargetTemp.Text = "冷却機能: 非対応"
        End If

        ' ==========================================================
        ' ★追加：反転設定コンボボックスの初期化
        ' ==========================================================
        CmbFlip.Items.Clear()
        CmbFlip.Items.Add("反転なし")
        CmbFlip.Items.Add("左右反転")
        CmbFlip.Items.Add("上下反転")
        CmbFlip.Items.Add("180度回転 (上下左右)")

        ' My.Settingsから現在の設定を読み込む（0〜3の範囲外なら0にする安全対策）
        Dim flipVal As Integer = My.Settings.ZwoFlip
        If flipVal >= 0 AndAlso flipVal <= 3 Then
            CmbFlip.SelectedIndex = flipVal
        Else
            CmbFlip.SelectedIndex = 0
        End If

        ' 初期化完了！これ以降のユーザー操作だけをカメラに送る
        _isInitializing = False
    End Sub

    Private Sub TrcUsbBandwidth_Scroll(sender As Object, e As EventArgs) Handles TrcUsbBandwidth.Scroll
        ' ★追加：初期化中は無視
        If _isInitializing Then Return

        ' ★修正：デフォルト値の表示を維持する
        LblUsbBandwidth.Text = $"USB帯域 (Def: {LblUsbBandwidth.Tag}%): {TrcUsbBandwidth.Value}%"

        If FrmMain.CamID <> -1 Then
            ASI_SDK.ASISetControlValue(FrmMain.CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_BANDWIDTHOVERLOAD, TrcUsbBandwidth.Value, 0)
        End If
    End Sub

    Private Sub ChkHighSpeed_CheckedChanged(sender As Object, e As EventArgs) Handles ChkHighSpeed.CheckedChanged
        ' ★追加：初期化中は無視
        If _isInitializing Then Return

        If FrmMain.CamID <> -1 Then
            Dim val As Integer = If(ChkHighSpeed.Checked, 1, 0)
            ASI_SDK.ASISetControlValue(FrmMain.CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_HIGH_SPEED_MODE, val, 0)
        End If
    End Sub

    Private Sub ChkCoolerOn_CheckedChanged(sender As Object, e As EventArgs) Handles ChkCoolerOn.CheckedChanged
        ' ★追加：初期化中はカメラに「0℃」などを誤送信しない
        If _isInitializing Then Return

        If FrmMain.CamID <> -1 Then
            FrmMain.IsExternalChanging = True

            Dim isOn As Integer = If(ChkCoolerOn.Checked, 1, 0)
            ASI_SDK.ASISetControlValue(FrmMain.CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_COOLER_ON, isOn, 0)

            If isOn = 1 Then
                ASI_SDK.ASISetControlValue(FrmMain.CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_TARGET_TEMP, TrcTargetTemp.Value, 0)
            End If
        End If
    End Sub

    Private Sub TrcTargetTemp_Scroll(sender As Object, e As EventArgs) Handles TrcTargetTemp.Scroll
        ' ★追加：初期化中は無視
        If _isInitializing Then Return

        ' ★修正：デフォルト値の表示を維持する
        LblTargetTemp.Text = $"目標温度 (Def: {LblTargetTemp.Tag}℃): {TrcTargetTemp.Value}℃"

        If FrmMain.CamID <> -1 Then
            FrmMain.IsExternalChanging = True
            ASI_SDK.ASISetControlValue(FrmMain.CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_TARGET_TEMP, TrcTargetTemp.Value, 0)
        End If
    End Sub

    Private Sub BtnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click
        ' 1. UIの値を設定に格納
        My.Settings.ZwoUsbBandwidth = TrcUsbBandwidth.Value
        My.Settings.ZwoHighSpeed = ChkHighSpeed.Checked
        My.Settings.ZwoCoolerOn = ChkCoolerOn.Checked
        My.Settings.ZwoTargetTemp = TrcTargetTemp.Value

        ' 2. 反転設定の保存（0,1,2,3）
        My.Settings.ZwoFlip = CmbFlip.SelectedIndex

        ' 3. まとめて保存
        My.Settings.Save()

        ' 4. カメラが接続中なら、冷却などの安全な設定のみ即座に反映させる
        If FrmMain.CamID <> -1 Then
            ASI_SDK.ASISetControlValue(FrmMain.CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_TARGET_TEMP, TrcTargetTemp.Value, 0)
            ASI_SDK.ASISetControlValue(FrmMain.CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_COOLER_ON, If(ChkCoolerOn.Checked, 1, 0), 0)

            ' ★注意: ASI_FLIP は絶対に送らない！（ハードウェア反転による色化けを防ぐため）
            ' 裏で動いている ClsZwoCamera.vb が、保存された My.Settings.ZwoFlip を読み取って、
            ' OpenCVの機能で安全にソフトウェア反転を行ってくれます。
        End If

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class