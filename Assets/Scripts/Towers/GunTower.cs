#region Copyright
// <copyright file="GunTower.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/08/2016, 5:30 PM </date>
#endregion
using UnityEngine;
using System.Collections;

public class GunTower : Tower {
	protected override void Start()
	{
		type = "Gun";
		base.Start ();
	}

	public override void Fire(int noProjectiles)
	{
		int projectiles = (int) EvaluateSpecialAbility(GameData.GameProperies.PROJECTILES);
		if (projectiles < 0) projectiles = 1;
		base.Fire (projectiles);
	}

	public override void CauseDamage(float damageAmount)
	{
		float extraDamage = EvaluateSpecialAbility(GameData.GameProperies.DAMAGE_PERCENTAGE);
		if (extraDamage > 0) 
			damageAmount *= extradamage;
		base.CauseDamage (damageAmount);
	}
}