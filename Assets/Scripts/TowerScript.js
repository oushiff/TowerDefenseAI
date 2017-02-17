#pragma strict

//index to reference the excelsheet
var towerNumber: int = 1;	//starting from 1

var reloadTime: float;
var firePauseTime: float;
var rotationSpeed: float;
var errorAmount: float; //input
var towerRange: float;

var myProjectile: GameObject;
var myTarget: Transform;
var nozzles: Transform[];
var turretSphere: Transform;


internal var desiredRotation: Quaternion;
internal var aimError: float; //one used internally
internal var nextFireTime: float;
internal var nextMoveTime: float;
internal var colliderList: ArrayList;

private var excel : ExcelReader;

function Awake()
{
	//Get the ExcelReader Script
	excel = GameObject.Find("Scripts").GetComponent("ExcelReader");
}

function Start () 
{
	//assign the values from the excelsheet
	if(towerNumber > excel.NUM_TOWERS)
	{
		towerNumber = excel.NUM_TOWERS;
	}
	var towerIndex: int = towerNumber - 1;
	reloadTime = excel.towerArray[towerIndex, 1];
	firePauseTime = excel.towerArray[towerIndex, 2];
	rotationSpeed = excel.towerArray[towerIndex, 3];
	errorAmount = excel.towerArray[towerIndex, 4];
	towerRange = excel.towerArray[towerIndex, 5];
	
	//assign the towerRange to the sphere collider radius
	this.GetComponent(SphereCollider).radius = towerRange;
	
	nextFireTime = Time.time + reloadTime;
	colliderList = new ArrayList();
}

function Update () {
//	print (colliderList.Count);
	if (excel.selectedEnemy != null)
	{
		var distance: float = Vector3.Distance(this.transform.position, excel.selectedEnemy.transform.position);
		if (distance <= towerRange)
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
		if(Time.time > nextMoveTime)
		{
			getRotation(myTarget.position);
			turretSphere.rotation = Quaternion.Lerp(turretSphere.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
		}
		if(Time.time >= nextFireTime)
		{
			for( nozzle in nozzles )
			{								
				Instantiate(myProjectile, nozzle.position, nozzle.rotation);
			}
			nextFireTime = Time.time + reloadTime;
			nextMoveTime = Time.time + firePauseTime;
		}	
	}
}

function OnTriggerEnter(intruder: Collider)
{
	
	if(intruder.gameObject.tag == "Enemy")
	{
		//myTarget = intruder.gameObject.transform;
		colliderList.Add(intruder.gameObject);
	}
}

function OnTriggerExit(intruder: Collider)
{
	//myTarget = null;
	colliderList.Remove(intruder.gameObject);
}

function CalculateAimError()
{
	aimError = Random.Range(-errorAmount, errorAmount);
}

function getRotation(targetPos: Vector3)
{
	CalculateAimError();
	var aimPoint = Vector3(targetPos.x+aimError, targetPos.y+aimError, targetPos.z+aimError);
	aimPoint = aimPoint - transform.position;
//	print(aimPoint);
	desiredRotation = Quaternion.LookRotation(aimPoint);
}