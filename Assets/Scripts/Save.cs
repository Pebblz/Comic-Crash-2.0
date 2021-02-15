using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable()]
public class Save
{
    public int CoinCount;
    public int MainCollectibleCount;

    public Save(int coinCount, int mainCollectibleCount)
    {
        CoinCount = coinCount;
        MainCollectibleCount = mainCollectibleCount;
    }
    public Save() : this(0, 0) { }
}
