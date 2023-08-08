/*************************************************************************
 *  Copyright © 2023 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  AssetManifest.cs
 *  Description  :  
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0
 *  Date         :  7/23/2023
 *  Description  :  Initial development version.
 *************************************************************************/

using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MGS.AssetBundles
{
    public class AssetManifest : IAssetManifest
    {
        public string AssetsRoot
        {
            set
            {
                assetsPath = value;
                manifestFile = $"{assetsPath}/{Path.GetFileName(assetsPath)}";//Unity default build manifest file.
            }
            get { return assetsPath; }
        }
        string assetsPath;
        string manifestFile;

        public IEnumerable<string> GetDependences(string abName)
        {
            var manifest = LoadManifest();
            if (manifest == null)
            {
                return null;
            }

            var assetName = GetAssetName(abName);
            return manifest.GetAllDependencies(assetName);
        }

        public string GetAssetPath(string abName)
        {
            return $"{AssetsRoot}/{GetAssetName(abName)}";
        }

        string GetAssetName(string abName)
        {
            var manifest = LoadManifest();
            if (manifest == null)
            {
                return abName;
            }

            var assets = manifest.GetAllAssetBundles();
            foreach (var asset in assets)
            {
                if (asset.Contains(abName))
                {
                    return asset;
                }
            }
            return abName;
        }

        AssetBundleManifest LoadManifest()
        {
            //The manifest AB will Unload after AssetBundleHandler.UnloadCacheABs or UnloadCacheABsAsync invoke.
            return AssetBundleHandler.LoadAsset<AssetBundleManifest>(manifestFile, "AssetBundleManifest");
        }
    }
}