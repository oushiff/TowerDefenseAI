#pragma strict

//assigning the level before instantiating it
var level:int = 0;
var myTarget:Transform;

var flying:boolean;
var damage:double;
var range:double;

var extradamage: double;

//not coming from excel
var speed: float = 10;
internal var distance: float;

var myRigidBody:Rigidbody;
var force = 100;
var angle = 50;

internal var excel : ExcelReader;

function Awake()
{
	//Get the ExcelReader Script
	excel = GameObject.FindGameObjectWithTag("ExcelReader").GetComponent("ExcelReader");
}

function Start () {
	damage = excel.cannonTowerArray[level, excel.DAMAGE];
	range = excel.cannonTowerArray[level, excel.RANGE];
	extradamage = 0;
	//myRigidBody = gameObject.GetComponent("Rigidbody");
	//Fire();
}

function Update () {
	transform.Translate(Vector3.forward * Time.deltaTime * speed);
	distance += Time.deltaTime * speed;
	if( distance >= range)
	{
		Destroy(gameObject);
	}
	
	if(extradamage>0)
	{
		this.GetComponent.<Renderer>().material.color = Color.red;
	}
}

function Fire () {
    //transform.eulerAngles.z = -angle;
    //myRigidBody.velocity = Vector3.back * force;
    //myRigidBody.AddForce(Vector3.forward * force);
    
    // Short delay added before Projectile is thrown
    //yield return new WaitForSeconds(1.5f);

    // Move projectile to the position of throwing object + add some offset if needed.
    //Projectile.position = myTransform.position + new Vector3(0, 0.0f, 0);

	var firingAngle:float = 45.0f;
	var gravity:float = 0.001f;
    // Calculate distance to target
    var target_Distance:float = Vector3.Distance(transform.position, myTarget.position);
	print("target distance :: "+ target_Distance);
    // Calculate the velocity needed to throw the object to the target at specified angle.
    var projectile_Velocity:float = Mathf.Sqrt(target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity));
	print("projectile_Velocity :: "+ projectile_Velocity);
    // Extract the X & Y componenent of the velocity
    var Vx:float = projectile_Velocity * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
    var Vy:float = projectile_Velocity * Mathf.Sin(firingAngle * Mathf.Deg2Rad);
	print("Vx :: "+ Vx);
	print("Vy :: "+ Vy);
    // Calculate flight time.
    var flightDuration:float = target_Distance / Vx;
    print("flightDuration :: "+ flightDuration);
    //var flightDuration:float = 2;
    // Rotate projectile to face the target.
    //transform.rotation = Quaternion.LookRotation(myTarget.position - transform.position);
    var elapse_time:float = 0;

    while (elapse_time < flightDuration)
    {
        transform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);
        elapse_time += Time.deltaTime;
//        yield return null;
	}
}

function OnTriggerEnter(intruder:Collider)
{
	if(intruder.tag == "Enemy")
	{
		//print("collision");
		//get the child object
		var firstMonsterScript:MonsterScript = intruder.GetComponent("MonsterScript");
		
		if (firstMonsterScript.flying == 0 || flying == true)
		{
			var childObject:GameObject = transform.gameObject.Find("SphereCollider");
			var childScript:SplashCollider = childObject.GetComponent("SplashCollider");
			var enemies:ArrayList = childScript.enemiesInRange;
			if(enemies.Count > 0)
			{
				for( var enemy:GameObject in enemies )
				{
					//print("loop");
					if(enemy)
					{
						//print("enemy");
						var monsterScript:MonsterScript = enemy.GetComponent("MonsterScript");
						if (monsterScript.flying == 0 || flying == true)
						{
							damage+=extradamage;
							var effectiveDamage : double = damage - monsterScript.armor;
							if(effectiveDamage>0)
							{
								monsterScript.hp = monsterScript.hp - damage;
							}
						}
					}
				}
			}
			Destroy(gameObject);
		}
	}
}