/*
 Copyright (c) 2014-2015 Takahiro Kasanami
 
 This software is provided 'as-is', without any express or implied
 warranty. In no event will the authors be held liable for any damages
 arising from the use of this software.
 
 Permission is granted to anyone to use this software for any purpose,
 including commercial applications, and to alter it and redistribute it
 freely, subject to the following restrictions:
 
    1. The origin of this software must not be misrepresented; you must not
    claim that you wrote the original software. If you use this software
    in a product, an acknowledgment in the product documentation would be
    appreciated but is not required.
 
    2. Altered source versions must be plainly marked as such, and must not be
    misrepresented as being the original software.
 
    3. This notice may not be removed or altered from any source
    distribution.
 */
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
#if Ksnm_Using_UniLinq
using UniLinq;
#else
using System.Linq;
#endif

namespace Ksnm
{
    /// <summary>
    /// スクリプトファイルの書式（文字コード、改行コード）を規定値に変更します。
    /// </summary>
    public class ScriptFileFormater
    {
        /// <summary>
        /// UnicodeでBOMが付いたスクリプトファイルだけを
        /// 改行コード：LF
        /// 文字コード：UTF-8(BOM付き)
        /// へ変換します。
        /// </summary>
        [MenuItem("Assets/Script File Formater/to UTF-8(BOM,LF) from Unicode(BOM)", false, 0)]
        static void Unicode_BOM_To_UTF8_BOM_LF()
        {
            ProcessingToSelection(ChangeTo_LF_UTF8_BOM);
        }
        /// <summary>
        /// UnicodeでBOMが付いたスクリプトファイルを
        /// 改行コード：LF
        /// 文字コード：UTF-8(BOM付き)
        /// へ変換します。
        /// 
        /// BOMがついていないスクリプトファイルはUTF-8として処理。
        /// </summary>
        [MenuItem("Assets/Script File Formater/to UTF-8(BOM,LF) from Unicode(BOM) otherwise UTF-8", false, 1)]
        static void UTF8_To_UTF8_BOM_LF()
        {
            ProcessingToSelection((filePath) =>
                    ChangeFormat(filePath, "\n", new UTF8Encoding(true), new UTF8Encoding(false), true)
                );
        }

        #region Utility
        const string DialogTitle = "Script File Formater";
        /// <summary>
        /// このファイルのパス
        /// </summary>
        static string ThisFilePath
        {
            get
            {
                var thisFileFullPath = new System.Diagnostics.StackTrace(true).GetFrame(0).GetFileName();
                thisFileFullPath = thisFileFullPath.Replace('\\', '/');
                var thisFilePath = thisFileFullPath.Replace(Application.dataPath, string.Empty);
                thisFilePath = "Assets" + thisFilePath;
                return thisFilePath;
            }
        }
        /// <summary>
        /// 選択されたオブジェクトのパス
        /// </summary>
        static List<string> SelectionPaths
        {
            get
            {
                var selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
                var selectionPaths = new List<string>();
                for (int i = 0; i < selection.Length; ++i)
                {
                    selectionPaths.Add(AssetDatabase.GetAssetPath(selection[i]));
                }
                return selectionPaths;
            }
        }
        /// <summary>
        /// スクリプトの拡張子のファイルであれば、trueを返します。
        /// </summary>
        /// <param name="filePaths"></param>
        /// <returns></returns>
        static bool IsScriptFilePath(string filePath)
        {
            // ファイルかチェック
            if (File.Exists(filePath) == false)
                return false;
            // 拡張子をチェック
            {
                string extension = Path.GetExtension(filePath);
                if (extension == ".cs" ||
                    extension == ".js" ||
                    extension == ".boo")
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 指定されたスクリプトファイルを、指定した改行コード/文字コードへ変換します。
        /// funcの戻り値がfalseの場合、ループ処理を途中で抜けて終了します。
        /// </summary>
        public static void ProcessingToFiles(string[] paths, System.Func<string, bool> func)
        {
            bool scriptFileExists = false;
            foreach (var path in paths)
            {
                if (IsScriptFilePath(path) == false)
                    continue;
                scriptFileExists = true;
                if (func(path) == false)
                    break;
            }
            if (scriptFileExists == false)
            {
                EditorUtility.DisplayDialog(DialogTitle, "スクリプトファイルが見つかりませんでした。", "OK");
                return;
            }
            // 更新
            AssetDatabase.Refresh();
        }
        /// <summary>
        /// 選択されたファイルのパスを、funcの第一引数に渡します。
        /// </summary>
        static void ProcessingToSelection(System.Func<string, bool> func)
        {
            var selectionPaths = SelectionPaths;
            if (selectionPaths.Count <= 0)
            {
                EditorUtility.DisplayDialog(DialogTitle, "ファイルが選択されていません。", "OK");
                return;
            }
            if (EditorUtility.DisplayDialog(DialogTitle, "選択されたファイルまたはフォルダに含まれる\nスクリプトファイルを変更します。\nよろしいですか？", "OK", "キャンセル") == false)
            {
                return;
            }
            ProcessingToFiles(selectionPaths.ToArray(), func);
        }
        #endregion Utility

        #region Test
#if false
        /// <summary>
        /// テスト
        /// 選択されたスクリプトファイルをログ表示
        /// </summary>
        [MenuItem("Assets/Script File Formater/Test SelectionPaths")]
        public static void TestSelectionPaths()
        {
            string logText = "TestSelectionPaths\n";
            foreach (var filePath in SelectionPaths)
            {
                if (IsScriptFilePath(filePath))
                    logText += filePath + "\n";
            }
            Debug.Log(logText);
        }
        /// <summary>
        /// テスト
        /// 選択されたファイルのエンコーディングを検出
        /// </summary>
        [MenuItem("Assets/Script File Formater/Test Encoding")]
        public static void TestEncoding()
        {
            var selectionPaths = SelectionPaths;
            if (selectionPaths.Count <= 0)
            {
                Debug.Log("アセットが選択されていません。");
                return;
            }
            // 並び替え
            // [0]をフォルダのパスにする。
            selectionPaths.Sort((a, b) => a.Length - b.Length);
            var path = selectionPaths[0];
            string[] filePaths = null;
            if (File.Exists(path))
            {
                filePaths = new[] { path };
            }
            else if (Directory.Exists(path))
            {
                filePaths = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            }
            // ファイルが無ければ何もしない。
            if (filePaths == null)
            {
                Debug.Log("ファイルがありませんでした。");
                return;
            }
            StackableLog.Begin();
            foreach (var filePath in filePaths)
            {
                try
                {
                    var bytes = File.ReadAllBytes(filePath);
                    //エンコードを取得
                    Encoding encoding = null;
                    // 文字列に変換
                    var text = GetString(bytes, out encoding);
                    if (text != null)
                    {
                        Debug.Log(filePath + "\t" + encoding.CodePage.ToString("00000") + " " + encoding.EncodingName);
                    }
                    else
                    {
                        Debug.Log(filePath + "\tエンコーディング不明");
                    }
                }
                catch
                {
                    Debug.Log(filePath + "\t失敗");
                }
            }
            StackableLog.End();
        }
#endif
        #endregion Test

        #region Encoding操作
        /// <summary>
        /// 改行コード：LF
        /// 文字コード：UTF-8(BOM付き)
        /// </summary>
        static bool ChangeTo_LF_UTF8_BOM(string filePath)
        {
            return ChangeFormat(filePath, "\n", new UTF8Encoding(true), null, true);
        }
        /// <summary>
        /// 改行コード：LF
        /// 文字コード：UTF-8(BOM付き)
        /// </summary>
        public static void ChangeTo_LF_UTF8_BOM(string[] filePaths, bool enableWarning)
        {
            ChangeFormat(filePaths, "\n", new UTF8Encoding(true), enableWarning);
        }
        /// <summary>
        /// 改行コード：CRLF
        /// 文字コード：UTF-8(BOM付き)
        /// </summary>
        public static void ChangeTo_CRLF_UTF8_BOM(string[] filePaths, bool enableWarning)
        {
            ChangeFormat(filePaths, "\r\n", new UTF8Encoding(true), enableWarning);
        }
#if false// 使わないので無効化
        [MenuItem("Assets/Script File Formater/to LF UTF-16LE")]
        static void ChangeTo_LF_UTF16LE_BOM()
        {
            ProcessingToSelection(ChangeTo_LF_UTF16LE_BOM);
        }
        static bool ChangeTo_LF_UTF16LE_BOM(string filePath)
        {
            return ChangeFormat(filePath, "\n", new UnicodeEncoding(false, true));
        }
#endif
        /// <summary>
        /// 選択されたファイルを、指定した改行コード/文字コードへ変換します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="outNewLine"></param>
        /// <param name="outEncoding"></param>
        /// <param name="alternativeEncoding">エンコーディングが不明な場合、代わりのエンコーディング</param>
        /// <param name="enableWarning">警告の有無</param>
        /// <returns></returns>
        static bool ChangeFormat(string filePath, string outNewLine, Encoding outEncoding, Encoding alternativeEncoding, bool enableWarning)
        {
            try
            {
                var bytes = File.ReadAllBytes(filePath);
                //エンコードを取得
                Encoding encoding = null;
                // 文字列に変換
                var text = GetString(bytes, out encoding);
                if (text == null || encoding == null)
                {
                    if (alternativeEncoding == null)
                    {
                        if (enableWarning)
                        {
                            Debug.LogWarning("Non-support encoding:" + filePath);
                        }
                        return true;
                    }
                    // 代わりのエンコーディングで文字列に変換
                    encoding = alternativeEncoding;
                    text = encoding.GetString(bytes);
                }
                // 改行コード変更
                //  一旦"\n"に統一した後、OutNewLineに変更する。
                text = text.Replace("\r\n", "\n");
                if (outNewLine != "\n")
                    text = text.Replace("\n", outNewLine);
                //ファイルを保存
                File.WriteAllText(filePath, text, outEncoding);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            return true;
        }
        /// <summary>
        /// 指定されたファイルの内、スクリプトファイルを、指定した改行コード/文字コードへ変換します。
        /// </summary>
        static void ChangeFormat(string[] filePaths, string outNewLine, Encoding outEncoding, bool enableWarning)
        {
            foreach (var filePath in filePaths)
            {
                if (IsScriptFilePath(filePath) == false)
                    continue;
                ChangeFormat(filePath, outNewLine, outEncoding, null, enableWarning);
            }
        }
        /// <summary>
        /// バイト列から文字列を取得
        /// ・エンコーディングはバイト列から検出
        /// ・検出できなかった場合はnulを返す。
        /// </summary>
        /// <returns></returns>
        static string GetString(byte[] bytes, out Encoding decodedEncoding)
        {
            int bomSkipIndex = 0;
            decodedEncoding = DecodeEncoding(bytes, out bomSkipIndex);
            if (decodedEncoding != null)
            {
                return decodedEncoding.GetString(bytes, bomSkipIndex, bytes.Length - bomSkipIndex);
            }
            return null;
        }
        /// <summary>
        /// バイト列からエンコーディングを検出
        /// ・BOMがない場合、検出できません。
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="bomSkipIndex"></param>
        /// <returns></returns>
        static Encoding DecodeEncoding(byte[] bytes, out int bomSkipIndex)
        {
            // BOMのサイズの順に検査
            var encodings = new Encoding[]
        {
            new UnicodeEncoding(false,true),
            new UnicodeEncoding(true,true),
            new UTF8Encoding(true),
            new UTF32Encoding(true,true),
        };
            foreach (var encoding in encodings)
            {
                try
                {
                    bomSkipIndex = BomSkipIndex(bytes, encoding);
                    if (bomSkipIndex > 0)
                        return encoding;
                }
                catch
                {
                    // バイト配列の長さが短すぎる
                    break;
                }
            }
            bomSkipIndex = 0;
            return null;
        }
        /// <summary>
        /// BOMの次のインデックスを返します。
        /// BOMが無い場合は0を返します。
        /// </summary>
        static int BomSkipIndex(byte[] bytes, Encoding encoding)
        {
            var index = 0;
            var bom = encoding.GetPreamble();
            if (bom.Length > 0)
            {
                var tmp = new byte[bom.Length];
                System.Buffer.BlockCopy(bytes, 0, tmp, 0, bom.Length);
                if (bom.SequenceEqual(tmp))
                    index = bom.Length;
            }
            return index;
        }
        #endregion Encoding

        #region Copyright操作
#if false
        /// <summary>
        /// 選択されたスクリプトファイルの先頭に、コピーライト表記を挿入します。
        /// </summary>
        [MenuItem("Assets/Script File Formater/Insert Copyright")]
        static void InsertCopyrightToSelection()
        {
            ProcessingToSelection(InsertCopyright);
        }
        /// <summary>
        /// コピーライト表記
        /// </summary>
        static string CopyrightSource = null;
        /// <summary>
        /// 指定されたファイル/フォルダの先頭に、コピーライト表記を挿入します。
        /// </summary>
        static bool InsertCopyright(string filePath)
        {
            try
            {
                if (CopyrightSource == null)
                {
                    var copyrightSourcePath = Path.GetDirectoryName(ThisFilePath) + "/CopyrightSource.txt";
                    CopyrightSource = File.ReadAllText(copyrightSourcePath);
                    CopyrightSource = "/*\n" + CopyrightSource + "\n*/\n";
                }
                var bytes = File.ReadAllBytes(filePath);
                //エンコードを取得
                Encoding encoding = null;
                // 文字列に変換
                var text = GetString(bytes, out encoding);
                if (text == null || encoding == null)
                {
                    Debug.LogWarning("Non-support encoding:" + filePath);
                    return true;
                }
                // 改行コード変更
                //  一旦"\n"に統一した後、OutNewLineに変更する。
                text = text.Replace("\r\n", "\n");
                // 条件に合えば挿入
                if (text.IndexOf("using") == 0)
                {
                    text = CopyrightSource + text;
                }
                else
                {
                    //何もしない
                }
                //ファイルを保存
                File.WriteAllText(filePath, text, new UTF8Encoding(true));
                Debug.Log("InsertCopyrightToSelection " + filePath);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            return true;
        }
#endif
        #endregion Copyright
    }
}