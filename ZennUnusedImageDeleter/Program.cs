using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using IniParser;
using IniParser.Model;


namespace ZennUnusedImageDeleter
{
    internal class Program
    {
        static FileInfo[] GetMarkdownFileInfo(string dirPath) => new DirectoryInfo(dirPath).GetFiles("*.md", SearchOption.AllDirectories);
        static FileInfo[] GetImageFileInfo(string dirPath) => new DirectoryInfo(dirPath).GetFiles("*.png", SearchOption.AllDirectories);


        static void Main(string[] args)
        {
            var iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.ini");
            if (!File.Exists(iniPath))
            {
                Console.WriteLine("settings.iniが見つかりません。");
                return;
            }

            IniData data;
            try
            {
                var parser = new FileIniDataParser();
                data = parser.ReadFile(iniPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"INIファイル読み込みエラー: {ex.Message}");
                return;
            }

            string repoDirPath = data["Paths"]["repositoryDirPath"];
            string imagesDirPath = Path.Combine(repoDirPath, "images");
            string articleDirPath = Path.Combine(repoDirPath, "articles");
            string bookDirPath = Path.Combine(repoDirPath, "books");

            if (!Directory.Exists(imagesDirPath) || !Directory.Exists(articleDirPath) || !Directory.Exists(bookDirPath))
            {
                Console.WriteLine("リポジトリ内に必要なフォルダが存在しません。");
                return;
            }

            var pngLines = new HashSet<string>();
            pngLines.UnionWith(GetPngLineList(GetMarkdownFileInfo(articleDirPath)));
            pngLines.UnionWith(GetPngLineList(GetMarkdownFileInfo(bookDirPath)));

            var deletePngFileList = new List<string>();
            foreach (var imgFile in GetImageFileInfo(imagesDirPath))
            {
                bool exists = false;
                foreach (var line in pngLines)
                {
                    if (line.Contains(imgFile.Name))
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                    deletePngFileList.Add(imgFile.FullName);
            }

            if (deletePngFileList.Count == 0)
            {
                Console.WriteLine("削除するファイルがありません。");
                return;
            }

            foreach (var filePath in deletePngFileList)
            {
                try
                {
                    var fileInfo = new FileInfo(filePath);
                    if (!fileInfo.IsReadOnly)
                        File.Delete(filePath);
                    else
                        Console.WriteLine($"スキップしました (読み取り専用): {filePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"削除に失敗しました {filePath}: {ex.Message}");
                }
            }

            var outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output");
            Directory.CreateDirectory(outputDir);

            File.WriteAllLines(Path.Combine(outputDir, "deletePngFileList.txt"), deletePngFileList);
            File.WriteAllLines(Path.Combine(outputDir, "usedPngLineList.txt"), pngLines);

            Console.WriteLine("削除が完了しました。");
        }

        static HashSet<string> GetPngLineList(FileInfo[] files)
        {
            var lines = new HashSet<string>();
            foreach (var file in files)
            {
                foreach (var line in File.ReadLines(file.FullName))
                {
                    if (Regex.IsMatch(line, @"!\[.*?\]\(.*\.png\)"))
                        lines.Add(line);
                }
            }
            return lines;
        }
    }
}
