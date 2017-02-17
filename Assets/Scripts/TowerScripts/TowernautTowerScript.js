#pragma strict

//variable to store the current level of upgrades from 0 to 4
 var level:int = 0;
//variables to store the properties of the tower
 var damage: double;
 var range: double;
 var rateOfFire: double;
 var cost: double;
 var healTime: double;
 var splashArea: double;
 public var flying:boolean;
 
//not being used right now
internal var firePauseTime:int = 0;
internal var rotationSpeed:double = 5;

//variables used in calculation
internal var desiredRotation: Quaternion;
internal var nextFireTime: float;
internal var nextMoveTime: float;
internal var colliderList: ArrayList;

var myProjectile: GameObject;
var myTarget: Transform;
var nozzles: Transform[];
var turretSphere: Transform;

//health bar GUI
 var cameraObj: Camera;
 var healthBarWidth: int = 40;
 var healthBarHeight: int = 20;
 var healthBarPosition: Vector3;
 var healthBarTexture: Texture;
var speed: float = 1; // projectile speed
var dist: float = 0; // spawn point distance from the turret

//get the GunTowerFireScript component on the projectile
internal var towernautTowerFireScript: TowernautTowerFireScript;

//variable to store the placement on the grid
internal var gridObj: GameObject;

internal var excel : ExcelReader;

internal var stats : TowerStats;
function Awake()
{
	//Get the ExcelReader Script
	excel = GameObject.Find("Scripts").GetComponent("ExcelReader");
	stats = this.GetComponent("TowerStats");
}

function Start () {
	//load the parameters from the excelreader based on the level
	loadTowerParameters();
	stats.online = true;
	cameraObj = excel.cameraObj;
	//assign the towerRange to the sphere collider radius
	this.GetComponent(SphereCollider).radius = 2 * range;
	nextFireTime = Time.time + rateOfFire;
	colliderList = new ArrayList();
}
function Regenerate ()
{
	if (!stats.online)
	{
		stats.hp += stats.maxHp/healTime;
	}
	
	if (stats.hp >= stats.maxHp)
	{
		stats.hp = stats.maxHp;
		stats.online = true;
		gameObject.transform.localScale = Vector3(0.5,0.5,0.5);
		CancelInvoke("Regenerate");
	
	}

}

function Update () {
	if(stats.hp <= 0 && stats.online)
	{ 
		//gridObj.tag = excel.PLANE_HOVER;
		//Destroy(gameObject);
		stats.hp = 0;
		gameObject.transform.localScale = Vector3(0.2,0.2,0.2);
		stats.online = false;
		InvokeRepeating("Regenerate",1,1);
	}

	
	if (excel.selectedEnemy != null)
	{
		var distance: float = Vector3.Distance(this.transform.position, excel.selectedEnemy.transform.position);
		if (distance <= range)
		{
			myTarget = excel.selectedEnemy.transform;
		}
	}	
	else if(colliderList && colliderList.Count > 0)
	{
		if(colliderList[0])
		{
			var gameObjectTemp:GameObject = colliderList[0];
			myTarget = gameObjectTemp.transform;
		}
		else
		{
			colliderList.RemoveAt(0);
		}
	}
	else
	{
		myTarget = null;
	}
	
	if(myTarget != null)
	{
		if(Time.time > nextMoveTime && stats.online)
		{
			getRotation(myTarget.position);
			turretSphere.rotation = Quaternion.Lerp(turretSphere.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
		}
		if(Time.time >= nextFireTime)
		{
			if (stats.online)
			{
				for( nozzle in nozzles )
				{		
					//assign the level to the projectile
					towernautTowerFireScript = myProjectile.GetComponent("TowernautTowerFireScript");
					towernautTowerFireScript.level = level;	
					towernautTowerFireScript.flying = flying;		
					var createdObj:GameObject = Instantiate(myProjectile, nozzle.position, nozzle.rotation);
					var childObject:GameObject = createdObj.transform.gameObject.Find("SphereCollider");
					var childScript:SplashCollider = childObject.GetComponent("SplashCollider");
					childScript.level = level;
				}
			}
			nextFireTime = Time.time + rateOfFire;
			nextMoveTime = Time.time + firePauseTime;
		}	
	}
}

function OnTriggerEnter(intruder: Collider)
{
	
	if(intruder.gameObject.tag == "Enemy")
	{
		var other:MonsterScript = intruder.gameObject.GetComponent(MonsterScript);
		//only aim at targets we can hit
		if (other.flying == 0 || flying == true)
		{
			//myTarget = intruder.gameObject.transform;
			colliderList.Add(intruder.gameObject);
		}
	}
}

function OnTriggerExit(intruder: Collider)
{
	//myTarget = null;
	colliderList.Remove(intruder.gameObject);
}

function getRotation(targetPos: Vector3)
{
	//CalculateAimError();
	//var aimPoint = Vector3(targetPos.x+aimError, targetPos.y+aimError, targetPos.z+aimError);
	//aimPoint = aimPoint - transform.position;
	var aimPoint = targetPos - transform.position;
//	print(aimPoint);
	desiredRotation = Quaternion.LookRotation(aimPoint);
}

function loadTowerParameters()
{
	damage = excel.towernautTowerArray[level, excel.DAMAGE];
	stats.hp = excel.towernautTowerArray[level, excel.HP];
	stats.armor = excel.towernautTowerArray[level, excel.ARMOR];
	range = excel.towernautTowerArray[level, excel.RANGE];
	rateOfFire = excel.towernautTowerArray[level, excel.RATE_OF_FIRE];
	cost = excel.towernautTowerArray[level, excel.COST];
	stats.maxHp = stats.hp;
	splashArea = excel.towernautTowerArray[level, excel.ARMOR_PENETRATION];
	healTime = excel.towernautTowerArray[level,excel.HEAL_TIME];
	flying = excel.towernautTowerFlight;
}

function causeDamage(damageamount:double)
{
	if(stats.online)
	{
		if(damageamount > stats.armor)
		{
			stats.hp-=(damageamount - stats.armor);
		}
		if(stats.hp <= 0.0f)
		{
			stats.hp = 0.0f;
			//this.transform.FindChild("Cube").renderer.material.color = Color.gray;
		}
	}
}
function OnGUI()
{
	var enemyPosition = cameraObj.WorldToScreenPoint(this.transform.position);
	healthBarPosition.y = Screen.height - enemyPosition.y - 30;
	healthBarPosition.x = enemyPosition.x - healthBarWidth/2;
	var barPercentage: float = stats.hp * healthBarWidth/ stats.maxHp;
	GUI.DrawTexture(Rect(healthBarPosition.x, healthBarPosition.y, barPercentage, healthBarHeight), healthBarTexture, ScaleMode.ScaleToFit, true, 0);
	GUI.Label (Rect (healthBarPosition.x, healthBarPosition.y - 20, 150, 20), "Towernaut Tower:"+level);
}