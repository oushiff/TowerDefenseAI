#region Copyright
// <copyright file="GunTowerFire.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/04/2016, 8:14 AM </date>
#endregion
using UnityEngine;
using System.Collections;

public class GunTowerFire : TowerFire {
    protected override void Awake()
    {
        type = "Gun";
        base.Awake();
    }
}