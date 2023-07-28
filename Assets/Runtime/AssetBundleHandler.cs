/*************************************************************************
 *  Copyright © 2023 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  AssetBundleHandler.cs
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
    public sealed class AssetBundleHandler
    {
        #region Cache
        class Cache
        {
            internal AssetBundle assetBundle;
            internal Dictionary<string, Object> assets = new Dictionary<string, Object>();
        }
        static Dictionary<string, Cache> abCaches = new Dictionary<string, Cache>();
        #endregion

        #region Sync
        public static AssetBundle LoadBundle(string abFile)
        {
            var abKey = ContentToKey(abFile);
            var ab = FindFromCache(abKey);
            if (ab == null)
            {
                ab = AssetBundle.LoadFromFile(abFile);
                if (ab != null)
                {
                    AddToCache(abKey, ab);
                }
            }
            return ab;
        }

        public static T LoadAsset<T>(string abFile, string assetName) where T : Object
        {
            var abKey = ContentToKey(abFile);
            var asset = FindFromCache<T>(abKey, assetName);
            if (asset == null)
            {
                var ab = LoadBundle(abFile);
                if (ab == null)
                {
                    return null;
                }
                asset = ab.LoadAsset<T>(assetName);
                if (asset != null)
                {
                    AddToCache(abKey, assetName, asset);
                }
            }
            return asset;
        }

        public static void UnloadCacheAB(string abFile)
        {
            var abKey = ContentToKey(abFile);
            var cache = FindCache(abKey);
            if (cache != null)
            {
                cache.assetBundle.Unload(true);
                RemoveCache(abKey);
            }
        }

        public static void UnloadCacheABs()
        {
            foreach (var abInfo in abCaches.Values)
            {
                abInfo.assetBundle.Unload(true);
            }
            abCaches.Clear();
        }

        static string ContentToKey(string content)
        {
            return content.GetHashCode().ToString();
        }

        static void RequireCache(string abKey)
        {
            if (!abCaches.ContainsKey(abKey))
            {
                abCaches.Add(abKey, new Cache());
            }
        }

        static void AddToCache(string abKey, AssetBundle ab)
        {
            RequireCache(abKey);
            abCaches[abKey].assetBundle = ab;
        }

        static void AddToCache(string abKey, string assetKey, Object asset)
        {
            RequireCache(abKey);
            abCaches[abKey].assets.Add(assetKey, asset);
        }

        static Cache FindCache(string abKey)
        {
            if (abCaches.ContainsKey(abKey))
            {
                return abCaches[abKey];
            }
            return null;
        }

        static AssetBundle FindFromCache(string abKey)
        {
            var info = FindCache(abKey);
            if (info == null)
            {
                return null;
            }
            return info.assetBundle;
        }

        static T FindFromCache<T>(string abKey, string assetKey) where T : Object
        {
            var info = FindCache(abKey);
            if (info == null)
            {
                return null;
            }
            if (info.assets.ContainsKey(assetKey))
            {
                return info.assets[assetKey] as T;
            }
            return null;
        }

        static void RemoveCache(string abKey)
        {
            abCaches.Remove(abKey);
        }
        #endregion

        #region Async
        public static IEnumerator LoadBundleAsync(string abFile, System.Action<AssetBundle> finished = null)
        {
            var abKey = ContentToKey(abFile);
            var ab = FindFromCache(abKey);
            if (ab == null)
            {
                var load = AssetBundle.LoadFromFileAsync(abFile);
                yield return load;

                ab = load.assetBundle;
                if (ab != null)
                {
                    AddToCache(abKey, ab);
                }
            }
            finished?.Invoke(ab);
        }

        public static IEnumerator LoadAssetAsync<T>(string abFile, string assetName, System.Action<T> finished) where T : Object
        {
            var abKey = ContentToKey(abFile);
            var asset = FindFromCache<T>(abKey, assetName);
            if (asset == null)
            {
                AssetBundle ab = null;
                yield return LoadBundleAsync(abFile, result => ab = result);

                var load = ab.LoadAssetAsync<T>(assetName);
                yield return load;

                asset = load.asset as T;
                if (asset != null)
                {
                    AddToCache(abKey, assetName, asset);
                }
            }
            finished?.Invoke(asset);
        }

#if UNITY_2021
        public static IEnumerator UnloadCacheABAsync(string abFile, System.Action finished = null)
        {
            var abKey = ContentToKey(abFile);
            var cache = FindCache(abKey);
            if (cache != null)
            {
                yield return cache.assetBundle.UnloadAsync(true);
            }
            RemoveCache(abKey);
            finished?.Invoke();
        }

        public static IEnumerator UnloadCacheABsAsync(System.Action finished = null)
        {
            foreach (var abInfo in abCaches.Values)
            {
                yield return abInfo.assetBundle.UnloadAsync(true);
            }
            abCaches.Clear();
            finished?.Invoke();
        }
#endif
        #endregion
    }
}