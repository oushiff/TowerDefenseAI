#pragma strict

function Start () {

}

function Update () {

}

/*function OnTriggerEnter(intruder: Collider)
{
	if(intruder.transform.tag == "EnemyShell")
	{
		//read the damage values from the shell
		var monsterScript:MonsterFireScript = intruder.GetComponent("MonsterFireScript");
		var monsterDamage:double = monsterScript.damage;
		//read for the tower parameters based on the tower tag
		var parentObj:GameObject = this.transform.parent.gameObject;
		print("hit " + parentObj.tag);
		var towerArmor:double; 
		var damageValue:double;
		if(parentObj.tag == "GunTower")
		{
			var gunTowerScript:GunTowerScript = parentObj.GetComponent("GunTowerScript");
			towerArmor = gunTowerScript.armor;
			damageValue = (monsterDamage - towerArmor);
			print(damageValue);
			if(damageValue > 0)
			{
				gunTowerScript.stats.hp = gunTowerScript.stats.hp - damageValue;
			}
			Destroy(intruder.gameObject);
		}
		else if(parentObj.tag == "CanonTower")
		{
			var canonTowerScript:CannonTowerScript = parentObj.GetComponent("CanonTowerScript");
			towerArmor = canonTowerScript.armor;
			damageValue = (monsterDamage - towerArmor);
			print(damageValue);
			if(damageValue > 0)
			{
				canonTowerScript.stats.hp = canonTowerScript.stats.hp - damageValue;
			}
			Destroy(intruder.gameObject);
		}
		else if(parentObj.tag == "ThresherTower")
		{
			var thresherTowerScript:ThresherTowerScript = parentObj.GetComponent("ThresherTowerScript");
			towerArmor = thresherTowerScript.armor;
			damageValue = (monsterDamage - towerArmor);
			//print(damageValue);
			if(damageValue > 0)
			{
				thresherTowerScript.hp = thresherTowerScript.hp - damageValue;
			}
			Destroy(intruder.gameObject);
		}
	}
}*/