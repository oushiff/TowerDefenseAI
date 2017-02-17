#region Copyright
// <copyright file="PoisonTowerFire.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/09/2016, 10:55 PM </date>
#endregion
using UnityEngine;
using System.Collections;

public class PoisonTowerFire : TowerFire
{
	public GameObject walkingBomb;

	protected override void HitEnemy(Enemy enemy, bool destroyProjectile){
		if (enemy.isValidTarget (flying)) {
			base.HitEnemy (enemy, false);
			
			if (enemy != null) {
				float reduction = tower.EvaluateSpecialAbility (GameData.GameProperies.REDUCTION);
				if (reduction > 0) {
					float duration = tower.EvaluateSpecialAbility (GameData.GameProperies.DURATION, true);
					float radius = tower.EvaluateSpecialAbility (GameData.GameProperies.RANGE, true);


					// Red-Eye Tower: slow down all enemies in range
					if (level == 3)
					{
						Collider[] enemies = Physics.OverlapSphere(enemy.transform.position, radius);
						for (int i = 0 ; i < enemies.Length ; i++)
						{
							if (enemies[i].CompareTag("Enemy"))
							{
								Enemy closeEnemy = enemies[i].GetComponent<Enemy>();
								SlowDownEnemy(closeEnemy, reduction, duration);
							}
						}
					}
					// Rot-Gut Tower: imobilize single enemy
					else if (level == 4)
					{
						GameObject poisonBomb = Instantiate(walkingBomb) as GameObject;
						poisonBomb.transform.parent = enemy.transform.parent;
						poisonBomb.transform.position = enemy.transform.position;

						PoisonTowerBomb bomb = poisonBomb.GetComponent<PoisonTowerBomb>();
						bomb.Initialize(radius, reduction, duration);
					}
				}
			}

			Destroy (gameObject);
		}
	}

	protected void SlowDownEnemy(Enemy enemy, float reduction, float duration)
	{
		Enemy.EnemyProperties slowEnemy = new Enemy.EnemyProperties();
		slowEnemy.speed = -reduction * slowEnemy.originalSpeed;
		enemy.properties.AddProperties(slowEnemy, duration);
	}
}
