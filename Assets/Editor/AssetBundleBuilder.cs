/*************************************************************************
 *  Copyright © 2023 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  AssetBundleBuilder.cs
 *  Description  :  
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0
 *  Date         :  7/23/2023
 *  Description  :  Initial development version.
 *************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MGS.AssetBundles.Editors
{
    public sealed class AssetBundleBuilder
    {
        public static AssetBundleManifest BuildToEachBundles(IEnumerable<Object> assets, string variant, string outputPath,
            BuildAssetBundleOptions options, BuildTarget target)
        {
            var assetPaths = FindAssetPaths(assets);
            return BuildToEachBundles(assetPaths, variant, outputPath, options, target);
        }

        public static AssetBundleManifest BuildToEachBundles(IEnumerable<string> assets, string variant, string outputPath,
            BuildAssetBundleOptions options, BuildTarget target)
        {
            RequireDirectory(outputPath);

            var buildMap = BuildAsEachBundles(assets, variant).ToArray();
            return BuildPipeline.BuildAssetBundles(outputPath, buildMap, options, target);
        }

        public static IEnumerable<AssetBundleBuild> BuildAsEachBundles(IEnumerable<string> assets, string variant)
        {
            var maps = new List<AssetBundleBuild>();
            foreach (var asset in assets)
            {
                var name = Path.GetFileNameWithoutExtension(asset);
                var map = BuildAsOneBundle(new string[] { asset }, name, variant);
                maps.Add(map);
            }
            return maps;
        }

        public static AssetBundleBuild BuildAsOneBundle(string[] assets, string name, string variant)
        {
            return new AssetBundleBuild()
            {
                assetBundleName = name,
                assetNames = assets,
                assetBundleVariant = variant
            };
        }

        public static IEnumerable<string> FindAssetPaths(IEnumerable<Object> assets)
        {
            var assetPaths = new List<string>();
            foreach (var asset in assets)
            {
                var assetPath = AssetDatabase.GetAssetPath(asset);
                assetPaths.Add(assetPath);
            }
            return assetPaths;
        }

        public static void RequireDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}