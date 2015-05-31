/*
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
 
    3. This notice may not be removed or altered from any source
    distribution.
*/
using UnityEngine;
using GoogleMobileAds.Api;

namespace Ksnm
{
    /// <summary>
    /// AdMob広告の管理
    /// </summary>
    public class AdMobManager : MonoBehaviour
    {
        [SerializeField]
        private string androidBannerUnitId;
        [SerializeField]
        private string androidInterstitialUnitId;
        [SerializeField]
        private string[] androidTestDeviceIds;
        [SerializeField]
        private string iosBannerUnitId;
        [SerializeField]
        private string iosInterstitialUnitId;
        [SerializeField]
        private string[] iosTestDeviceIds;
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

        private InterstitialAd interstitial = null;

        void Awake()
        {
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
            var adSize = GetBannerAdSize();
            BannerView bannerView = new BannerView(adUnitId, adSize, bannerPosition);
            var request = Build();
            bannerView.LoadAd(request);
        }

        /// <summary>
        /// インタースティシャル広告をロードします。
        /// ユニットIDが設定されてい場合は何もしません。
        /// 画面に表示するには、ShowInterstitial()を呼んでください。
        /// </summary>
        private void LoadInterstitial()
        {
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
            interstitial = new InterstitialAd(adUnitId);
            interstitial.AdClosed += InterstitialClosed;
            var request = Build();
            interstitial.LoadAd(request);
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
#if UNITY_ANDROID
            var deviceIDs = androidTestDeviceIds;
#elif UNITY_IPHONE
            var deviceIDs = iosTestDeviceIds;
#else
            var deviceIDs = new string[0];
#endif
            var builder = new AdRequest.Builder();
            if (Debug.isDebugBuild)
            {
                builder.AddTestDevice(AdRequest.TestDeviceSimulator);
                foreach (var deviceID in deviceIDs)
                {
                    builder.AddTestDevice(deviceID);
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
        /// <returns>true:成功 false:準備ができていないため失敗</returns>
        public void ShowInterstitial()
        {
            if (IsLoadedInterstitial() == false)
            {
                Debug.LogError("InterstitialAdがロードされていません");
                return;
            }
            interstitial.Show();
        }
    }
}
