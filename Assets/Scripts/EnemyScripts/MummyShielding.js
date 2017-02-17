#pragma strict

import System.Collections.Generic;//needed for List

var activeEnemies:List.<GameObject>;//Refreshed whenever heal is called

private var myX:float;
private var myZ:float;
public var shieldingRadius:float;
public var shieldingCooldownSeconds:float;
public var shieldStrength:double;
internal var excel : ExcelReader;
function Start ()
{
	excel = GameObject.Find("Scripts").GetComponent("ExcelReader");
	//Default values that won't clobber inspector values
	shieldingCooldownSeconds = excel.mummyCooldown;
	if (shieldingCooldownSeconds <= 0)
	{
		shieldingCooldownSeconds= 10;
	}
	shieldStrength = excel.mummyPower;
	if (shieldStrength <=0)
	{
		shieldStrength = 10;
	}
	shieldingRadius = excel.mummyRadius;
	if (shieldingRadius <= 0)
	{
		shieldingRadius = 3;
	}
	InvokeRepeating("Shield",shieldingCooldownSeconds,shieldingCooldownSeconds);
}
//basic helper function
function Distance(otherX:float,otherZ:float)
{
	return Mathf.Sqrt(Mathf.Pow(otherX-myX,2) + Mathf.Pow(otherZ-myZ,2));
}

function Shield ()
{	
	myX = gameObject.transform.position.x;
	myZ = gameObject.transform.position.z;
	activeEnemies = GamePlay.activeEnemies;
	
	for (var i = 0; i < activeEnemies.Count;i++)
	{
		if (activeEnemies[i]!=this.gameObject)//don't shield self
		{
			if (Distance(activeEnemies[i].transform.position.x,activeEnemies[i].transform.position.z) <=shieldingRadius)
			{
				if (activeEnemies[i].GetComponent(MonsterScript).shield < shieldStrength)
				{
					//raise shield up to current value
					activeEnemies[i].GetComponent(MonsterScript).shield = shieldStrength;
				}
			}
		}
	}
}



function Update ()
{
	
}
