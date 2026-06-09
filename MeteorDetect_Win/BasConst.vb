Module BasConst
    ' カメラ・入力ソースの種類を定義
    Public Enum SourceType
        USB_Camera = 0
        ZWO_ASI = 1
        Video_File = 2
    End Enum

    ' アプリケーション名
    Public Const APP_NAME As String = "Meteor Detector"

    ' 画面のログ（リストボックス）に表示する最大件数
    ' この数を超えると古いものから自動削除されます
    Public Const MAX_LOG_LINES As Integer = 30

    ' ==========================================================
    ' ※ 検知感度、最小エリア、蓄積秒、ガンマ、前後の録画秒数などは
    ' すべて「UI (設定画面)」と「My.Settings」に移行したため削除しました！
    ' ==========================================================

    ' --- 動画保存設定 ---
    Public Const VIDEO_EXTENSION As String = ".mp4"
    ' Windows標準で再生しやすい H.264 (AVC) または MP4V を指定
    Public Const VIDEO_CODEC As String = "avc1"

    ''' <summary>
    ''' 録画を停止する最小空き容量 (MB)
    ''' </summary>
    Public Const MIN_FREE_SPACE_MB As Long = 50

    ' --- 表示・録画用 画質調整変数 (Alpha/Betaの初期値) ---
    Public Const DEFAULT_ALPHA As Double = 1.0
    Public Const DEFAULT_BETA As Integer = 0
End Module