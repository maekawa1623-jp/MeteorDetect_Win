

Public Class FrmSettings
    ' ==========================================
    ' 【追加】メイン画面から現在のカメラ映像を受け取るための変数
    ' ==========================================
    ' ==========================================
    ' 【修正】デザイナーが保存しようとしてエラーになるのを防ぐ「お札（属性）」を追加
    ' ==========================================
    '<System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    'Public Property CurrentLiveFrame As Image


    Private Sub FrmSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' ==========================================
        ' [タブ1: 保存設定] の初期化
        ' ==========================================
        Dim currentPath As String = My.Settings.SaveDirectoryBase
        If String.IsNullOrEmpty(currentPath) Then
            ' 初期値はマイビデオ
            currentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)
        End If
        TxtSavePath.Text = currentPath


        ' ★追加：常時録画フォルダの読み込み
        TxtContinuousDir.Text = My.Settings.ContinuousDirectory


        ' ==========================================
        ' [タブ2: 検知設定] の初期化 (スケルトン)
        ' ==========================================
        ' (将来拡張) My.Settings から DETECT_THRESHOLD などを読み込み、
        ' TrackBar や NumericUpDown にセットする処理をここに書きます。
        ' My.Settings からスライダーへ反映
        TrcThreshold.Value = My.Settings.SysThreshold
        LblThreshold.Text = $"感度 (Def:15): {TrcThreshold.Value}"

        TrcMinLength.Value = CInt(My.Settings.SysMinLength)
        LblMinLength.Text = $"最小長 (Def:20): {TrcMinLength.Value}"

        TrcAccSec.Value = CInt(My.Settings.SysAccSec * 10)
        LblAccSec.Text = $"蓄積秒 (Def:1.0): {TrcAccSec.Value / 10.0:F1}"

        TrcLineGap.Value = CInt(My.Settings.SysLineGap)
        LblLineGap.Text = $"連結許容 (Def:5): {TrcLineGap.Value}"

        TrcHough.Value = My.Settings.SysHough
        LblHough.Text = $"判定数 (Def:25): {TrcHough.Value}"

        ' ==========================================
        ' ★追加：[タブ2: 検知設定] 内の CLAHE 初期化
        ' ==========================================
        ' 有効/無効チェック
        ChkEnableClahe.Checked = My.Settings.SysEnableClahe

        ' コントラスト強調度 (Double値を10倍して整数スライダーへ)
        TrcClaheClip.Value = CInt(My.Settings.SysClaheClip * 10)
        LblClaheClip.Text = $"コントラスト強調 (Def:3.0): {TrcClaheClip.Value / 10.0:F1}"

        TxtAlertSound.Text = My.Settings.AlertSoundPath


        ' ==========================================
        ' [タブ3: 録画設定] の初期化 (スケルトン)
        ' ==========================================
        ' (将来拡張) My.Settings から PRE_ROLL_SEC などを読み込む処理を書きます。

        TrcPreRoll.Value = CInt(My.Settings.SysPreRoll)
        LblPreRoll.Text = $"録画前 (Def:3s): {TrcPreRoll.Value}s"

        TrcPostRoll.Value = CInt(My.Settings.SysPostRoll)
        LblPostRoll.Text = $"録画後 (Def:4s): {TrcPostRoll.Value}s"


    End Sub

    'Private Sub BtnEditMask_Click(sender As Object, e As EventArgs)
    '    ' カメラ映像が渡されていない場合は警告
    '    If CurrentLiveFrame Is Nothing Then
    '        MessageBox.Show("カメラを開始して、映像が映っている状態で設定を開いてください。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning)
    '        Return
    '    End If

    '    ' マスク描画専用の大きな画面（FrmMask）を呼び出す
    '    Using frm As New FrmMask
    '        ' 【修正3】Clone() の代わりに New Bitmap を使って安全に下敷きを作る
    '        Using bgImage = New Bitmap(CurrentLiveFrame)
    '            frm.Setup(bgImage)

    '            ' マスク画面を最前面で開く
    '            frm.ShowDialog(Me)
    '        End Using
    '    End Using
    'End Sub

    Private Sub BtnBrowse_Click(sender As Object, e As EventArgs) Handles BtnBrowse.Click
        Using fbd As New FolderBrowserDialog()
            fbd.Description = "データの保存先（ベースフォルダ）を選択してください。"
            fbd.SelectedPath = TxtSavePath.Text

            If fbd.ShowDialog() = DialogResult.OK Then
                TxtSavePath.Text = fbd.SelectedPath
            End If
        End Using

    End Sub

    ' ==========================================================
    ' 参照ボタンが押されたとき：ファイル選択ダイアログを開く
    ' ==========================================================
    Private Sub BtnBrowseSound_Click(sender As Object, e As EventArgs) Handles BtnBrowseSound.Click
        Using ofd As New OpenFileDialog()
            ' WAVファイルのみ選択できるように戻す
            ofd.Filter = "WAVEファイル (*.wav)|*.wav|すべてのファイル (*.*)|*.*"
            ofd.Title = "検知音の音声ファイルを選択してください"

            If ofd.ShowDialog() = DialogResult.OK Then
                TxtAlertSound.Text = ofd.FileName
            End If
        End Using
    End Sub

    ' ==========================================================
    ' テキストボックスに入っている音声ファイルをその場でテスト再生する
    ' ==========================================================
    Private Sub BtnTestSound_Click_1(sender As Object, e As EventArgs) Handles BtnTestSound.Click
        Dim path As String = TxtAlertSound.Text

        If String.IsNullOrEmpty(path) OrElse Not IO.File.Exists(path) Then
            MessageBox.Show("正しいファイルが選択されていません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            ' WAVファイルをテスト再生する
            My.Computer.Audio.Play(path, AudioPlayMode.Background)

        Catch ex As Exception
            MessageBox.Show("再生に失敗しました。" & vbCrLf & "エラー: " & ex.Message & vbCrLf & "※WAV形式（標準PCM）ではない可能性があります。",
                            "再生エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnSelectContinuousDir_Click(sender As Object, e As EventArgs) Handles BtnSelectContinuousDir.Click
        Using fbd As New FolderBrowserDialog()
            fbd.Description = "常時録画（ATOM互換）の保存先フォルダを選択してください"

            ' テキストボックスにパスが入っていれば、そこを初期位置にする
            If Not String.IsNullOrEmpty(TxtContinuousDir.Text) AndAlso IO.Directory.Exists(TxtContinuousDir.Text) Then
                fbd.SelectedPath = TxtContinuousDir.Text
            End If

            If fbd.ShowDialog() = DialogResult.OK Then
                TxtContinuousDir.Text = fbd.SelectedPath
            End If
        End Using
    End Sub

    Private Sub TrcThreshold_Scroll(sender As Object, e As EventArgs) Handles TrcThreshold.Scroll
        LblThreshold.Text = $"感度 (Def:15): {TrcThreshold.Value}"
    End Sub

    Private Sub TrcMinLength_Scroll(sender As Object, e As EventArgs) Handles TrcMinLength.Scroll
        LblMinLength.Text = $"最小長 (Def:20): {TrcMinLength.Value}"
    End Sub

    Private Sub TrcAccSec_Scroll(sender As Object, e As EventArgs) Handles TrcAccSec.Scroll
        LblAccSec.Text = $"蓄積秒 (Def:1.0): {TrcAccSec.Value / 10.0:F1}"
    End Sub

    Private Sub TrcLineGap_Scroll(sender As Object, e As EventArgs) Handles TrcLineGap.Scroll
        LblLineGap.Text = $"連結許容 (Def:5): {TrcLineGap.Value}"
    End Sub

    Private Sub TrcHough_Scroll(sender As Object, e As EventArgs) Handles TrcHough.Scroll
        LblHough.Text = $"判定数 (Def:25): {TrcHough.Value}"
    End Sub

    Private Sub TrcPreRoll_Scroll(sender As Object, e As EventArgs) Handles TrcPreRoll.Scroll
        LblPreRoll.Text = $"録画前 (Def:3s): {TrcPreRoll.Value}s"
    End Sub

    Private Sub TrcPostRoll_Scroll(sender As Object, e As EventArgs) Handles TrcPostRoll.Scroll
        LblPostRoll.Text = $"録画後 (Def:4s): {TrcPostRoll.Value}s"
    End Sub

    Private Sub BtnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click
        ' ==========================================
        ' [タブ1: 保存設定] の保存
        ' ==========================================
        My.Settings.SaveDirectoryBase = TxtSavePath.Text

        ' ★追加：常時録画フォルダの設定を保存
        My.Settings.ContinuousDirectory = TxtContinuousDir.Text

        ' ==========================================
        ' [タブ2・タブ3] の保存 (スケルトン)
        ' ==========================================
        ' (将来拡張) 画面上の数値を My.Settings に代入する処理をここに書きます。
        My.Settings.AlertSoundPath = TxtAlertSound.Text

        My.Settings.SysThreshold = TrcThreshold.Value
        My.Settings.SysMinLength = TrcMinLength.Value
        My.Settings.SysAccSec = TrcAccSec.Value / 10.0
        My.Settings.SysLineGap = TrcLineGap.Value
        My.Settings.SysHough = TrcHough.Value

        My.Settings.SysPreRoll = TrcPreRoll.Value
        My.Settings.SysPostRoll = TrcPostRoll.Value

        ' ==========================================
        ' ★追加：CLAHE 設定の保存
        ' ==========================================
        My.Settings.SysEnableClahe = ChkEnableClahe.Checked
        My.Settings.SysClaheClip = TrcClaheClip.Value / 10.0

        ' ==========================================
        ' [タブ4: SNS設定] の保存
        ' ==========================================

        ' すべての設定をディスクに書き込んで記憶させる
        My.Settings.Save()

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    ' ==========================================
    ' ★追加：CLAHE スライダーのスクロールイベント
    ' ==========================================
    Private Sub TrcClaheClip_Scroll(sender As Object, e As EventArgs) Handles TrcClaheClip.Scroll
        LblClaheClip.Text = $"コントラスト強調 (Def:3.0): {TrcClaheClip.Value / 10.0:F1}"
    End Sub


End Class