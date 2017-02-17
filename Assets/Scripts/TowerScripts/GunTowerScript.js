#pragma strict

//variable to store the current level of upgrades from 0 to 4
 var level:int = 0;
//variables to store the properties of the tower
 var damage: double;
 var range: double;
 var rateOfFire: double;
 var cost: double;
 var healTime: double;
 var armorPenetration: double;
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
 var extradamage: double;


//get the GunTowerFireScript component on the projectile
internal var gunTowerFireScript: GunTowerFireScript;

//variable to store the placement on the grid
internal var gridObj: GameObject;
internal var stats : TowerStats;
internal var excel : ExcelReader;

function Awake()
{
	//Get the ExcelReader Script
	excel = GameObject.FindGameObjectWithTag("ExcelReader").GetComponent("ExcelReader");
	stats = this.GetComponent("TowerStats");
}

function Start () {
	//load the parameters from the excelreader based on the level
	loadTowerParameters();
	stats.online = true;
	//assign the towerRange to the sphere collider radius
	this.GetComponent(SphereCollider).radius = 2 * range;
	
	cameraObj = excel.cameraObj;
	extradamage = 0;
	
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
				var projectiles = parseInt(ExcelReader.instance.EvaluateSpecialAbility("Gun", level, excel.SpecialAbilityStat.PROJECTILES, false));
				
				for (var i=0 ; i < projectiles ; i++)
				{
					for( nozzle in nozzles )
					{		
						//assign the level to the projectile
						gunTowerFireScript = myProjectile.GetComponent("GunTowerFireScript");
						gunTowerFireScript.level = level;	
						gunTowerFireScript.flying = flying;		
						gunTowerFireScript.extradamage = extradamage;
						Instantiate(myProjectile, nozzle.position, nozzle.rotation);
					}
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
	damage = excel.gunTowerArray[level, excel.DAMAGE];
	stats.hp = excel.gunTowerArray[level, excel.HP];
	stats.armor = excel.gunTowerArray[level, excel.ARMOR];
	range = excel.gunTowerArray[level, excel.RANGE];
	rateOfFire = excel.gunTowerArray[level, excel.RATE_OF_FIRE];
	cost = excel.gunTowerArray[level, excel.COST];
	armorPenetration = excel.gunTowerArray[level, excel.ARMOR_PENETRATION];
	stats.maxHp = stats.hp;
	healTime = excel.gunTowerArray[level,excel.HEAL_TIME];
	flying = excel.gunTowerFlight;
}

function causeDamage(damageamount:double)
{
	damageamount *= ExcelReader.instance.EvaluateSpecialAbility("Gun", level, excel.SpecialAbilityStat.DAMAGE, false);
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
	GUI.Label (Rect (healthBarPosition.x, healthBarPosition.y - 20, 100, 20), "Gun Tower:"+level);
}