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
        public static IAssetManifest Manifest { set; get; } = new AssetManifest();
        static Dictionary<string, List<string>> depRefs = new Dictionary<string, List<string>>();
        #region Sync
        public static AssetBundle LoadBundle(string abFile)
        {
            LoadDepBundles(abFile);
            return AssetBundleHandler.LoadBundle(abFile);
        }

        public static T LoadAsset<T>(string abFile, string assetName) where T : Object
        {
            LoadDepBundles(abFile);
            return AssetBundleHandler.LoadAsset<T>(abFile, assetName);
        }

        public static void UnloadCacheAB(string abFile)
        {
            if (!depRefs.ContainsKey(abFile))
            {
                AssetBundleHandler.UnloadCacheAB(abFile);
                UnloadDepBundles(abFile);
            }
        }

        public static void UnloadCacheABs()
        {
            depRefs.Clear();
            AssetBundleHandler.UnloadCacheABs();
        }

        static void LoadDepBundles(string abFile)
        {
            var depFiles = Manifest.GetDependences(abFile);
            if (depFiles != null)
            {
                foreach (var depFile in depFiles)
                {
                    LoadDepBundles(depFile);
                    AssetBundleHandler.LoadBundle(depFile);
                    CacheDepRef(depFile, abFile);
                }
            }
        }

        static void CacheDepRef(string depABFile, string refABFile)
        {
            if (!depRefs.ContainsKey(depABFile))
            {
                depRefs.Add(depABFile, new List<string>());
            }
            if (!depRefs[depABFile].Contains(refABFile))
            {
                depRefs[depABFile].Add(refABFile);
            }
        }

        static void UnloadDepBundles(string abFile)
        {
            var deps = Manifest.GetDependences(abFile);
            if (deps != null)
            {
                foreach (var dep in deps)
                {
                    UnloadDepBundles(dep);

                    if (depRefs.ContainsKey(dep))
                    {
                        depRefs[dep].Remove(abFile);
                        if (depRefs[dep].Count > 0)
                        {
                            continue;
                        }
                        depRefs.Remove(dep);
                    }
                    AssetBundleHandler.UnloadCacheAB(dep);
                }
            }
        }
        #endregion

        #region Async
        public static IEnumerator LoadBundleAsync(string abFile, System.Action<AssetBundle> finished)
        {
            yield return LoadDepBundlesAsync(abFile);
            yield return AssetBundleHandler.LoadBundleAsync(abFile, finished);
        }

        static IEnumerator LoadDepBundlesAsync(string abFile)
        {
            var deps = Manifest.GetDependences(abFile);
            if (deps != null)
            {
                foreach (var dep in deps)
                {
                    yield return LoadDepBundlesAsync(dep);
                    yield return AssetBundleHandler.LoadBundleAsync(dep);
                    CacheDepRef(dep, abFile);
                }
            }
        }

        public static IEnumerator LoadAssetAsync<T>(string abFile, string assetName, System.Action<T> finished) where T : Object
        {
            yield return LoadDepBundlesAsync(abFile);
            yield return AssetBundleHandler.LoadAssetAsync(abFile, assetName, finished);
        }

#if UNITY_2021
        public static IEnumerator UnloadCacheABAsync(string abFile, System.Action finished = null)
        {
            if (!depRefs.ContainsKey(abFile))
            {
                yield return AssetBundleHandler.UnloadCacheABAsync(abFile);
                yield return UnloadDepBundlesAsync(abFile);
            }
            finished?.Invoke();
        }

        public static IEnumerator UnloadCacheABsAsync(System.Action finished = null)
        {
            depRefs.Clear();
            yield return AssetBundleHandler.UnloadCacheABsAsync(finished);
        }

        static IEnumerator UnloadDepBundlesAsync(string abFile)
        {
            var deps = Manifest.GetDependences(abFile);
            if (deps != null)
            {
                foreach (var dep in deps)
                {
                    yield return UnloadDepBundlesAsync(dep);

                    if (depRefs.ContainsKey(dep))
                    {
                        depRefs[dep].Remove(abFile);
                        if (depRefs[dep].Count > 0)
                        {
                            continue;
                        }
                        depRefs.Remove(dep);
                    }
                    yield return AssetBundleHandler.UnloadCacheABAsync(dep);
                }
            }
        }
#endif
        #endregion
    }
}