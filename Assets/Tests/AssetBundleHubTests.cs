using MGS.AssetBundles;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

public class AssetBundleHubTests
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        AssetBundleHub.AssetsRoot = $"{Application.streamingAssetsPath}/AssetBundles";
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        AssetBundleHub.UnloadCacheABs();
    }

    [Test]
    public void LoadBundleTest()
    {
        var abName = "spherecube.ab";
        var cube = AssetBundleHub.LoadBundle(abName);//Use AB name, and will load deps.
        Assert.IsNotNull(cube);

        var depName = "cube.ab";
        var abs = AssetBundle.GetAllLoadedAssetBundles();
        var isFind = false;
        foreach (var ab in abs)
        {
            if (ab.name.Contains(depName))
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
        var abName = $"{assetName}.ab";
        var asset = AssetBundleHub.LoadAsset<GameObject>(abName, assetName);
        Assert.IsNotNull(asset);

        var depName = "cube.ab";
        var abs = AssetBundle.GetAllLoadedAssetBundles();
        var isFind = false;
        foreach (var ab in abs)
        {
            if (ab.name.Contains(depName))
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
        AssetBundleHub.UnloadCacheAB(depName);//Can not unlod, cube ref by spherecube.
        var abs = AssetBundle.GetAllLoadedAssetBundles();
        var isFind = false;
        foreach (var ab in abs)
        {
            if (ab.name.Contains(depName))
            {
                isFind = true;
                break;
            }
        }
        if (!isFind)
        {
            Assert.Fail();
        }


        depName = "spherecube.ab";
        AssetBundleHub.UnloadCacheAB(depName);
        var deps = AssetBundleHub.Manifest.GetDependences(depName).ToList();
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
        var abName = "spherecube.ab";
        AssetBundle bundle = null;
        yield return AssetBundleHub.LoadBundleAsync(abName, ab => bundle = ab);
        Assert.IsNotNull(bundle);

        var depName = "cube.ab";
        var abs = AssetBundle.GetAllLoadedAssetBundles();
        var isFind = false;
        foreach (var ab in abs)
        {
            if (ab.name.Contains(depName))
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
        var abName = $"{assetName}.ab";
        GameObject asset = null;
        yield return AssetBundleHub.LoadAssetAsync<GameObject>(abName, assetName, go => asset = go);
        Assert.IsNotNull(asset);

        var depName = "cube.ab";
        var abs = AssetBundle.GetAllLoadedAssetBundles();
        var isFind = false;
        foreach (var ab in abs)
        {
            if (ab.name.Contains(depName))
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
        yield return AssetBundleHub.UnloadCacheABAsync(depName);//Can not unlod, cube ref by spherecube.
        var abs = AssetBundle.GetAllLoadedAssetBundles();
        var isFind = false;
        foreach (var ab in abs)
        {
            if (ab.name.Contains(depName))
            {
                isFind = true;
                break;
            }
        }
        if (!isFind)
        {
            Assert.Fail();
        }

        depName = "spherecube.ab";
        yield return AssetBundleHub.UnloadCacheABAsync(depName);
        var deps = AssetBundleHub.Manifest.GetDependences(depName).ToList();
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