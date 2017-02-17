#region Copyright
// <copyright file="ElectricalTowerFire.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/09/2016, 10:54 PM </date>
#endregion
using UnityEngine;
using System.Collections;

public class ElectricalTowerFire : TowerFire {

	protected override void Awake()
	{
		type = "Electrical";
		base.Awake();
	}

	protected override void HitEnemy(Enemy enemy, bool destroyProjectile){
		if (enemy.isValidTarget (flying)) {
			base.HitEnemy (enemy, false);
			
			if (enemy != null){
			    float duration = tower.EvaluateSpecialAbility(GameData.GameProperies.DURATION);
				if (duration > 0) {
					float radius = tower.EvaluateSpecialAbility (GameData.GameProperies.RANGE, true);

					// Snake-Charmer Tower: slow down all enemies in range
					if (level == 3)
					{
						StunEnemy(enemy, duration);
					}
					// Snanke Revival Tower: imobilize single enemy
					else if (level == 4)
					{
						Collider[] enemies = Physics.OverlapSphere(enemy.transform.position, radius);
						for (int i = 0 ; i < enemies.Length ; i++)
						{
							if (enemies[i].CompareTag("Enemy"))
							{
								Enemy closeEnemy = enemies[i].GetComponent<Enemy>();
								if (closeEnemy != null)
									StunEnemy(closeEnemy, duration);
							}
						}
					}
				}
			}
			
			Destroy (gameObject);
		}
	}

	protected void StunEnemy(Enemy enemy, float duration)
	{
		Enemy.EnemyProperties stunEnemy = new Enemy.EnemyProperties();
		stunEnemy.stun = true;
		enemy.properties.AddProperties(stunEnemy, duration);
	}
}
