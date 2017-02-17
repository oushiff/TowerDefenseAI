#region Copyright
// <copyright file="CannonTowerFire.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/08/2016, 10:53 PM </date>
#endregion
using UnityEngine;
using System.Collections;

public class CannonTowerFire : SplashTowerFire {

	public GameObject burnParticle;

	protected override void Awake()
	{
		type = "Cannon";
		base.Awake();
	}
	
	protected override void HitEnemy(Enemy enemy, bool destroyProjectile){
		if (enemy.isValidTarget (flying)) {
			base.HitEnemy (enemy, false);

			if (enemy != null) {
				float damage = tower.EvaluateSpecialAbility(GameData.GameProperies.DAMAGE);
				if (damage > 0){
				    float duration = tower.EvaluateSpecialAbility(GameData.GameProperies.DURATION, true);
					StartCoroutine (BurnEnemy (enemy, damage, duration));
				}

			    float armor = tower.EvaluateSpecialAbility(GameData.GameProperies.REDUCTION);
				if (armor > 0) {
					float duration = tower.EvaluateSpecialAbility(GameData.GameProperies.DURATION, true);
					Enemy.EnemyProperties armorReduction = new Enemy.EnemyProperties ();
					armorReduction.armor = enemy.properties.active.armor * 0.5f;
					enemy.properties.AddProperties (armorReduction, duration);
				}
			}
		}
	}

	IEnumerator BurnEnemy (Enemy enemy, float damage, float duration)
	{
		GameObject burningInstance = Instantiate (burnParticle) as GameObject;
		burningInstance.transform.parent = enemy.transform;
		burningInstance.transform.localPosition = Vector3.zero;

		// Damage enemy for the duration of the effect
		do {
			if (enemy == null) break;
			enemy.causeDamage(damage, armorPenetration);
			yield return new WaitForSeconds(1f);
			duration--;
		} while(duration > 0);

		if (burningInstance != null)
			Destroy (burningInstance);

		Destroy (gameObject);
	}
}
