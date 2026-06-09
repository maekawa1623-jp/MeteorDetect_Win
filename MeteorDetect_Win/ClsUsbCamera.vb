Imports System.Threading
Imports OpenCvSharp
Imports System.Diagnostics ' デバッグ出力用に追加

Public Class ClsUsbCamera
    Implements ICameraProvider

    ' ICameraProviderで定義されたイベントの実装
    Public Event FrameArrived(ByVal frame As Mat) Implements ICameraProvider.FrameArrived

    Private _capture As VideoCapture
    Private _isRunning As Boolean
    Private _captureThread As Thread

    ' ==========================================================
    ' 【追加】外部(FrmMain)から要求する設定値を受け取るプロパティ
    ' ==========================================================
    Public Property ReqWidth As Integer = 1920
    Public Property ReqHeight As Integer = 1080
    Public Property ReqFps As Integer = 30
    Public Property ReqFormat As String = "MJPG"

    Public Function StartCamera(ByVal deviceIndex As Integer) As Boolean Implements ICameraProvider.StartCamera
        Try
            ' DirectShow(DSHOW)を明示的に指定してカメラを開く。
            ' これによりDirectShowLibで取得したデバイス名リストの順番(index)と完全に一致します。
            _capture = New VideoCapture(deviceIndex, VideoCaptureAPIs.DSHOW)

            If Not _capture.IsOpened() Then Return False

            ' ==========================================================
            ' シンプルに解像度だけを要求する（FPSとフォーマットはカメラなりに任せる）
            ' これによりキャプチャボードの最も安定したモード（1080p / 5fps / YUY2）で動作します
            ' ==========================================================
            _capture.Set(VideoCaptureProperties.FrameWidth, ReqWidth)
            _capture.Set(VideoCaptureProperties.FrameHeight, ReqHeight)

            ' 実際に設定された値を確認
            Dim actualW = _capture.Get(VideoCaptureProperties.FrameWidth)
            Dim actualH = _capture.Get(VideoCaptureProperties.FrameHeight)
            Dim actualFps = _capture.Get(VideoCaptureProperties.Fps)
            Debug.WriteLine($"[USB Camera] 安定起動: {actualW}x{actualH} @ {actualFps}fps")

            _isRunning = True
            _captureThread = New Thread(AddressOf CaptureLoop)
            _captureThread.IsBackground = True
            _captureThread.Start()

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub CaptureLoop()
        Dim frame As New Mat()

        While _isRunning
            ' _capture.Read は次の映像が届くまで待機（ブロック）します
            If _capture.Read(frame) AndAlso Not frame.Empty() Then
                ' UIスレッドで描画中に画像が破棄されないよう、Clone(コピー)を渡す
                RaiseEvent FrameArrived(frame.Clone())
            End If

            ' 注意: ここに Thread.Sleep() を入れるとFPSが大幅に低下するため入れません。
            ' _capture.Read 自体がカメラのハードウェアFPSに合わせて適切に待機してくれます。
        End While
        frame.Dispose()
    End Sub

    Public Sub StopCamera() Implements ICameraProvider.StopCamera
        _isRunning = False

        ' スレッドが安全に終了するのを最大1秒待機
        If _captureThread IsNot Nothing Then
            _captureThread.Join(1000)
        End If

        ' カメラデバイスの解放
        If _capture IsNot Nothing Then
            _capture.Release()
            _capture.Dispose()
            _capture = Nothing
        End If
    End Sub

    Public ReadOnly Property IsRunning As Boolean Implements ICameraProvider.IsRunning
        Get
            Return _isRunning
        End Get
    End Property
End Class
