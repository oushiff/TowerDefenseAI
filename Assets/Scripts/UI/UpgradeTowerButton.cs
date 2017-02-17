#region Copyright
// <copyright file="UpgradeTowerButton.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/06/2016, 12:24 AM </date>
#endregion
using UnityEngine.EventSystems;

public class UpgradeTowerButton : TowerButton
{
    public UpgradeTowerUI upgradeTowerUi;

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (this.upgradeTowerUi != null)
            this.upgradeTowerUi.UpgradeTower(towerIndex);
    }
}
