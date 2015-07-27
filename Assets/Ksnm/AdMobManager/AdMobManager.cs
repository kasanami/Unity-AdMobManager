/*
The zlib License

Copyright (c) 2015 Takahiro Kasanami

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
using GoogleMobileAds.Api;
using System;

namespace Ksnm
{
    /// <summary>
    /// AdMob広告の管理
    /// </summary>
    public class AdMobManager : SingletonMonoBehaviour<AdMobManager>
    {
        [SerializeField, HeaderAttribute("Android")]
        private string androidBannerUnitId;
        [SerializeField]
        private string androidInterstitialUnitId;
        [SerializeField]
        private TestDeviceInfo[] androidTestDeviceInfos;

        [SerializeField, HeaderAttribute("iOS")]
        private string iosBannerUnitId;
        [SerializeField]
        private string iosInterstitialUnitId;
        [SerializeField]
        private TestDeviceInfo[] iosTestDeviceInfos;

        [SerializeField, HeaderAttribute("共通")]
        private bool useTestDevice;

        [SerializeField]
        private bool enableBanner = true;

        [SerializeField]
        private bool enableInterstitial = true;

        [SerializeField]
        private AdPosition bannerPosition;

        /// <summary>
        /// バナーのサイズを列挙
        /// </summary>
        public enum BannerSize
        {
            Banner,// = new AdSize(320, 50);
            MediumRectangle,// = new AdSize(300, 250);
            IABBanner,// = new AdSize(468, 60);
            Leaderboard,// = new AdSize(728, 90);
            SmartBanner,// = new AdSize(true);
        }
        /// <summary>
        /// バナーのサイズを指定
        /// </summary>
        [SerializeField]
        private BannerSize bannerSize;

        [SerializeField, Range(0, 60), TooltipAttribute("インタースティシャル広告の再表示が\n可能になるまでの時間(分)")]
        int interstitialIntervalMinutes = 3;
        /// <summary>
        /// インタースティシャル広告が表示可能になる日時
        /// </summary>
        DateTime interstitialNextPermittedTime;

        private InterstitialAd interstitial = null;

        public event EventHandler OnInterstitialClosed = delegate { };

        /// <summary>
        /// Awakeの代わり
        /// </summary>
        protected override void OnAwake()
        {
            interstitialNextPermittedTime = DateTime.Now;
            // 起動時にインタースティシャル広告をロード
            LoadInterstitial();
            // バナー広告を表示
            LoadBanner();
        }

        /// <summary>
        /// バナー広告をロードします。
        /// ユニットIDが設定されてい場合は何もしません。
        /// 画面に表示されます。
        /// </summary>
        private void LoadBanner()
        {
            if (enableBanner == false)
            {
                return;
            }
#if UNITY_ANDROID
            var adUnitId = androidBannerUnitId;
#elif UNITY_IPHONE
            var adUnitId = iosBannerUnitId;
#else
            var adUnitId = string.Empty;
#endif
            if (string.IsNullOrEmpty(adUnitId))
            {
                Debug.LogWarning("ユニットIDが設定されていません");
                return;
            }
#if UNITY_EDITOR
            // エディタではロードは成功するが、なにも表示されず ややこしいのでロードしない。
#else
            var adSize = GetBannerAdSize();
            BannerView bannerView = new BannerView(adUnitId, adSize, bannerPosition);
            var request = Build();
            bannerView.LoadAd(request);
#endif
        }

        /// <summary>
        /// インタースティシャル広告をロードします。
        /// ユニットIDが設定されてい場合は何もしません。
        /// 画面に表示するには、ShowInterstitial()を呼んでください。
        /// </summary>
        private void LoadInterstitial()
        {
            if (enableInterstitial == false)
            {
                return;
            }
#if UNITY_ANDROID
            string adUnitId = androidInterstitialUnitId;
#elif UNITY_IPHONE
            string adUnitId = iosInterstitialUnitId;
#else
            string adUnitId = string.Empty;
#endif
            if (string.IsNullOrEmpty(adUnitId))
            {
                Debug.LogWarning("ユニットIDが設定されていません");
                return;
            }
#if UNITY_EDITOR
            // エディタではロードは成功するが、なにも表示されず ややこしいのでロードしない。
#else
            interstitial = new InterstitialAd(adUnitId);
            interstitial.AdClosed += InterstitialClosed;
            var request = Build();
            interstitial.LoadAd(request);
#endif
        }

        private AdSize GetBannerAdSize()
        {
            switch (bannerSize)
            {
                case BannerSize.Banner: return AdSize.Banner;
                case BannerSize.MediumRectangle: return AdSize.MediumRectangle;
                case BannerSize.IABBanner: return AdSize.IABBanner;
                case BannerSize.Leaderboard: return AdSize.Leaderboard;
                case BannerSize.SmartBanner: return AdSize.SmartBanner;
            }
            return null;
        }

        private AdRequest Build()
        {
            var builder = new AdRequest.Builder();
            if (useTestDevice)
            {
#if UNITY_ANDROID
                var deviceInfos = androidTestDeviceInfos;
#elif UNITY_IPHONE
                var deviceInfos = iosTestDeviceInfos;
#else
                var deviceInfos = new TestDeviceInfo[0];
#endif
                builder.AddTestDevice(AdRequest.TestDeviceSimulator);
                foreach (var deviceInfo in deviceInfos)
                {
                    builder.AddTestDevice(deviceInfo.id);
                }
            }
            return builder.Build();
        }

        /// <summary>
        /// インタースティシャル広告が閉じられた
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void InterstitialClosed(object sender, System.EventArgs e)
        {
            // 再表示するために削除
            interstitial.Destroy();
            interstitial = null;
            // 再ロード
            LoadInterstitial();
            // 次の日時を計算
            interstitialNextPermittedTime = DateTime.Now + TimeSpan.FromMinutes(interstitialIntervalMinutes);
            
            OnInterstitialClosed(this, EventArgs.Empty);
        }

        /// <summary>
        /// インタースティシャル広告がロード済みか取得
        /// </summary>
        public bool IsLoadedInterstitial()
        {
            if (interstitial == null)
            {
                return false;
            }
            return interstitial.IsLoaded();
        }

        /// <summary>
        /// インタースティシャル広告を表示
        /// </summary>
        /// <returns>true:成功しました。広告が表示されます。 false:準備ができていないため失敗。広告は表示されません。</returns>
        public bool ShowInterstitial()
        {
#if UNITY_EDITOR
            // エディタではロードが成功しても なにも表示されず ややこしいので、表示しない。
#else
            if (IsLoadedInterstitial() == false)
            {
                Debug.LogError("InterstitialAdがロードされていません");
                return false;
            }
            // 表示可能になる日時になっていれば表示
            if (DateTime.Now >= interstitialNextPermittedTime)
            {
                interstitial.Show();
                return true;
            }
#endif
            return false;
        }

        /// <summary>
        /// テストデバイスの情報
        /// ・IDだけだと、どのデバイスなのか分からないので追加
        /// </summary>
        [System.Serializable]
        public class TestDeviceInfo
        {
            public string name;
            public string id;
        }
    }
}
