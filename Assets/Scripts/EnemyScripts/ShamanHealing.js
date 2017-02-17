#pragma strict

import System.Collections.Generic;//needed for List

var activeEnemies:List.<GameObject>;//Refreshed whenever heal is called

private var myX:float;
private var myZ:float;
public var healingRadius:float;
public var healingCooldownSeconds:float;
public var healingPower:double;
internal var excel : ExcelReader;
function Start ()
{
	excel = GameObject.Find("Scripts").GetComponent("ExcelReader");
	//Default values that won't clobber inspector values
	
	healingCooldownSeconds = excel.shamanCooldown;
	if (healingCooldownSeconds <= 0)
	{
		healingCooldownSeconds= 6;
	}
	healingPower = excel.shamanStrength;
	
	if (healingPower <=0)
	{
		healingPower = 20;
	}
	healingRadius = excel.shamanRadius;
	
	if (healingRadius <=0)
	{
		healingRadius = 3;
	}
	InvokeRepeating("Heal",healingCooldownSeconds,healingCooldownSeconds);
}
//basic helper function
function Distance(otherX:float,otherZ:float)
{
	return Mathf.Sqrt(Mathf.Pow(otherX-myX,2) + Mathf.Pow(otherZ-myZ,2));
}

function Heal ()
{	
	myX = gameObject.transform.position.x;
	myZ = gameObject.transform.position.z;
	activeEnemies = GamePlay.activeEnemies;
	
	for (var i = 0; i < activeEnemies.Count;i++)
	{
		if (Distance(activeEnemies[i].transform.position.x,activeEnemies[i].transform.position.z) <=healingRadius)
		{
			activeEnemies[i].GetComponent(MonsterScript).hp += healingPower;//heal em up
			if (activeEnemies[i].GetComponent(MonsterScript).hp > activeEnemies[i].GetComponent(MonsterScript).maxHp)
			{
				//cap at max HP
				activeEnemies[i].GetComponent(MonsterScript).hp = activeEnemies[i].GetComponent(MonsterScript).maxHp;
			}
		
		}
		
	}
	
}



function Update ()
{
	
}
