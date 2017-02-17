#region Copyright
// <copyright file="Shield.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/08/2016, 1:33 AM </date>
#endregion
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shield : EnemyAbility {
	List<Enemy> activeEnemies = new List<Enemy>();//Refreshed whenever heal is called

	Vector3 myLocation;

	public float radius = 0f;
	public float strength = 0f;

	protected override void InitializeAbility()
	{
		cooldown = enemy.baseEnemy.GetSpecialAbilityFloat("cooldown"); ;
		if (cooldown <= 0)
		{
			cooldown = 10f;
		}
		
		strength = enemy.baseEnemy.GetSpecialAbilityFloat("power"); ;
		if (strength <=0)
		{
			strength = 10;
		}
		
		radius = enemy.baseEnemy.GetSpecialAbilityFloat("radius"); ;
		if (radius <= 0)
		{
			radius = 3;
		}
	}

	protected override void ExecuteAbility ()
	{
		UpdatePosition ();
		activeEnemies = GamePlay.instance.activeEnemies;
		
		for (var i = 0; i < activeEnemies.Count; i++)
		{
			if (activeEnemies[i].gameObject != this.gameObject)//don't shield self
			{
				if (Distance(activeEnemies[i].transform.position.x, activeEnemies[i].transform.position.z) <= radius)
				{
					if (activeEnemies[i].properties.active.shield < strength)
					{
						//raise shield up to current value
						activeEnemies[i].properties.active.shield = strength;
					}
				}
			}
		}
	}
}
