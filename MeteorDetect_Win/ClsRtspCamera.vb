Imports System.Threading
Imports OpenCvSharp

Public Class ClsRtspCamera
    Implements ICameraProvider

    Public Event FrameArrived(ByVal frame As Mat) Implements ICameraProvider.FrameArrived

    Private _capture As VideoCapture
    Private _isRunning As Boolean
    Private _captureThread As Thread

    ' ==========================================================
    ' 【追加】RTSPのURL（rtsp://...）を保持するプロパティ
    ' ==========================================================
    Public Property ConnectionUrl As String = ""

    Public ReadOnly Property IsRunning As Boolean Implements ICameraProvider.IsRunning
        Get
            Return _isRunning
        End Get
    End Property

    ' 引数の deviceIndex はインターフェースのルール上存在しますが、RTSPでは使いません
    Public Function StartCamera(deviceIndex As Integer) As Boolean Implements ICameraProvider.StartCamera
        If String.IsNullOrEmpty(ConnectionUrl) Then Return False

        Try
            ' FFmpegバックエンドを明示的に指定してネットワークストリームを開く
            _capture = New VideoCapture(ConnectionUrl, VideoCaptureAPIs.FFMPEG)

            ' ★RTSP特有の遅延（数秒遅れる現象）を軽減するため、内部バッファを最小化する
            _capture.Set(VideoCaptureProperties.BufferSize, 1)

            If Not _capture.IsOpened() Then Return False

            _isRunning = True
            _captureThread = New Thread(AddressOf CaptureLoop) With {
                .IsBackground = True
            }
            _captureThread.Start()

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub CaptureLoop()
        Dim frame As New Mat()

        While _isRunning
            Try
                ' ネットワーク経由のため、パケットロスで一時的に空フレームが返ることがある
                If _capture.Read(frame) AndAlso Not frame.Empty() Then
                    ' クローンを渡してメインスレッドの処理と切り離す
                    RaiseEvent FrameArrived(frame.Clone())
                Else
                    ' 映像が途切れた場合は少し休んでリトライ（CPU負荷暴走防止）
                    Thread.Sleep(10)
                End If
            Catch ex As Exception
                ' ネットワークの瞬断でクラッシュしないように保護
                Thread.Sleep(100)
            End Try
        End While

        If frame IsNot Nothing Then frame.Dispose()
    End Sub

    Public Sub StopCamera() Implements ICameraProvider.StopCamera
        _isRunning = False

        ' もし _captureThread が空じゃなければ Join を実行する
        _captureThread?.Join(1000)

        ' もし _capture が空じゃなければ Release と Dispose を実行する
        _capture?.Release()
        _capture?.Dispose()
        _capture = Nothing
    End Sub

End Class