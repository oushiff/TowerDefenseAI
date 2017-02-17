#pragma strict

//variable to store the current level of upgrades from 0 to 4
 var monsterIndex:int = 0;
//variables to store the properties of the tower
 var damage: double;
 public var hp: double;
 var armor: double;
 var range: double;
 var rateOfFire: double;
 var gain: double;
 var speed: double;
 var mana: double;
 var originalSpeed:double;
 var monsterName:String = "name";
 var flying:double;
 var shield:double;
 
 //path traversal
 internal var numberOfWayPoints: int = 0;
 internal var wayPoints: Transform[];
 var nextPointIndex:int = 1;
 internal var pos: float = 0.0;
 internal var distance: float;
 
 //health bar GUI
 var cameraObj: Camera;
 public var maxHp:double;
 var healthBarWidth: int = 40;
 var healthBarHeight: int = 20;
 var healthBarPosition: Vector3;
 var healthBarTexture: Texture;
 
 var SoulWellsAttached: ArrayList;
 var scriptsObj : GameObject;
 internal var excel : ExcelReader;

function Awake()
{
	//Get the ExcelReader Script
	excel = GameObject.FindGameObjectWithTag("ExcelReader").GetComponent("ExcelReader");
}

function Attack() {
	if (hp <= maxHp / 2)
	{
		for (var i:int = 0; i < GamePlay.activeTowers.Count; i++)
		{
			var distance: float = Vector3.Distance(this.transform.position, GamePlay.activeTowers[i].transform.position);
			if (distance <= range)
			{
				speed = 0;
				Invoke("FinishAttack",rateOfFire/2);
				return;
			}
		}
	}
}

function FinishAttack()
{
	for (var i:int = 0; i < GamePlay.activeTowers.Count; i++)
	{
		var distance: float = Vector3.Distance(this.transform.position, GamePlay.activeTowers[i].transform.position);
		if (distance <= range)
		{
			//All towers are not generic tower
			//var gts:GenericTowerScriptJS = GamePlay.activeTowers[i].GetComponent("GenericTowerScriptJS");
			//gts.causeDamage(damage);
			if(GamePlay.activeTowers[i].tag == "GunTower")
			{
				var towerType1:GunTowerScript = GamePlay.activeTowers[i].GetComponent("GunTowerScript");
				towerType1.causeDamage(damage);
			}
			else if(GamePlay.activeTowers[i].tag == "CanonTower")
			{
				var towerType2:CannonTowerScript = GamePlay.activeTowers[i].GetComponent("CannonTowerScript");
				towerType2.causeDamage(damage);
			}
			else if(GamePlay.activeTowers[i].tag == "ThresherTower")
			{
				var towerType3:ThresherTowerScript = GamePlay.activeTowers[i].GetComponent("ThresherTowerScript");
				towerType3.causeDamage(damage);
			}
			else if(GamePlay.activeTowers[i].tag == "ElectricalTower")
			{
				var towerType4:ElectricalTowerScript = GamePlay.activeTowers[i].GetComponent("ElectricalTowerScript");
				towerType4.causeDamage(damage);
			}
			else if(GamePlay.activeTowers[i].tag == "ScytheTower")
			{	
				var towerType5:ScytheTowerScript = GamePlay.activeTowers[i].GetComponent("ScytheTowerScript");
				towerType5.causeDamage(damage);
			}
			else if(GamePlay.activeTowers[i].tag == "TowernautTower")
			{
				var towerType6:TowernautTowerScript = GamePlay.activeTowers[i].GetComponent("TowernautTowerScript");
				towerType6.causeDamage(damage);
			}
			else if(GamePlay.activeTowers[i].tag == "GenericTower")
			{
				var towerType7:GenericTowerScriptJS = GamePlay.activeTowers[i].GetComponent("GenericTowerScriptJS");
				towerType7.causeDamage(damage);
			}
			else if(GamePlay.activeTowers[i].tag == "SoulWell")
			{
				var towerType8:SoulTowerScript = GamePlay.activeTowers[i].GetComponent("SoulTowerScript");
				towerType8.causeDamage(damage);
			}
		}
	}
	speed = originalSpeed;
}

function Start () {
	scriptsObj = GameObject.FindGameObjectWithTag("ExcelReader");
		
	//load enemy paramaters from the excelsheet
	loadEnemyParameters();
	InvokeRepeating("Attack",rateOfFire,rateOfFire);
	//dynamic health bar
	//assuming camera name is Main Camera
	cameraObj = excel.cameraObj;
	maxHp = hp;
	shield = 0;
	numberOfWayPoints = excel.numberOfWayPoints+2;
		//print("waypoints " +numberOfWayPoints);
	//path traversal
	wayPoints = new Transform[numberOfWayPoints];
	for (var n=0; n < numberOfWayPoints; ++n) 
	{	
		wayPoints[n] = GameObject.Find("WayPoint"+(n)).transform;         
    }
    //position the object at the start of the path
	if (numberOfWayPoints > 0 && wayPoints[0] != null && nextPointIndex == 1)
	{
		this.transform.position = wayPoints[0].position;
	}
	
	SoulWellsAttached = new ArrayList();
}

function Update () {
	if(hp <= 0)
	{ 
		var moneyScript:MoneyScript = scriptsObj.GetComponent("MoneyScript");
		moneyScript.startingMoney += gain;
		GamePlay.activeEnemies.Remove(gameObject);
		notifySoulWellDied();
		Destroy(gameObject); 
	}
	
	//path traversal
	if (numberOfWayPoints > 0 && wayPoints[0]!=null)
	{
		
		distance = Vector3.Distance(transform.position, wayPoints[nextPointIndex].position);
		if (distance > 0)
		{
			transform.position = Vector3.Lerp(transform.position, wayPoints[nextPointIndex].position, Time.deltaTime* speed/distance);
		}
		if(this.transform.position == wayPoints[nextPointIndex].position && nextPointIndex != numberOfWayPoints-1)
		{
			nextPointIndex++;
			pos = 0.0;
		}
		if(this.transform.position == wayPoints[numberOfWayPoints-1].position)
		{
			GamePlay.activeEnemies.Remove(gameObject);
			Destroy(gameObject);
		}
	}
}

function OnTriggerEnter(other: Collider)
{
//	print("collision " + other.transform.tag);
	if(other.transform.tag == "GunShell")
	{
		var gunTowerFireScript: GunTowerFireScript = other.GetComponent("GunTowerFireScript");
		if (gunTowerFireScript.flying == true || flying == false)
		{
			//read the hit points from the projectile controller script on the collider object
			var gunShellDamage:double = gunTowerFireScript.damage + gunTowerFireScript.extradamage;
			var gunShellArmorPenetration:double = gunTowerFireScript.armorPenetration;
			var damageValue:double = gunShellDamage - ( armor - gunShellArmorPenetration);
			
			if (shield > 0)
			{
				if (shield >= damageValue)
				{
					shield = shield - damageValue;
					damageValue = 0;
				}
				else
				{
					damageValue = damageValue - shield;
					shield = 0;
				}
			
			}
			
			if(damageValue >0)
			{
				hp = hp - damageValue;
			}
			Destroy(other.gameObject);
		
			}
		}
	else if(other.transform.tag == "ElectricalShell")
	{
		var electricalTowerFireScript: ElectricalTowerFireScript = other.GetComponent("ElectricalTowerFireScript");
		if (electricalTowerFireScript.flying == true || flying == false)
		{
			//read the hit points from the projectile controller script on the collider object
			var electricalShellDamage:double = electricalTowerFireScript.damage + electricalTowerFireScript.extradamage;
			var electricalShellArmorPenetration:double = electricalTowerFireScript.armorPenetration;
			var newDamageValue:double = electricalShellDamage - ( armor - electricalShellArmorPenetration);
			
			
			if (shield > 0)
			{
				if (shield >= damageValue)
				{
					shield = shield - damageValue;
					damageValue = 0;
				}
				else
				{
					damageValue = damageValue - shield;
					shield = 0;
				}
			
			}
			
			hp = hp - newDamageValue;
			Destroy(other.gameObject);
		}
	}
	else if(other.transform.tag == "ThresherShell")
	{
		var thresherTowerFireScript: ThresherTowerFireScript = other.GetComponent("ThresherTowerFireScript");
		if (thresherTowerFireScript.flying == true || flying == 0)
		{
			//print("Collision with thresher");
			var thresherShellEffect:double = thresherTowerFireScript.slowEffect;
			speed = (1 - thresherShellEffect)*originalSpeed;
			Destroy(other.gameObject);
		}
	}
}

function OnGUI()
{
	var enemyPosition = cameraObj.WorldToScreenPoint(this.transform.position);
	healthBarPosition.y = Screen.height - enemyPosition.y - 30;
	healthBarPosition.x = enemyPosition.x - healthBarWidth/2;
	var barPercentage: float = hp * healthBarWidth/ maxHp;
	GUI.DrawTexture(Rect(healthBarPosition.x, healthBarPosition.y, barPercentage, healthBarHeight), healthBarTexture, ScaleMode.ScaleToFit, true, 0);
	GUI.Label (Rect (healthBarPosition.x, healthBarPosition.y - 20, 100, 20), monsterName);
}

function loadEnemyParameters()
{
	damage = excel.enemyStatsArray[monsterIndex, excel.E_DAMAGE];
	hp = excel.enemyStatsArray[monsterIndex, excel.E_HP];
	armor = excel.enemyStatsArray[monsterIndex, excel.E_ARMOR];
	range = excel.enemyStatsArray[monsterIndex, excel.E_RANGE];
	rateOfFire = excel.enemyStatsArray[monsterIndex, excel.E_RATE_OF_FIRE];
	gain = excel.enemyStatsArray[monsterIndex, excel.E_GAIN];
	speed = excel.enemyStatsArray[monsterIndex, excel.E_SPEED];
	mana = excel.enemyStatsArray[monsterIndex, excel.E_MANA];
	flying = excel.enemyStatsArray[monsterIndex, excel.E_FLYING];
	originalSpeed = speed;
	//print("Monster flying: " + this.flying);
}

function AddSoulWell(go: GameObject)
{
	if(!SoulWellsAttached.Contains(go))
	{
		SoulWellsAttached.Add(go);
	}
}

function RemoveSoulWell(go: GameObject)
{
	if(SoulWellsAttached.Contains(go))
	{
		SoulWellsAttached.Remove(go);
	}
}

function notifySoulWellDied()
{
	for(soulWell in SoulWellsAttached)
	{
		var swgo : GameObject = soulWell as GameObject;
		var soultowerscript : SoulTowerScript = swgo.GetComponent("SoulTowerScript");
		soultowerscript.notifyDied(gameObject);
	}
}

function causeDamage(amount:double)
{
    if(amount <0)
    {
        return;
    }
    var damageValue : double = amount;
			
    if (shield > 0)
    {
        if (shield >= damageValue)
        {
            shield = shield - damageValue;
            damageValue = 0;
        }
        else
        {
            damageValue = damageValue - shield;
            shield = 0;
        }
			
    }
			
    if(damageValue > 0)
    {
        hp = hp - damageValue;
    }
}