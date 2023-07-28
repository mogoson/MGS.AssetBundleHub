/*************************************************************************
 *  Copyright © 2023 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  AssetBundleSettings.cs
 *  Description  :  
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0
 *  Date         :  7/23/2023
 *  Description  :  Initial development version.
 *************************************************************************/

using System;
using UnityEditor;
using UnityEngine;

namespace MGS.AssetBundles.Editors
{
    public class AssetBundleSettings : ScriptableObject
    {
        public string output = "Assets/StreamingAssets/AssetBundles";
        public BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
        public string variant = "ab";
        public AssetBundleConfig[] assetBundles;
    }

    [Serializable]
    public class AssetBundleConfig
    {
        public string asset;
        public bool eachChildren;
    }
}