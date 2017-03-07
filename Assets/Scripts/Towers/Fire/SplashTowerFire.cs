using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SplashTowerFire : TowerFire {
	SplashFireArea splashFire;

	protected override void Awake()
	{
		base.Awake ();
		splashFire = transform.GetComponentInChildren<SplashFireArea>();
		splashFire.type = type;
	}

	protected override void OnTriggerEnter(Collider intruderCollider)
	{
		if(intruderCollider.CompareTag("Enemy"))
		{
			Enemy intruder = intruderCollider.GetComponent<Enemy>();
			//if (intruder == null)
			//	intruder = intruderCollider.transform.parent.GetComponent<Enemy>();

			if (intruder != null && intruder.isValidTarget(flying))
			{
				if (maxHits-- > 0)
				{
                    splashFire.enemiesInRange.Add(intruder.gameObject);
					List<GameObject> enemies = splashFire.enemiesInRange;
					if(enemies.Count > 0)
					{
						for( int i = 0 ; i < enemies.Count ; i++)
						{
							if(enemies[i] != null)
							{
								Enemy enemy = enemies[i].GetComponent<Enemy>();
								HitEnemy(enemy, true);
							}
						}
					}
				}
			}
		}
	}
	
	protected override void HitEnemy(Enemy enemy, bool destroyProjectile){
		if (enemy.isValidTarget(flying))
			base.HitEnemy (enemy, destroyProjectile);
	}
}
