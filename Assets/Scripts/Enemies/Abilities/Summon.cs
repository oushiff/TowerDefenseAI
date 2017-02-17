#region Copyright
// <copyright file="Summon.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/07/2016, 3:44 PM </date>
#endregion
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Summon : EnemyAbility {
	public GameObject enemyPrefab;
	private Enemy myScript;
	private Enemy newScript;
	private int enemyType = -1;
	public List<GameObject> types;
	
	protected override void InitializeAbility (){
		cooldown = enemy.baseEnemy.GetSpecialAbilityFloat("cooldown"); ;
		//Default values that won't clobber inspector values
		if (cooldown <= 0)
		{
			cooldown= 6;
		}
		enemyType = enemy.baseEnemy.GetSpecialAbilityInt("type");
	    MonsterData.Level monster = GameData.instance.GetMonster(enemyType);
	    if (monster != null){
	        enemyPrefab = monster.GetPrefab();

	        if (enemyPrefab == null){
	            monster = GameData.instance.GetMonster(0);

                if (monster != null)
                    enemyPrefab = monster.GetPrefab();

	            if (enemyPrefab == null){
	                Destroy(this);
                    Debug.LogWarning("Summoned enemy not found! Removing Summon ability");
                }
	        }
	    }
	    myScript = GetComponent<Enemy> ();
	}

	protected override void ExecuteAbility(){
		UpdatePosition ();
		
		GameObject newEnemyGO = Instantiate(enemyPrefab) as GameObject;
		Vector3 newEnemyPosition = new Vector3 (position.x, newEnemyGO.transform.position.y, position.y);
		newEnemyGO.transform.position = newEnemyPosition;

		Enemy newEnemy = newEnemyGO.GetComponent<Enemy>();
		newEnemy.nextPointIndex = myScript.nextPointIndex;
		
		GamePlay.instance.activeEnemies.Add(newEnemy);
	}
}
