/*************************************************************************
 *  Copyright © 2023 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  AssetBundleManager.cs
 *  Description  :  
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0
 *  Date         :  7/23/2023
 *  Description  :  Initial development version.
 *************************************************************************/

using System.Collections;
using UnityEngine;

namespace MGS.AssetBundles
{
    public sealed class AssetBundleManager
    {
        public static string AssetsRoot
        {
            set
            {
                assetsRoot = value;
                AssetBundleHub.Manifest.AssetsPath = AssetsPath;
            }
            get { return assetsRoot; }
        }
        static string assetsRoot = $"{Application.streamingAssetsPath}/AssetBundles";

        public static string AssetsPath
        {
            get { return $"{AssetsRoot}/{Platform}"; }
        }

        public static string Platform
        {
            get
            {
#if UNITY_IOS
                return "iOS";
#elif UNITY_ANDROID
                return "Android";
#else
                return "Windows";
#endif
            }
        }

        #region Sync
        public static AssetBundle LoadBundle(string abName)
        {
            var abFile = ResolveABFile(abName);
            return AssetBundleHub.LoadBundle(abFile);
        }

        public static T LoadAsset<T>(string abName, string assetName) where T : Object
        {
            var abFile = ResolveABFile(abName);
            return AssetBundleHub.LoadAsset<T>(abFile, assetName);
        }

        public static void UnloadCacheAB(string abName)
        {
            var abFile = ResolveABFile(abName);
            AssetBundleHub.UnloadCacheAB(abFile);
        }

        public static void UnloadCacheABs()
        {
            AssetBundleHub.UnloadCacheABs();
        }

        static string ResolveABFile(string abName)
        {
            return $"{AssetsPath}/{abName}";
        }
        #endregion

        #region Async
        public static IEnumerator LoadBundleAsync(string abName, System.Action<AssetBundle> finished)
        {
            var abFile = ResolveABFile(abName);
            yield return AssetBundleHub.LoadBundleAsync(abFile, finished);
        }

        public static IEnumerator LoadAssetAsync<T>(string abName, string assetName, System.Action<T> finished) where T : Object
        {
            var abFile = ResolveABFile(abName);
            yield return AssetBundleHub.LoadAssetAsync(abFile, assetName, finished);
        }

#if UNITY_2021
        public static IEnumerator UnloadCacheABAsync(string abName, System.Action finished = null)
        {
            var abFile = ResolveABFile(abName);
            yield return AssetBundleHub.UnloadCacheABAsync(abFile, finished);
        }

        public static IEnumerator UnloadCacheABsAsync(System.Action finished = null)
        {
            yield return AssetBundleHub.UnloadCacheABsAsync(finished);
        }
#endif
        #endregion
    }
}