/*************************************************************************
 *  Copyright © 2023 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  AssetBundleHub.cs
 *  Description  :  
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0
 *  Date         :  7/23/2023
 *  Description  :  Initial development version.
 *************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGS.AssetBundles
{
    public sealed class AssetBundleHub
    {
        #region Platform
        public static string AssetsRoot
        {
            set
            {
                assetsRoot = value;
                Manifest.AssetsRoot = $"{assetsRoot}/{Platform}";
            }
            get { return assetsRoot; }
        }
        static string assetsRoot;

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
        #endregion

        #region Refs
        public static IAssetManifest Manifest { set; get; } = new AssetManifest();
        static Dictionary<string, List<string>> depRefs = new Dictionary<string, List<string>>();
        #endregion

        #region Sync
        public static AssetBundle LoadBundle(string abName)
        {
            LoadDepBundles(abName);

            var abFile = Manifest.GetAssetPath(abName);
            return AssetBundleHandler.LoadBundle(abFile);
        }

        public static T LoadAsset<T>(string abName, string assetName) where T : Object
        {
            LoadDepBundles(abName);

            var abFile = Manifest.GetAssetPath(abName);
            return AssetBundleHandler.LoadAsset<T>(abFile, assetName);
        }

        public static void UnloadCacheAB(string abName)
        {
            if (!depRefs.ContainsKey(abName))
            {
                var abFile = Manifest.GetAssetPath(abName);
                AssetBundleHandler.UnloadCacheAB(abFile);
                UnloadDepBundles(abName);
            }
        }

        public static void UnloadCacheABs()
        {
            depRefs.Clear();
            AssetBundleHandler.UnloadCacheABs();
        }

        static void LoadDepBundles(string abName)
        {
            var depAssets = Manifest.GetDependences(abName);
            if (depAssets != null)
            {
                foreach (var depAsset in depAssets)
                {
                    LoadDepBundles(depAsset);
                    var depFile = Manifest.GetAssetPath(depAsset);
                    AssetBundleHandler.LoadBundle(depFile);
                    CacheDepRef(depAsset, abName);
                }
            }
        }

        static void CacheDepRef(string depABName, string refABName)
        {
            if (!depRefs.ContainsKey(depABName))
            {
                depRefs.Add(depABName, new List<string>());
            }
            if (!depRefs[depABName].Contains(refABName))
            {
                depRefs[depABName].Add(refABName);
            }
        }

        static void UnloadDepBundles(string abName)
        {
            var deps = Manifest.GetDependences(abName);
            if (deps != null)
            {
                foreach (var dep in deps)
                {
                    UnloadDepBundles(dep);

                    if (depRefs.ContainsKey(dep))
                    {
                        depRefs[dep].Remove(abName);
                        if (depRefs[dep].Count > 0)
                        {
                            continue;
                        }
                        depRefs.Remove(dep);
                    }

                    var depFile = Manifest.GetAssetPath(dep);
                    AssetBundleHandler.UnloadCacheAB(depFile);
                }
            }
        }
        #endregion

        #region Async
        public static IEnumerator LoadBundleAsync(string abName, System.Action<AssetBundle> finished)
        {
            yield return LoadDepBundlesAsync(abName);

            var abFile = Manifest.GetAssetPath(abName);
            yield return AssetBundleHandler.LoadBundleAsync(abFile, finished);
        }

        static IEnumerator LoadDepBundlesAsync(string abName)
        {
            var deps = Manifest.GetDependences(abName);
            if (deps != null)
            {
                foreach (var dep in deps)
                {
                    yield return LoadDepBundlesAsync(dep);

                    var depFile = Manifest.GetAssetPath(dep);
                    yield return AssetBundleHandler.LoadBundleAsync(depFile);
                    CacheDepRef(dep, abName);
                }
            }
        }

        public static IEnumerator LoadAssetAsync<T>(string abName, string assetName, System.Action<T> finished) where T : Object
        {
            yield return LoadDepBundlesAsync(abName);

            var abFile = Manifest.GetAssetPath(abName);
            yield return AssetBundleHandler.LoadAssetAsync(abFile, assetName, finished);
        }

#if UNITY_2021
        public static IEnumerator UnloadCacheABAsync(string abName, System.Action finished = null)
        {
            if (!depRefs.ContainsKey(abName))
            {
                var abFile = Manifest.GetAssetPath(abName);
                yield return AssetBundleHandler.UnloadCacheABAsync(abFile);
                yield return UnloadDepBundlesAsync(abName);
            }
            finished?.Invoke();
        }

        public static IEnumerator UnloadCacheABsAsync(System.Action finished = null)
        {
            depRefs.Clear();
            yield return AssetBundleHandler.UnloadCacheABsAsync(finished);
        }

        static IEnumerator UnloadDepBundlesAsync(string abName)
        {
            var deps = Manifest.GetDependences(abName);
            if (deps != null)
            {
                foreach (var dep in deps)
                {
                    yield return UnloadDepBundlesAsync(dep);

                    if (depRefs.ContainsKey(dep))
                    {
                        depRefs[dep].Remove(abName);
                        if (depRefs[dep].Count > 0)
                        {
                            continue;
                        }
                        depRefs.Remove(dep);
                    }

                    var abFile = Manifest.GetAssetPath(dep);
                    yield return AssetBundleHandler.UnloadCacheABAsync(abFile);
                }
            }
        }
#endif
        #endregion
    }
}