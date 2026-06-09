# MeteorDetect_Win

Windows Forms、OpenCVSharp、およびZWO ASIカメラSDKを組み合わせた、高性能な流星自動検知・常時録画システムです。
個人的に作成したツールの為、無保証、無サポートです。

## 主な機能
- **マルチソース対応**: USBカメラ、ZWO ASI冷却/非冷却カメラ、RTSPストリーム（ATOM Cam等）、動画ファイルに対応
- **超高速検知エンジン**: 差分比較明合成と確率的ハフ変換（HoughLinesP）による高精度な直線（流星軌跡）抽出
- **画質自動補正**: CLAHE（局所的ヒストグラム平滑化）を搭載し、光害や月明かりを抑えて天頂の微光流星をあぶり出し
- **デュアル録画システム**: 流星検知時に「枠なしの綺麗な動画」と「緑枠付きの証拠動画」を非同期で同時保存
- **流星軌跡の比較明合成写真**: イベント全体の流星の通り道を1枚のJPEG写真として自動合成出力
- **ATOM Cam互換常時録画**: バックグラウンドキュー（ゼロフレームドロップ）による1分ごとの常時録画機能

## 動作環境 / 必要コンポーネント
- Windows 10 / 11 (.NET 6.0以上推奨)
- **OpenCvSharp4** (NuGetにてインストール)
- **DirectShowLib** (NuGetにてインストール)
- **ASICamera2.dll** (ZWO公式SDKより入手し、実行フォルダに配置してください)

## 使い方
1. 本リポジトリをクローンまたはダウンロードします。
2. 実行環境に合わせて `ASICamera2.dll` を配置します。（ZWO社のサイトからASI Camera SDKをダウンロード）
https://dl.zwoastro.com/software?app=DeveloperCameraSdk&platform=windows86&region=Overseas
3. アプリを起動し、入力ソース（USB / ZWO / RTSP / Video）を選択して「開始」をクリックします。
4. 必要に応じて、メイン画面からダイレクトに「マスク編集（検知除外エリア指定）」を設定可能です。

## 謝辞 / Credits
本システムのコアとなる流星検出アルゴリズムおよび画像処理パイプラインの設計にあたっては、
kin-hasegawa 氏のオープンソースプロジェクトである meteor-detect を深く参考にさせていただきました。
素晴らしい先行知見と成果物の公開に、心より感謝と敬意を表します。
https://github.com/kin-hasegawa/meteor-detect
