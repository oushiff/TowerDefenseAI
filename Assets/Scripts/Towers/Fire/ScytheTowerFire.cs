#region Copyright
// <copyright file="ScytheTowerFire.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/09/2016, 1:40 AM </date>
#endregion
using UnityEngine;
using System.Collections;

public class ScytheTowerFire : TowerFire {

    protected override void Awake()
    {
        type = "Scythe";
        base.Awake();
    }
    
    protected override void HitEnemy(Enemy enemy, bool destroyProjectile){
        if (enemy.isValidTarget (flying)) {
            // If the tower gains extra coins for kills set enemy property
            if ((tower as ScytheTower).extraGain > 0)
            {
                Enemy.EnemyProperties doubleGain = new Enemy.EnemyProperties();
                doubleGain.coins = ((tower as ScytheTower).extraGain - 1) * enemy.properties.active.coins;
                enemy.properties.AddProperties(doubleGain, Time.fixedDeltaTime);
            }
            base.HitEnemy (enemy, false);
            
            if (enemy != null) {
                float effect = tower.EvaluateSpecialAbility (GameData.GameProperies.DAMAGE);
                if (effect > 0) {
                    
                    // Blade Tower: Give extra gain for enemy kills
                    if (level == 3)
                    {
                        float duration = tower.EvaluateSpecialAbility (GameData.GameProperies.DURATION, true);
                        (tower as ScytheTower).SetExtraGain(effect, duration);					
                    }
                    // Saw Tower: push back enemies
                    else if (level == 4)
                    {
                        float radius = tower.EvaluateSpecialAbility (GameData.GameProperies.RANGE, true);
                        Collider[] enemies = Physics.OverlapSphere(enemy.transform.position, radius);
                        for (int i = 0 ; i < enemies.Length ; i++)
                        {
                            if (enemies[i].CompareTag("Enemy"))
                            {
                                Enemy closeEnemy = enemies[i].GetComponent<Enemy>();
                                if (closeEnemy != null)
                                {
                                    enemy.PushBack(effect);
                                }
                            }
                        }
                    }
                }
            }
            
            Destroy (gameObject);
        }
    }
}
