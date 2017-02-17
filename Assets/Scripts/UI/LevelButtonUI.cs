#region Copyright
// <copyright file="LevelButtonUI.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 04/20/2016, 3:59 PM </date>
#endregion
using UnityEngine;
using System.Collections;

public class LevelButtonUI : MonoBehaviour
{
    public LevelsMenu levelMenu;
    public int level;

    public void LoadLevel()
    {
        levelMenu.SelectLevel(level);
    }
}
