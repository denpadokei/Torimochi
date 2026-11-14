# Torimochi

[![Build and Release](https://github.com/denpadokei/Torimochi/actions/workflows/build-and-release.yml/badge.svg)](https://github.com/denpadokei/Torimochi/actions/workflows/build-and-release.yml)

斬ったノーツがセイバーに張り付く MOD です。斬った位置と向きのままノーツのメッシュがセイバーに残り、一定数まで積み重なります。

## 機能

- 斬ったノーツのメッシュをセイバーに張り付けて表示
- 通常ノーツ、バーストスライダーのヘッドとエレメントに対応 (ボムは対象外)
- ノーツの色、矢印・ドットの表示をそのまま再現
- 張り付くノーツが上限を超えると、古いものから順に消える
- Custom Notes 導入時にカスタムノーツを選択している場合はそのメッシュを使用
- TrickSaber (1.1.3 以上 2.0.0 未満) 導入時は、TrickSaber 側のセイバーモデルにも同じノーツを表示
- ゲームプレイ設定タブから有効/無効と一人称視点での表示を切り替え

## 動作環境

| 項目 | 内容 |
| --- | --- |
| Beat Saber | 1.39.1 |
| 依存 MOD | BSIPA 4.3.5 / SiraUtil 3.1.14 / BeatSaberMarkupLanguage 1.12.3 |

## インストール

1. Beat Saber を 1.39.1 に合わせる (BSManager などのバージョン管理ツールを推奨)
2. BSIPA と上記の依存 MOD をインストールする
3. [Releases](https://github.com/denpadokei/Torimochi/releases) から `Torimochi-2.0.0-bs1.39.1.zip` をダウンロードし、`Plugins` フォルダをゲームフォルダへ上書きコピーする

## 使い方

ゲームプレイ設定 (Gameplay Setup) の「Torimochi」タブで `Enable` と `Show first persion view` を切り替えられます。

- Custom Notes を導入していてノートを選択している場合、そのノートのメッシュが使われます。選択していない場合はゲーム標準のノートが使われます
- TrickSaber 1.1.3 以上 2.0.0 未満を導入していると、TrickSaber のモデルにもノーツが表示されます

## 設定

| 項目 | 型 | 既定値 | 説明 |
| --- | --- | --- | --- |
| Enable | bool | true | MOD を有効にするか。無効にするとゲームプレイ時に何も表示されない |
| MaxNotesCount | int | 100 | 同時に張り付けておけるノーツの最大数。超えた分は古いものから消える |
| ShowHMDCam | bool | true | 一人称視点 (HMD カメラ) でもノーツを表示するか。false にすると三人称カメラからのみ見える |

`MaxNotesCount` はゲーム内の設定タブには無いため、`UserData/Torimochi.json` を直接編集してください。

## ビルド

```powershell
git clone https://github.com/denpadokei/Torimochi.git
cd Torimochi
dotnet build .\Torimochi\Torimochi.csproj -c Release
```

`BeatSaberDir` に Beat Saber のインストール先を設定すると、ゲームの参照アセンブリを解決できます。

GitHub Actions の `build-and-release.yml` が push ごとにビルドし、`X.X.X` 形式のタグを push するとリリースを作成します。

## ライセンス

MIT
