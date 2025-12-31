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
using UnityEditor;
using UnityEngine;

namespace MGS.AssetBundles.Editors
{
    public sealed class AssetBundleEditor
    {
        #region Settings
        const string SETTINGS_PATH = "Assets/AssetBundleSettings/Editor/AssetBundleSettings.asset";
        static AssetBundleSettings settings;

        static AssetBundleEditor()
        {
            settings = RequireSettings();
        }

        static AssetBundleSettings RequireSettings()
        {
            var settings = LoadSettings();
            if (settings == null)
            {
                settings = CreateSettings();
            }
            return settings;
        }

        static AssetBundleSettings LoadSettings()
        {
            return AssetDatabase.LoadAssetAtPath<AssetBundleSettings>(SETTINGS_PATH);
        }

        static AssetBundleSettings CreateSettings()
        {
            AssetBundleBuilder.RequireDirectory(SETTINGS_PATH);
            var settings = ScriptableObject.CreateInstance<AssetBundleSettings>();
            AssetDatabase.CreateAsset(settings, SETTINGS_PATH);
            return settings;
        }
        #endregion

        #region Tool Menu
        [MenuItem("Tools/AssetBundleEditor/Settings")]
        public static void ToolFocusSettings()
        {
            settings = LoadSettings();
            Selection.activeObject = settings;
        }

        [MenuItem("Tools/AssetBundleEditor/BuildForWindows")]
        public static void ToolBuildForWindows()
        {
            BuildToEachBundles(settings, BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Tools/AssetBundleEditor/BuildForAndroid")]
        public static void ToolBuildForAndroid()
        {
            BuildToEachBundles(settings, BuildTarget.Android);
        }

        [MenuItem("Tools/AssetBundleEditor/BuildForIOS")]
        public static void ToolBuildForIOS()
        {
            BuildToEachBundles(settings, BuildTarget.iOS);
        }

        [MenuItem("Tools/AssetBundleEditor/BuildForAllPlatform")]
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
        static string GetOutputDir(string root, BuildTarget target)
        {
            return $"{root}/{GetPlatFolder(target)}/";
        }

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

        static void BuildToEachBundles(IEnumerable<Object> assets, AssetBundleSettings settings, BuildTarget target)
        {
            if (CheckSettingsIsValid(settings))
            {
                var variant = settings.variant;
                var output = GetOutputDir(settings.output, target);
                var options = settings.options;
                var root = ResolveRootPath(settings);
                AssetBundleBuilder.BuildToEachBundles(assets, variant, output, options, target, root);
                AssetDatabase.Refresh();
            }
        }

        static void BuildToEachBundles(AssetBundleSettings settings, BuildTarget target)
        {
            if (CheckSettingsIsValid(settings))
            {
                var assets = FindEachAssets(settings.assets);
                var variant = settings.variant;
                var output = GetOutputDir(settings.output, target);
                var options = settings.options;
                var root = ResolveRootPath(settings);
                AssetBundleBuilder.BuildToEachBundles(assets, variant, output, options, target, root);
                AssetDatabase.Refresh();
            }
        }

        static string ResolveRootPath(AssetBundleSettings settings)
        {
            return $"{settings.assets}/";
        }

        static bool CheckSettingsIsValid(AssetBundleSettings settings)
        {
            if (string.IsNullOrEmpty(settings.output))
            {
                DisplayDialog("Config the output in the Settings(Tool/AssetBundleEditor/Settings) first.");
                return false;
            }

            if (string.IsNullOrEmpty(settings.assets))
            {
                DisplayDialog("Config the assets in the Settings(Tool/AssetBundleEditor/Settings) first.");
                return false;
            }

            return true;
        }

        static void DisplayDialog(string message)
        {
            EditorUtility.DisplayDialog("Asset Bundle Editor", message, "OK");
        }

        static IEnumerable<string> FindEachAssets(string assetRoot)
        {
            var assets = new List<string>();
            var guids = AssetDatabase.FindAssets("t:Folder", new string[] { assetRoot });
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                assets.Add(assetPath);
            }
            return assets;
        }
        #endregion
    }
}