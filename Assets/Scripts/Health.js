#pragma strict

var healthBarTexture: Texture;

var cameraObj: Camera;
var maxHealth: int = 100;
var health:int = 100;
var hitPoints:int = 5;
var healthBarWidth: int = 40;
var healthBarHeight: int = 20;
var healthBarPosition: Vector3;

function Start () {
	//assuming camera name is Main Camera
	cameraObj = GameObject.Find("Main Camera").GetComponent.<Camera>();
}

function Update () {
	if ( health <= 0 )
		Destroy(gameObject);
}

function OnTriggerEnter(other: Collider)
{
	//print(other.transform.name);
	if(other.transform.tag == "Projectile")
	{
	//	print(health);
	//	read the hit points from the projectile controller script on the collider object
	//	var projectileScript: ProjectileController = other.GetComponent("ProjectileController");
	//	hitPoints = projectileScript.damagePoints;
	//	print(hitPoints);
	//	health -= hitPoints;
	//	Destroy(other.gameObject);
	}	
	else if(other.transform.tag == "SpeedBreaker")
	{
		var wayPointPath: WayPointPath = this.GetComponent("WayPointPath");
		wayPointPath.currentSpeed = wayPointPath.originalSpeed/2;
	}
}

function OnGUI()
{
	var enemyPosition = cameraObj.WorldToScreenPoint(this.transform.position);
	healthBarPosition.y = Screen.height - enemyPosition.y - 30;
	healthBarPosition.x = enemyPosition.x - healthBarWidth/2;
	var barPercentage: float = health * healthBarWidth/ maxHealth;
	GUI.DrawTexture(Rect(healthBarPosition.x, healthBarPosition.y, barPercentage, healthBarHeight), healthBarTexture, ScaleMode.ScaleToFit, true, 0);
}

/*function OnCollisionEnter(other: Collision)
{
	print("yes");
	if(other.gameObject.name == "Cannonball(Clone)")
	{
		print(health);
		health -= 10;
		Destroy(other.gameObject);
	}
}*/
