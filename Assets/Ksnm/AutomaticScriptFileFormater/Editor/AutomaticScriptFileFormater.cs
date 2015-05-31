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

namespace Ksnm
{
    /// <summary>
    /// スクリプトファイルの書式（文字コード、改行コード）を規定値に変更します。
    /// </summary>
    public class AutomaticScriptFileFormater : AssetPostprocessor
    {
        /// <summary>
        /// あらゆる種類のアセットで、任意の数のインポートが完了したときに呼ばれる処理です。
        /// </summary>
        /// <param name="importedAssets"> インポートされたアセットのファイルパス。 </param>
        /// <param name="deletedAssets"> 削除されたアセットのファイルパス。 </param>
        /// <param name="movedAssets"> 移動されたアセットのファイルパス。 </param>
        /// <param name="movedFromPath"> 移動されたアセットの移動前のファイルパス。 </param>
        static void OnPostprocessAllAssets
            (string[] importedAssets, string[] deletedAssets,
             string[] movedAssets, string[] movedFromPath)
        {
            if (importedAssets.Length > 0)
            {
                ScriptFileFormater.ChangeTo_LF_UTF8_BOM(importedAssets, false);
            }
        }
    }
}