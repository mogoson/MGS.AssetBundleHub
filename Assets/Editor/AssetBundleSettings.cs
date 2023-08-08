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

using UnityEditor;
using UnityEngine;

namespace MGS.AssetBundles.Editors
{
    public class AssetBundleSettings : ScriptableObject
    {
        public string assets = "Assets/AssetToBundle";
        public string variant = "ab";
        public BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
        public string output = "Assets/StreamingAssets/AssetBundles";
    }
}