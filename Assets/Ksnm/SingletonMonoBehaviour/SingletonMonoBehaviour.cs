/*
The zlib License

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

3. This notice may not be removed or altered from any source distribution.
*/
using UnityEngine;

namespace Ksnm
{
    /// <summary>
    /// シングルトンなMonoBehaviour
    /// </summary>
    /// <typeparam name="T">継承先のクラス</typeparam>
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// 唯一のインスタンスの参照
        /// </summary>
        protected static T instance;
        /// <summary>
        /// 唯一のインスタンスの参照
        /// インスタンスが無い場合に呼び出されると、ゲームオブジェクトを作成します。
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    // ヒエラルキーから検索
                    instance = (T)FindObjectOfType(typeof(T));
                    // ヒエラルキーに無ければ作成
                    if (instance == null)
                    {
                        // ゲームオブジェクトを作成しコンポーネントを追加＆取得
                        instance = new GameObject(typeof(T).Name).AddComponent<T>();
                    }
                }
                return instance;
            }
        }
        /// <summary>
        /// インスタンス化済みの場合、trueを返す。
        /// メモ：削除後 nullを設定していないが、instanceは自動でnullになるので問題無し。
        /// </summary>
        public static bool IsInstantiated
        {
            get
            {
                return instance != null;
            }
        }
        /// <summary>
        /// インスタンスが有る場合は、MonoBehaviour.isActiveAndEnabledと同等です。
        /// インスタンスが無い場合はfalseを返します。
        /// </summary>
        /// <remarks>
        /// Instanceを通して、isActiveAndEnabledを参照するとインスタンスが作成されてしまいます。
        /// インスタンスを作成せずにアクティブか確認する際に、このプロパティを使用します。
        /// </remarks>
        public static bool IsActiveAndEnabled
        {
            get
            {
                if (instance != null)
                {
                    return instance.isActiveAndEnabled;
                }
                return false;
            }
        }
        /// <summary>
        /// 継承先でのAwakeの代わり。
        /// ・2番目以降のインスタンスでは呼び出されません。
        /// </summary>
        protected virtual void OnAwake()
        {
        }
        #region MonoBehaviour
        /// <summary>
        /// 基本的に継承先ではoverrideしない。
        /// </summary>
        protected void Awake()
        {
            if (this == Instance)
            {
                DontDestroyOnLoad(this);
                OnAwake();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        #endregion MonoBehaviour
    }
}