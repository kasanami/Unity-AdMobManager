using UnityEngine;
using System.Collections;

namespace Ksnm.Examples
{
    /// <summary>
    /// uGUIのボタンのonClickから関数を呼び出すためだけのクラス
    /// AdMobManager.ShowInterstitial()は戻り値が有り、
    /// 戻り値が有る関数をonClickから呼ぶとエラーになる。
    /// </summary>
    [RequireComponent(typeof(AdMobManager))]
    public class AdMobManagerExample : MonoBehaviour
    {
        [SerializeField]
        AdMobManager adMobManager;
        void Reset()
        {
            adMobManager = GetComponent<AdMobManager>();
        }
        /// <summary>
        /// インタースティシャル広告を表示
        /// </summary>
        public void ShowInterstitial()
        {
            adMobManager.ShowInterstitial();
        }
    }
}
