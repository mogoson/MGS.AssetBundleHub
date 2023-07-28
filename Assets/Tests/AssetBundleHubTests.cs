using MGS.AssetBundles;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

public class AssetBundleHubTests
{
    string assetsPath;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        assetsPath = $"{Application.streamingAssetsPath}/AssetBundles/Windows";
        AssetBundleHub.Manifest.AssetsPath = assetsPath;
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        AssetBundleHub.UnloadCacheABs();
    }

    [Test]
    public void LoadBundleTest()
    {
        var abFile = $"{assetsPath}/spherecube.ab";
        var cube = AssetBundleHub.LoadBundle(abFile);//Use file path, and will load deps.
        Assert.IsNotNull(cube);

        var depName = "cube.ab";
        var abs = AssetBundle.GetAllLoadedAssetBundles();
        var isFind = false;
        foreach (var ab in abs)
        {
            if (ab.name == depName)
            {
                isFind = true;
                break;
            }
        }
        if (!isFind)
        {
            Assert.Fail();
        }
    }

    [Test]
    public void LoadAssetTest()
    {
        var assetName = "spherecube";
        var abFile = $"{assetsPath}/{assetName}.ab";
        var asset = AssetBundleHub.LoadAsset<GameObject>(abFile, assetName);
        Assert.IsNotNull(asset);

        var depName = "cube.ab";
        var abs = AssetBundle.GetAllLoadedAssetBundles();
        var isFind = false;
        foreach (var ab in abs)
        {
            if (ab.name == depName)
            {
                isFind = true;
                break;
            }
        }
        if (!isFind)
        {
            Assert.Fail();
        }
    }

    [Test]
    public void UnloadCacheABTest()
    {
        LoadBundleTest();

        var depName = "cube.ab";
        var abFile = $"{assetsPath}/{depName}";
        AssetBundleHub.UnloadCacheAB(abFile);//Can not unlod, cube ref by spherecube.
        var abs = AssetBundle.GetAllLoadedAssetBundles();
        var isFind = false;
        foreach (var ab in abs)
        {
            if (ab.name == depName)
            {
                isFind = true;
                break;
            }
        }
        if (!isFind)
        {
            Assert.Fail();
        }


        abFile = $"{assetsPath}/spherecube.ab";
        AssetBundleHub.UnloadCacheAB(abFile);
        var deps = AssetBundleHub.Manifest.GetDependences(abFile).ToList();
        abs = AssetBundle.GetAllLoadedAssetBundles();
        foreach (var ab in abs)
        {
            if (!string.IsNullOrEmpty(ab.name))
            {
                foreach (var dep in deps)
                {
                    if (dep.Contains(ab.name))
                    {
                        Assert.Fail();
                    }
                }
            }
        }
    }

    [Test]
    public void UnloadCacheABsTest()
    {
        LoadBundleTest();
        AssetBundleHub.UnloadCacheABs();

        var abs = AssetBundle.GetAllLoadedAssetBundles();
        foreach (var ab in abs)
        {
            Assert.Fail();
        }
    }

    [UnityTest]
    public IEnumerator LoadBundleAsyncTest()
    {
        var abFile = $"{assetsPath}/spherecube.ab";
        AssetBundle bundle = null;
        yield return AssetBundleHub.LoadBundleAsync(abFile, ab => bundle = ab);
        Assert.IsNotNull(bundle);

        var depName = "cube.ab";
        var abs = AssetBundle.GetAllLoadedAssetBundles();
        var isFind = false;
        foreach (var ab in abs)
        {
            if (ab.name == depName)
            {
                isFind = true;
                break;
            }
        }
        if (!isFind)
        {
            Assert.Fail();
        }
    }

    [UnityTest]
    public IEnumerator LoadAssetAsyncTest()
    {
        var assetName = "spherecube";
        var abFile = $"{assetsPath}/{assetName}.ab";
        GameObject asset = null;
        yield return AssetBundleHub.LoadAssetAsync<GameObject>(abFile, assetName, go => asset = go);
        Assert.IsNotNull(asset);

        var depName = "cube.ab";
        var abs = AssetBundle.GetAllLoadedAssetBundles();
        var isFind = false;
        foreach (var ab in abs)
        {
            if (ab.name == depName)
            {
                isFind = true;
                break;
            }
        }
        if (!isFind)
        {
            Assert.Fail();
        }
    }

#if UNITY_2021
    [UnityTest]
    public IEnumerator UnloadCacheABAsyncTest()
    {
        LoadBundleTest();

        var depName = "cube.ab";
        var abFile = $"{assetsPath}/{depName}";
        yield return AssetBundleHub.UnloadCacheABAsync(abFile);//Can not unlod, cube ref by spherecube.
        var abs = AssetBundle.GetAllLoadedAssetBundles();
        var isFind = false;
        foreach (var ab in abs)
        {
            if (ab.name == depName)
            {
                isFind = true;
                break;
            }
        }
        if (!isFind)
        {
            Assert.Fail();
        }

        abFile = $"{assetsPath}/spherecube.ab";
        yield return AssetBundleHub.UnloadCacheABAsync(abFile);
        var deps = AssetBundleHub.Manifest.GetDependences(abFile).ToList();
        abs = AssetBundle.GetAllLoadedAssetBundles();
        foreach (var ab in abs)
        {
            if (!string.IsNullOrEmpty(ab.name))
            {
                foreach (var dep in deps)
                {
                    if (dep.Contains(ab.name))
                    {
                        Assert.Fail();
                    }
                }
            }
        }
    }

    [UnityTest]
    public IEnumerator UnloadCacheABsAsyncTest()
    {
        LoadBundleTest();
        yield return AssetBundleHub.UnloadCacheABsAsync();

        var abs = AssetBundle.GetAllLoadedAssetBundles();
        foreach (var ab in abs)
        {
            Assert.Fail();
        }
    }
#endif
}