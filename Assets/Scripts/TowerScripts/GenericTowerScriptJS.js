#pragma strict

var hp: double;
var maxHp: double;
var armor:double ;
var rateOfFire:double ;
var regenerating:boolean;
var level:int;
var flying:boolean;
var damage:double ;
var range:double ;
var extradamage:double ;
var cost:double ;
var splashArea:double ;
var healTime:double ;

internal var colourChangeTimeout: double;
internal var myTarget: GameObject;
internal var myTargetTransform: Transform;
internal var rotationSpeed:float;
internal var aligned:boolean;
internal var projectile: GameObject;

public var gridObj: GameObject;
public var hasSplash:boolean;
public var projectileFollowsEnemies:boolean;
public var towerFacesEnemies:boolean;
public var supportsSoulTower:boolean;
public var rotateAboutYAxis:boolean;
public var nozzles: Transform[];

var cameraobj: Camera;
internal var distance:int;
var excel: ExcelReader;

var nextFireTime: double;
var colliderList = new Queue();

function Start () {
    initValues();
    loadParameters();
}
	
// Update is called once per frame
function Update () {
    //If youre regenerating stick in here
    if (regenerating)
    {
        hp += maxHp * ( Time.deltaTime / healTime);
        gameObject.transform.localScale = Vector3(0.2,0.2,0.2);
        if (hp >= maxHp)
        {
            hp = maxHp;
            regenerating = false;
            gameObject.transform.localScale = Vector3(0.5,0.5,0.5);
        }
        return;
    }
        
    //If you're done regenerating execute this stuff
    if (extradamage > 0)
    {
        //Red when extradamage enabled
        transform.FindChild("Cube").GetComponent.<Renderer>().material.color = Color.red;
    }
    else if (colourChangeTimeout <= 0)
    {
        //Yellow in normal situations - TODO Change
        transform.FindChild("Cube").GetComponent.<Renderer>().material.color = Color.yellow;
    }
    else
    {
        //Green when added
        transform.FindChild("Cube").GetComponent.<Renderer>().material.color = Color.green;
    }

    if (colourChangeTimeout > 0)
    {
        colourChangeTimeout -= Time.deltaTime;
    }

    //Cases when we have strictly selected an enemy
    if (excel.selectedEnemy != null)
    {
        var distance: float= Vector3.Distance(this.transform.position, excel.selectedEnemy.transform.position);
        if (distance <= range)
        {
            myTarget = excel.selectedEnemy;
            myTargetTransform = myTarget.transform;
        }
    }
    else if(colliderList.Count > 0)
    {
        while (colliderList.Count > 0 && colliderList.Peek() == null)
        {
            colliderList.Dequeue();
        }
        if(colliderList.Count > 0)
        {
            myTarget = (colliderList.Peek());
            myTargetTransform = myTarget.transform;
        }
    }

    if (colliderList.Count == 0)
    {
        myTarget = null;
    }

    if (myTarget != null && towerFacesEnemies)
    {
        //Move in the direction of the enemy
        var initialconfig : Vector3 = transform.eulerAngles;
        var targetDir : Vector3 = myTargetTransform.position - transform.position;
        var newDir : Vector3 = Vector3.RotateTowards(transform.forward, targetDir, rotationSpeed * Time.deltaTime, 0.0f);
        if(!rotateAboutYAxis)
        {
            newDir.y = 0;
        }
        transform.rotation = Quaternion.LookRotation(newDir);

        //Check the angle in between
        if (Mathf.Abs(Vector3.Angle(targetDir, transform.forward)) < 10)
        {
            //We are aligned
            aligned = true;
        }
    }

    //If we are aligned and time is right or if we are not supposed to follow enemies and timing is right
    if ((!towerFacesEnemies || aligned) && Time.time >= nextFireTime && myTarget != null)
    {
        nextFireTime = Time.time + rateOfFire;

        //Fire a weapon
        generateProjectile();
    }
    aligned = false;
}

function loadParameters()
{
    maxHp = excel.cannonTowerArray[level, ExcelReader.HP];
    hp = maxHp;
    damage = excel.cannonTowerArray[level, ExcelReader.DAMAGE];
    armor = excel.cannonTowerArray[level, ExcelReader.ARMOR];
    range = excel.cannonTowerArray[level, ExcelReader.RANGE];
    rateOfFire = excel.cannonTowerArray[level, ExcelReader.RATE_OF_FIRE];
    cost = excel.cannonTowerArray[level, ExcelReader.COST];
    splashArea = excel.cannonTowerArray[level, ExcelReader.ARMOR_PENETRATION];
    healTime = excel.cannonTowerArray[level, ExcelReader.HEAL_TIME];
    flying = excel.cannonTowerFlight;

	var sphereCollider = this.GetComponent(SphereCollider);
    sphereCollider.radius = 2 * range;
}

function initValues()
{
    excel = GameObject.Find("Scripts").GetComponent(ExcelReader);
    regenerating = false;
    cameraobj = excel.cameraObj;
    nextFireTime = Time.time + rateOfFire;
    colliderList = new Queue();
    colourChangeTimeout = 0;
    rotationSpeed = 10;

    aligned = false;

    projectile = Resources.Load("Prefabs/GenericTowerProjectile", GameObject);
}

function OnTriggerEnter(intruder : Collider)
{
    if(intruder.gameObject.tag == "Enemy")
	{
		var other:MonsterScript = intruder.gameObject.GetComponent(MonsterScript);
		if (other.flying == 0 || flying == true)
		{
	    	colliderList.Enqueue(intruder.gameObject);
	    }
	}
}

function OnTriggerExit(intruder : Collider)
{
    if(colliderList.Count > 0)
	{
	    colliderList.Dequeue();
	}
}

function generateProjectile()
{
	for(var nozzle : Transform in nozzles)
    {
	    var firedProjectile : GameObject = Instantiate(projectile, nozzle.position, nozzle.rotation);
		var a : GenericTowerFireScriptJS = firedProjectile.GetComponent(GenericTowerFireScriptJS);
		a.level = level;
		a.followEnemy = projectileFollowsEnemies;
		a.myTarget = myTarget;
		a.hasSplash = hasSplash;
		a.flying = flying;
		a.damage = damage + extradamage;
		a.splashArea = splashArea;
		a.initValues();
		
		extradamage = 0;
    }
}

function powerShot(x : double)
{
	Debug.Log("Power Shot");
	extradamage = damage * x;
}

function causeDamage(damageamount:double)
{
	if(!regenerating)
	{
		if(damageamount > armor)
		{
			hp-=(damageamount - armor);
		}
		if(hp <= 0)
		{
			hp = 0;
			regenerating = true;
		}
	}
}