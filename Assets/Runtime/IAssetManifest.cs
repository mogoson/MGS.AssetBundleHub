/*************************************************************************
 *  Copyright © 2023 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  IAssetManifest.cs
 *  Description  :  
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0
 *  Date         :  7/23/2023
 *  Description  :  Initial development version.
 *************************************************************************/

using System.Collections.Generic;

namespace MGS.AssetBundles
{
    public interface IAssetManifest
    {
        string AssetsPath { set; get; }

        IEnumerable<string> GetDependences(string abFile);
    }
}