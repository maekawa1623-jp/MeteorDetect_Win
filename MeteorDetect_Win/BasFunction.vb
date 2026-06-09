Imports OpenCvSharp
Imports OpenCvSharp.Extensions

Module BasFunction
    ''' <summary>
    ''' OpenCVのMat形式（映像フレーム）をWindows FormsのPictureBoxで
    ''' 表示できるBitmap形式に変換します。
    ''' </summary>
    Public Function MatToBitmap(ByVal mat As Mat) As System.Drawing.Bitmap
        If mat IsNot Nothing AndAlso Not mat.Empty() Then
            Return BitmapConverter.ToBitmap(mat)
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' カメラから現在の設定値を取得し、UIのラベルやスライダーを更新する
    ''' </summary>
    Public Sub UpdateUIFromCamera()
        If FrmMain.CamID = -1 Then Return

        ' ★ 全体を通してイベント連鎖（逆流）を防ぐ
        FrmMain._isUpdatingUI = True

        Dim val As Integer = 0
        Dim isAuto As Integer = 0

        ' ==========================================
        ' 1. ゲイン取得と反映
        ' ==========================================
        If ASI_SDK.ASIGetControlValue(FrmMain.CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_GAIN, val, isAuto) = 0 Then
            ' 範囲外エラー防止チェック
            If val >= FrmMain.TrkbGain.Minimum AndAlso val <= FrmMain.TrkbGain.Maximum Then
                FrmMain.TrkbGain.Value = CInt(val)
            End If
            FrmMain.LblGain.Text = $"Gain: {val} {(If(isAuto = 1, "(Auto)", ""))}"
        End If

        ' ==========================================
        ' 2. 露出取得と反映
        ' ==========================================
        If ASI_SDK.ASIGetControlValue(FrmMain.CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_EXPOSURE, val, isAuto) = 0 Then
            Dim valMs As Integer = CInt(val / 1000.0)
            ' 範囲外エラー防止チェック
            If valMs >= FrmMain.TrkbExp.Minimum AndAlso valMs <= FrmMain.TrkbExp.Maximum Then
                FrmMain.TrkbExp.Value = valMs
            End If
            ' 小数点ではなく整数で表示（トラックバーに合わせる）
            FrmMain.LblExp.Text = $"Exp: {valMs} ms {(If(isAuto = 1, "(Auto)", ""))}"
        End If

        ' ==========================================
        ' 3. ホワイトバランス(赤・青)取得と反映
        ' ==========================================
        Dim rVal As Integer = 0, bVal As Integer = 0
        Dim rAuto As Integer = 0, bAuto As Integer = 0

        ASI_SDK.ASIGetControlValue(FrmMain.CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_WB_R, rVal, rAuto)
        ASI_SDK.ASIGetControlValue(FrmMain.CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_WB_B, bVal, bAuto)

        ' スライダーにも追随させる (範囲外チェック付き)
        If rVal >= FrmMain.TrkbWbR.Minimum AndAlso rVal <= FrmMain.TrkbWbR.Maximum Then
            FrmMain.TrkbWbR.Value = rVal
        End If
        If bVal >= FrmMain.TrkbWbB.Minimum AndAlso bVal <= FrmMain.TrkbWbB.Maximum Then
            FrmMain.TrkbWbB.Value = bVal
        End If

        FrmMain.LblWb.Text = $"WB (R/B): {rVal} / {bVal} {(If(rAuto = 1 OrElse bAuto = 1, "(Auto)", ""))}"

        ' ★ ロック解除
        FrmMain._isUpdatingUI = False
    End Sub

    ''' <summary>
    ''' カメラがサポートする制御項目（Gain, Exposure等）の限界値を取得し、UIのTrackBarに反映します
    ''' </summary>
    Public Sub InitializeCameraTrackBars()
        Dim numControls As Integer = 0
        ' 1. コントロールの総数を取得
        If ASI_SDK.ASIGetNumOfControls(FrmMain.CamID, numControls) <> 0 Then Return

        For i As Integer = 0 To numControls - 1
            Dim caps As New ASI_SDK.ASI_CONTROL_CAPS()
            ' 2. 各項目の詳細（最小・最大・デフォルト）を取得
            If ASI_SDK.ASIGetControlCaps(FrmMain.CamID, i, caps) = 0 Then
                Select Case caps.ControlType
                    Case ASI_SDK.ASI_CONTROL_TYPE.ASI_GAIN
                        SetTrackBarRange(FrmMain.TrkbGain, caps)
                    Case ASI_SDK.ASI_CONTROL_TYPE.ASI_EXPOSURE
                        ' ミリ秒単位に変換
                        Dim minMs As Integer = CInt(caps.MinValue / 1000)
                        Dim maxMs As Integer = CInt(caps.MaxValue / 1000)

                        ' ★ 上限を 5秒 (5,000 ms) に制限する
                        If maxMs > 5000 Then
                            maxMs = 5000
                        End If

                        FrmMain.TrkbExp.Minimum = minMs
                        FrmMain.TrkbExp.Maximum = maxMs

                        ' デフォルト値が上限を超えていないかチェックしてセット
                        Dim defMs As Integer = CInt(caps.DefaultValue / 1000)
                        If defMs > maxMs Then defMs = maxMs
                        If defMs < minMs Then defMs = minMs
                        FrmMain.TrkbExp.Value = defMs
                    Case ASI_SDK.ASI_CONTROL_TYPE.ASI_WB_R
                        SetTrackBarRange(FrmMain.TrkbWbR, caps)
                    Case ASI_SDK.ASI_CONTROL_TYPE.ASI_WB_B
                        SetTrackBarRange(FrmMain.TrkbWbB, caps)
                End Select
            End If
        Next
    End Sub

    ' 補助関数：TrackBarの値を安全にセット
    Private Sub SetTrackBarRange(ByRef trk As TrackBar, caps As ASI_SDK.ASI_CONTROL_CAPS)
        trk.Minimum = caps.MinValue
        trk.Maximum = caps.MaxValue
        ' 値が範囲内にあることを確認してセット
        Dim val = caps.DefaultValue
        If val < trk.Minimum Then val = trk.Minimum
        If val > trk.Maximum Then val = trk.Maximum
        trk.Value = val
    End Sub
End Module
