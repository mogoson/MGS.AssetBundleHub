using MGS.AssetBundles;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class AssetBundleHandlerTests
{
    string assetsPath;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        assetsPath = $"{Application.streamingAssetsPath}/AssetBundles/Windows";
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        AssetBundleHandler.UnloadCacheABs();
    }

    [Test]
    public void LoadBundleTest()
    {
        var abFile = $"{assetsPath}/cube.ab";
        var ab = AssetBundleHandler.LoadBundle(abFile);//Use file path, just load self.
        Assert.IsNotNull(ab);
    }

    [Test]
    public void LoadAssetTest()
    {
        var assetName = "cube";
        var abFile = $"{assetsPath}/{assetName}.ab";
        var asset = AssetBundleHandler.LoadAsset<GameObject>(abFile, assetName);
        Assert.IsNotNull(asset);
    }

    [Test]
    public void UnloadCacheABTest()
    {
        LoadBundleTest();

        var abName = "cube.ab";
        var abFile = $"{assetsPath}/{abName}";
        AssetBundleHandler.UnloadCacheAB(abFile);

        var abs = AssetBundle.GetAllLoadedAssetBundles();
        foreach (var ab in abs)
        {
            if (ab.name == abName)
            {
                Assert.Fail();
            }
        }
    }

    [Test]
    public void UnloadCacheABsTest()
    {
        LoadBundleTest();
        AssetBundleHandler.UnloadCacheABs();

        var abs = AssetBundle.GetAllLoadedAssetBundles();
        foreach (var ab in abs)
        {
            Assert.Fail();
        }
    }

    [UnityTest]
    public IEnumerator LoadBundleAsyncTest()
    {
        var abFile = $"{assetsPath}/cube.ab";
        AssetBundle bundle = null;
        yield return AssetBundleHandler.LoadBundleAsync(abFile, ab => bundle = ab);
        Assert.IsNotNull(bundle);
    }

    [UnityTest]
    public IEnumerator LoadAssetAsyncTest()
    {
        var assetName = "cube";
        var abFile = $"{assetsPath}/{assetName}.ab";
        GameObject cube = null;
        yield return AssetBundleHandler.LoadAssetAsync<GameObject>(abFile, assetName, asset => cube = asset);
        Assert.IsNotNull(cube);
    }

#if UNITY_2021
    [UnityTest]
    public IEnumerator UnloadCacheABAsyncTest()
    {
        LoadBundleTest();

        var abName = "cube.ab";
        var abFile = $"{assetsPath}/{abName}";
        yield return AssetBundleHandler.UnloadCacheABAsync(abFile);

        var abs = AssetBundle.GetAllLoadedAssetBundles();
        foreach (var ab in abs)
        {
            if (ab.name == abName)
            {
                Assert.Fail();
            }
        }
    }

    [UnityTest]
    public IEnumerator UnloadCacheABsAsyncTest()
    {
        LoadBundleTest();
        yield return AssetBundleHandler.UnloadCacheABsAsync();

        var abs = AssetBundle.GetAllLoadedAssetBundles();
        foreach (var ab in abs)
        {
            Assert.Fail();
        }
    }
#endif
}