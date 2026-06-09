Imports System.Threading
Imports OpenCvSharp

''' <summary>
''' 動画ファイルを擬似カメラとして扱うクラス
''' </summary>
Public Class ClsVideoFile
    ' ==========================================================
    ' 【ここがエラー解消の鍵】ICameraProviderのルールに従う宣言
    ' ==========================================================
    Implements ICameraProvider

    ' イベントにも Implements ICameraProvider.FrameArrived を付けます
    Public Event FrameArrived(ByVal frame As Mat) Implements ICameraProvider.FrameArrived

    ' ==========================================================
    ' 【1】動画が最後まで到達したことを知らせるイベントを追加
    ' ==========================================================
    Public Event VideoFinished()

    Private _cap As VideoCapture
    Private _cts As CancellationTokenSource
    Private _isLoop As Boolean = False ' ループ再生フラグ (ループしない＝FALSE、ループする＝TRUE)
    Private _isRunningStatus As Boolean = False ' 動作中フラグ

    ' --- ICameraProvider で要求されるプロパティ・メソッドの実装 ---

    Public ReadOnly Property IsRunning As Boolean Implements ICameraProvider.IsRunning
        Get
            Return _isRunningStatus
        End Get
    End Property

    Public Function StartCamera(deviceIndex As Integer) As Boolean Implements ICameraProvider.StartCamera
        ' 動画ファイル再生ではこのメソッドは使わず、下の StartVideo を使いますが、
        ' インターフェースのルール上、定義しておく必要があります。
        Return False
    End Function

    Public Sub StopCamera() Implements ICameraProvider.StopCamera
        ' メイン画面から StopCamera() が呼ばれたら、動画停止メソッドに横流しする
        StopVideo()
    End Sub

    ' -----------------------------------------------------------
    ' 以下、動画再生のための専用ロジック
    ' -----------------------------------------------------------

    ''' <summary>
    ''' 動画再生を開始する（動画専用メソッド）
    ''' </summary>
    Public Function StartVideo(ByVal filePath As String, ByVal targetFps As Double) As Boolean
        Try
            _cap = New VideoCapture(filePath)
            If Not _cap.IsOpened() Then Return False

            _cts = New CancellationTokenSource()
            _isRunningStatus = True ' 実行中フラグをON

            ' 再生ループをバックグラウンドで開始
            Task.Run(Sub() PlayLoop(targetFps), _cts.Token)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 内部再生ループ
    ''' </summary>
    Private Sub PlayLoop(ByVal fps As Double)
        ' FPSから1フレームあたりの待機時間を計算
        Dim waitMs As Integer = If(fps > 0, CInt(1000 / fps), 33)
        Dim frame As New Mat()

        Try
            While _cts IsNot Nothing AndAlso Not _cts.IsCancellationRequested
                Dim startTime = DateTime.Now

                ' フレーム読み込み
                If _cap.Read(frame) AndAlso Not frame.Empty() Then

                    ' 【重要】Clone() で別メモリにしてメイン画面に渡す
                    RaiseEvent FrameArrived(frame.Clone())

                    ' 再生速度の調整（処理時間を差し引いて待機）
                    Dim elapsed = (DateTime.Now - startTime).TotalMilliseconds
                    Dim sleepTime = Math.Max(1, waitMs - CInt(elapsed))
                    Thread.Sleep(sleepTime)
                Else
                    ' 動画が終了した時の処理
                    If _isLoop Then
                        _cap.PosFrames = 0 ' 最初に戻る
                    Else
                        ' ==========================================================
                        ' 【2】ループしない場合、メイン画面に「終わったよ」と合図を送ってからループを抜ける
                        ' ==========================================================
                        RaiseEvent VideoFinished()
                        Exit While
                    End If
                End If
            End While
        Catch ex As Exception
            ' エラー時はループを抜ける
        Finally
            _isRunningStatus = False ' 実行中フラグをOFF
            If frame IsNot Nothing Then frame.Dispose()
            If _cap IsNot Nothing Then _cap.Release()
        End Try
    End Sub

    ''' <summary>
    ''' 再生停止
    ''' </summary>
    Public Sub StopVideo()
        If _cts IsNot Nothing Then
            _cts.Cancel()
        End If
        _isRunningStatus = False
    End Sub

End Class