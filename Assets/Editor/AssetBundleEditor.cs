/*************************************************************************
 *  Copyright © 2023 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  AssetBundleEditor.cs
 *  Description  :  
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0
 *  Date         :  7/23/2023
 *  Description  :  Initial development version.
 *************************************************************************/

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MGS.AssetBundles.Editors
{
    public sealed class AssetBundleEditor
    {
        #region Settings
        static string settingsPath;
        static AssetBundleSettings settings;

        static AssetBundleEditor()
        {
            settingsPath = ResolveSettingsPath();
            settings = LoadSettings(settingsPath);
            if (settings == null)
            {
                settings = CreateSettings(settingsPath);
            }
        }

        static string ResolveSettingsPath()
        {
            var editorClass = $"{typeof(AssetBundleEditor).Name}.cs";
            var editorPath = AssetDatabase.GetAllAssetPaths().First(path => { return path.Contains(editorClass); });
            return editorPath.Replace(editorClass, "AssetBundleSettings.asset");
        }

        static AssetBundleSettings CreateSettings(string settingsPath)
        {
            var settings = new AssetBundleSettings();
            AssetDatabase.CreateAsset(settings, settingsPath);
            return settings;
        }

        static AssetBundleSettings LoadSettings(string settingsPath)
        {
            return AssetDatabase.LoadAssetAtPath(settingsPath, typeof(AssetBundleSettings)) as AssetBundleSettings;
        }
        #endregion

        #region Tool Menu
        [MenuItem("Tool/AssetBundleEditor/Settings")]
        public static void ToolFocusSettings()
        {
            settings = LoadSettings(settingsPath);
            Selection.activeObject = settings;
        }

        [MenuItem("Tool/AssetBundleEditor/BuildForWindows")]
        public static void ToolBuildForWindows()
        {
            BuildToEachBundles(settings, BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Tool/AssetBundleEditor/BuildForAndroid")]
        public static void ToolBuildForAndroid()
        {
            BuildToEachBundles(settings, BuildTarget.Android);
        }

        [MenuItem("Tool/AssetBundleEditor/BuildForIOS")]
        public static void ToolBuildForIOS()
        {
            BuildToEachBundles(settings, BuildTarget.iOS);
        }

        [MenuItem("Tool/AssetBundleEditor/BuildForAllPlatform")]
        public static void ToolBuildForAllPlatform()
        {
            ToolBuildForWindows();
            ToolBuildForAndroid();
            ToolBuildForIOS();
        }
        #endregion

        #region Assets Menu
        [MenuItem("Assets/AssetsBundleEditor/BuildForWindows")]
        public static void AssetsBuildForWindows()
        {
            BuildToEachBundles(Selection.objects, settings, BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Assets/AssetsBundleEditor/BuildForAndroid")]
        public static void AssetsBuildForAndroid()
        {
            BuildToEachBundles(Selection.objects, settings, BuildTarget.Android);
        }

        [MenuItem("Assets/AssetsBundleEditor/BuildForIOS")]
        public static void AssetsBuildForIOS()
        {
            BuildToEachBundles(Selection.objects, settings, BuildTarget.iOS);
        }

        [MenuItem("Assets/AssetsBundleEditor/BuildForAllPlatform")]
        public static void AssetsBuildForAllPlatform()
        {
            AssetsBuildForWindows();
            AssetsBuildForAndroid();
            AssetsBuildForIOS();
        }
        #endregion

        #region Build
        static string GetPlatFolder(BuildTarget target)
        {
            var platform = "Windows";
            switch (target)
            {
                case BuildTarget.Android:
                    platform = "Android";
                    break;

                case BuildTarget.iOS:
                    platform = "iOS";
                    break;
            }
            return platform;
        }

        static string GetOutputPath(string root, BuildTarget target)
        {
            return $"{root}/{GetPlatFolder(target)}";
        }

        static void BuildToEachBundles(IEnumerable<Object> assets, AssetBundleSettings settings, BuildTarget target)
        {
            var variant = settings.variant;
            var output = GetOutputPath(settings.output, target);
            var options = settings.options;
            AssetBundleBuilder.BuildToEachBundles(assets, variant, output, options, target);
            AssetDatabase.Refresh();
        }

        static void BuildToEachBundles(AssetBundleSettings settings, BuildTarget target)
        {
            var assets = FindEachAssets(settings.assetBundles);
            var variant = settings.variant;
            var output = GetOutputPath(settings.output, target);
            var options = settings.options;
            AssetBundleBuilder.BuildToEachBundles(assets, variant, output, options, target);
            AssetDatabase.Refresh();
        }

        static IEnumerable<string> FindEachAssets(IEnumerable<AssetBundleConfig> configs)
        {
            var allAssets = new List<string>();
            foreach (var config in configs)
            {
                var assets = FindEachAssets(config);
                allAssets.AddRange(assets);
            }
            return allAssets;
        }

        static IEnumerable<string> FindEachAssets(AssetBundleConfig config)
        {
            if (AssetDatabase.IsValidFolder(config.asset) && config.eachChildren)
            {
                var allAssets = new List<string>();
                var guids = AssetDatabase.FindAssets("*", new string[] { config.asset });
                foreach (var guid in guids)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    allAssets.Add(assetPath);
                }
                return allAssets;
            }
            return new string[] { config.asset };
        }
        #endregion
    }
}