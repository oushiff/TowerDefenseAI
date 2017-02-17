#region Copyright
// <copyright file="BuildTowerButton.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/06/2016, 12:14 AM </date>
#endregion
using UnityEngine.EventSystems;

public class BuildTowerButton : TowerButton
{
    public BuildTowerUI buildTowerUi;

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (this.buildTowerUi != null)
            this.buildTowerUi.SelectTower(towerIndex);
    }
}
