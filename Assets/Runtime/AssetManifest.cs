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
        public string AssetsPath
        {
            set
            {
                assetsPath = value;
                manifestFile = $"{assetsPath}/{Path.GetFileName(assetsPath)}";//Unity default build manifest file.
            }
            get { return assetsPath; }
        }
        string assetsPath = Application.streamingAssetsPath;
        string manifestFile;

        public IEnumerable<string> GetDependences(string abFile)
        {
            //The manifest AB will Unload after AssetBundleHandler.UnloadCacheABs or UnloadCacheABsAsync invoke.
            var manifest = AssetBundleHandler.LoadAsset<AssetBundleManifest>(manifestFile, "AssetBundleManifest");
            if (manifest == null)
            {
                return null;
            }

            var abName = Path.GetFileName(abFile);
            var depNames = manifest.GetAllDependencies(abName);
            if (depNames == null)
            {
                return null;
            }

            var depFiles = new List<string>();
            foreach (var depName in depNames)
            {
                var depFile = $"{AssetsPath}/{depName}";
                depFiles.Add(depFile);
            }

            return depFiles;
        }
    }
}