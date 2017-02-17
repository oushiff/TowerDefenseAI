#pragma strict

//variable to store the current level of upgrades from 0 to 4
 var level:int = 0;
//variables to store the properties of the tower
 var damage: double;
 var hp: double;
 var armor: double;
 var range: double;
 var rateOfFire: double;
 var gain: double;
 var speed: double;
 var mana: double;
 var originalSpeed:double;
 
 //path traversal
 var numberOfWayPoints: int = 0;
 internal var wayPoints: Transform[];
 internal var nextPointIndex:int = 1;
 internal var pos: float = 0.0;
 internal var distance: float;
 
 //health bar GUI
 var cameraObj: Camera;
 var maxHp:double;
 var healthBarWidth: int = 40;
 var healthBarHeight: int = 20;
 var healthBarPosition: Vector3;
 var healthBarTexture: Texture;
 
 internal var excel : ExcelReader;

function Awake()
{
	//Get the ExcelReader Script
	excel = GameObject.Find("Scripts").GetComponent("ExcelReader");
}

function Start () {
	//load enemy paramaters from the excelsheet
	loadEnemyParameters();
	
	//dynamic health bar
	//assuming camera name is Main Camera
	cameraObj = excel.cameraObj;
	maxHp = hp;
	
	//path traversal
	wayPoints = new Transform[numberOfWayPoints];
	for (var n=0; n < numberOfWayPoints; ++n) 
	{
        wayPoints[n] = GameObject.Find("WayPoint"+(n+1)).transform;      
    }
    //position the object at the start of the path
	if (numberOfWayPoints > 0)
	{
		this.transform.position = wayPoints[0].position;
	}
}

function Update () {
	if(hp <= 0)
	{ Destroy(gameObject); }
	
	//path traversal
	if (numberOfWayPoints > 0)
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
			Destroy(gameObject);
		}
	}
}

function OnTriggerEnter(other: Collider)
{
	if(other.transform.tag == "GunShell")
	{
		//read the hit points from the projectile controller script on the collider object
		var gunTowerFireScript: GunTowerFireScript = other.GetComponent("GunTowerFireScript");
		var gunShellDamage:double = gunTowerFireScript.damage;
		var gunShellArmorPenetration:double = gunTowerFireScript.armorPenetration;
		var damageValue:double = gunShellDamage - ( armor - gunShellArmorPenetration);
		if(damageValue > 0)
		{
			hp = hp - damageValue;
		}
		Destroy(other.gameObject);
	}	
	else if(other.transform.tag == "CanonBall")
	{
		var canonTowerFireScript: CanonTowerFireScript = other.GetComponent("CanonTowerFireScript");
		var canonBallDamage:double = canonTowerFireScript.damage;
		damageValue = canonBallDamage - armor;
		if(damageValue > 0)
		{
			hp = hp - canonBallDamage;
		}
		Destroy(other.gameObject);
	}
	else if(other.transform.tag == "ThresherShell")
	{
		var thresherTowerFireScript: ThresherTowerFireScript = other.GetComponent("ThresherTowerFireScript");
		var thresherShellEffect:double = thresherTowerFireScript.slowEffect;
		speed = (1 - thresherShellEffect)*originalSpeed;
		Destroy(other.gameObject);
	}
}

function OnGUI()
{
	var enemyPosition = cameraObj.WorldToScreenPoint(this.transform.position);
	healthBarPosition.y = Screen.height - enemyPosition.y - 30;
	healthBarPosition.x = enemyPosition.x - healthBarWidth/2;
	var barPercentage: float = hp * healthBarWidth/ maxHp;
	GUI.DrawTexture(Rect(healthBarPosition.x, healthBarPosition.y, barPercentage, healthBarHeight), healthBarTexture, ScaleMode.ScaleToFit, true, 0);
	GUI.Label (Rect (healthBarPosition.x, healthBarPosition.y - 20, 100, 20), "Jogged Mickey");
}

function loadEnemyParameters()
{
	damage = excel.joggedMickeyArray[level, excel.E_DAMAGE];
	hp = excel.joggedMickeyArray[level, excel.E_HP];
	armor = excel.joggedMickeyArray[level, excel.E_ARMOR];
	range = excel.joggedMickeyArray[level, excel.E_RANGE];
	rateOfFire = excel.joggedMickeyArray[level, excel.E_RATE_OF_FIRE];
	gain = excel.joggedMickeyArray[level, excel.E_GAIN];
	speed = excel.joggedMickeyArray[level, excel.E_SPEED];
	mana = excel.joggedMickeyArray[level, excel.E_MANA];
	originalSpeed = speed;
}