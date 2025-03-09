using System;
using System.Collections.Generic;
using UnityEngine;

namespace Internal
{
    [Serializable]
    public class AssetsDictionary : SerializableDictionary<string, List<EnvAsset>>
    {
    }
}