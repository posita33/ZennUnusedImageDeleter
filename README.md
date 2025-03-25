# ZennUnusedImageDeleter

Zennのリポジトリから未使用のPngファイルを削除するツール

## ツールを作った経緯

Zennで本を執筆する際に、PasteImageで画像を上書きすると未使用な画像が増えていきます。
上書きしたことで未使用のpngファイルを削除するツールを作成しました。

## ツールの使い方

ZennをGitHubで更新している場合に以下のようなフォルダ構成になります。

```
(リポジトリのパス)
 ├articles
 ├books
 └images
```

「ZennUnusedImageDeleter.exe」と同じフォルダにある「settings.ini」にリポジトリのフルパスを設定します。

```settings.ini
[Paths]
repositoryDirPath=D:\Projects\YourRepository
```

「settings.ini」にリポジトリのフルパスを設定したら、「ZennUnusedImageDeleter.exe」を実行します。
