#region Copyright
// <copyright file="Heal.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 04/08/2016, 2:38 PM </date>
#endregion
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Heal : EnemyAbility
{
	List<Enemy> activeEnemies = new List<Enemy> ();
//Refreshed whenever heal is called
    
	Vector3 myLocation;
    
	public float radius = 0f;
	public float strength = 0f;

	protected override void InitializeAbility ()
	{
		cooldown = enemy.baseEnemy.GetSpecialAbilityFloat ("cooldown");
		if (cooldown <= 0) {
			cooldown = 6f;
		}
        
		strength = enemy.baseEnemy.GetSpecialAbilityFloat ("power");
		if (strength <= 0) {
			strength = 20;
		}
        
		radius = enemy.baseEnemy.GetSpecialAbilityFloat ("radius");
		if (radius <= 0) {
			radius = 3;
		}
	}

	protected override void ExecuteAbility ()
	{
		UpdatePosition ();
		activeEnemies = GamePlay.instance.activeEnemies;
        
		for (var i = 0; i < activeEnemies.Count; i++) {
			if (activeEnemies [i].gameObject != this.gameObject) {//don't shield self
				if (Distance (activeEnemies [i].transform.position.x, activeEnemies [i].transform.position.z) <= radius) {
					activeEnemies [i].properties.active.hp += strength;
				}
			}
		}
	}
}
