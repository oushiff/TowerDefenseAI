 #pragma strict

var range:double = 2;
var speed:double = 1;
var originalSpeed:double = 1;
var rateOfFire:double = 1;

var myProjectile: GameObject;
internal var parentObj: GameObject;
var colliderList: ArrayList;
internal var desiredRotation: Quaternion;
internal var nextFireTime: double;

function Start () {
	//load the parameters from the parent script component
	parentObj = this.transform.parent.gameObject;
	//get the parameter values from the parent script
	
	this.GetComponent(SphereCollider).radius = range;
	colliderList = new ArrayList();
	nextFireTime = Time.time + rateOfFire;
}

function Update () {
	
	loadParameters();
	//print(colliderList.Count);
	if(Time.time > nextFireTime)
	{
		if(colliderList.Count > 0)
		{
			if(colliderList[colliderList.Count-1])
			{
				if(originalSpeed > speed)
				{
					var myTarget:GameObject = colliderList[colliderList.Count-1];
					getRotation(myTarget.transform.position);
					this.transform.rotation = desiredRotation; 
					//instantiate the projectile
					//assign the difference vector as the transform position
//					print("instantiate");
					Instantiate(myProjectile, this.transform.position, this.transform.rotation);
				}
			}
			else
			{
				//colliderList.RemoveAt(colliderList.Count-1);
			}
		}
		nextFireTime = Time.time + rateOfFire;
	}
	
}

function OnTriggerEnter(intruder: Collider)
{
	if(intruder.gameObject.tag == "Tower")
	{
		//myTarget = intruder.gameObject.transform;
		colliderList.Add(intruder.gameObject);
//		print("Tower found " + intruder.gameObject.tag);
	}
}

function getRotation(targetPos: Vector3)
{
	var aimPoint = targetPos - transform.position;
	desiredRotation = Quaternion.LookRotation(aimPoint);
}

function loadParameters()
{
	var script: MonsterScript = parentObj.GetComponent("MonsterScript");
	this.range = script.range;
	this.speed = script.speed;
	this.rateOfFire = script.rateOfFire;
	this.originalSpeed = script.originalSpeed;
}