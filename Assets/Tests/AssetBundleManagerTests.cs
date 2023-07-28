using MGS.AssetBundles;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class AssetBundleManagerTests
{
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        AssetBundleManager.UnloadCacheABs();
    }

    [Test]
    public void LoadBundleTest()
    {
        var abName = "sphere.ab";
        var ab = AssetBundleManager.LoadBundle(abName);//Just use file name, and will load deps.
        Assert.IsNotNull(ab);
    }

    [Test]
    public void LoadAssetTest()
    {
        var abName = "sphere.ab";
        var assetName = "sphere";
        var asset = AssetBundleManager.LoadAsset<GameObject>(abName, assetName);
        Assert.IsNotNull(asset);
    }

    [Test]
    public void UnloadCacheABTest()
    {
        LoadBundleTest();

        var abName = "sphere.ab";
        AssetBundleManager.UnloadCacheAB(abName);

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
        AssetBundleManager.UnloadCacheABs();

        var abs = AssetBundle.GetAllLoadedAssetBundles();
        foreach (var ab in abs)
        {
            Assert.Fail();
        }
    }

    [UnityTest]
    public IEnumerator LoadBundleAsyncTest()
    {
        var abName = "sphere.ab";
        AssetBundle bundle = null;
        yield return AssetBundleManager.LoadBundleAsync(abName, ab => bundle = ab);
        Assert.IsNotNull(bundle);
    }

    [UnityTest]
    public IEnumerator LoadAssetAsyncTest()
    {
        var abName = "sphere.ab";
        var assetName = "sphere";
        GameObject asset = null;
        yield return AssetBundleManager.LoadAssetAsync<GameObject>(abName, assetName, go => asset = go);
        Assert.IsNotNull(asset);
    }

#if UNITY_2021
    [UnityTest]
    public IEnumerator UnloadCacheABAsyncTest()
    {
        LoadBundleTest();

        var abName = "sphere.ab";
        yield return AssetBundleManager.UnloadCacheABAsync(abName);

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
        yield return AssetBundleManager.UnloadCacheABsAsync();

        var abs = AssetBundle.GetAllLoadedAssetBundles();
        foreach (var ab in abs)
        {
            Assert.Fail();
        }
    }
#endif
}