#pragma strict

//assigning the level before instantiating it
var level:int = 0;
var myTarget:Transform;
var flying:boolean;
var damage:double;
var range:double;

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
	excel = GameObject.Find("Scripts").GetComponent("ExcelReader");
}

function Start () {
	damage = excel.scytheTowerArray[level, excel.DAMAGE];
	range = excel.scytheTowerArray[level, excel.RANGE];
	
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
					if(enemy)
					{
						var monsterScript:MonsterScript = enemy.GetComponent("MonsterScript");
						if (monsterScript.flying == 0 || flying == true)
						{
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