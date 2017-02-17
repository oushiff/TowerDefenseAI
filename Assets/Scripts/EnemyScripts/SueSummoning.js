#pragma strict


private var myX:float;
private var myZ:float;
public var spawnCooldownSeconds:int;
public var enemyType:GameObject;
private var newObject:GameObject;
private var myScript:MonsterScript;
private var newScript:MonsterScript;
internal var excel : ExcelReader;
private var type:int;
public var types:GameObject[];
function Start () {
	//Get the ExcelReader Script
	excel = GameObject.Find("Scripts").GetComponent("ExcelReader");
	
	spawnCooldownSeconds = excel.sueCooldown;
	type = excel.sueType;
	//Default values that won't clobber inspector values
	if (spawnCooldownSeconds <=0)
	{
		spawnCooldownSeconds= 6;
	}
	DetermineType();
	if (enemyType == null)
	{
		enemyType = GamePlay.publicEnemies[1];
	}
	InvokeRepeating("Spawn",spawnCooldownSeconds,spawnCooldownSeconds);
}
function DetermineType()
{
	if (type >= 0 && type < 27)
		enemyType = types[type];
	return;
	switch (type)
	{
	case "Zombie":
		enemyType = types[0];
		break;
	case "Tenacious Zombie":
		enemyType = types[1];
		break;
	case "Starving Zombie":
		enemyType = types[2];
		break;
	case "Jogged Mickey":
		enemyType = types[3];
		break;
	case "Running Mickey":
		enemyType = types[4];
		break;
	case "Sprinter Mickey":
		enemyType = types[5];
		break;
	case "Blind Tommy Science":
		enemyType = types[6];
		break;
	case "Advanced Blind Tommy Science":
		enemyType = types[7];
		break;
	case "Heavy Blind Tommy Science":
		enemyType = types[8];
		break;
	case "Frankensteam":
		enemyType = types[9];
		break;
	case "Armored Frankensteam":
		enemyType = types[10];
		break;
	case "Berzerk Frankensteam":
		enemyType = types[11];
		break;
	case "Sickly Sue Susie":
		enemyType = types[12];
		break;
	case "Witch Sickly Sue Susie":
		enemyType = types[13];
		break;
	case "Banshee Sickly Sue Susie":
		enemyType = types[14];
		break;
	case "Shaman":
		enemyType = types[15];
		break;
	case "Red Shaman":
		enemyType = types[16];
		break;
	case "Spirit Shaman":
		enemyType = types[17];
		break;
	case "Root Reggie":
		enemyType = types[18];
		break;
	case "Reggie 2":
		enemyType = types[19];
		break;
	case "Reggie 3":
		enemyType = types[20];
		break;
	case "Mummy Dearest":
		enemyType = types[21];
		break;
	case "Hungry Mummy":
		enemyType = types[22];
		break;
	case "Ancient Mummy":
		enemyType = types[23];
		break;
	case "Werefallo":
		enemyType = types[24];
		break;
	case "Werefallo 2":
		enemyType = types[25];
		break;
	case "Werefallo 3":
		enemyType = types[26];
		break;
	default:
		enemyType = types[0];
		break;
	}
}
function Update () {

}

function Spawn ()
{
	myX = gameObject.transform.position.x;
	myZ = gameObject.transform.position.z;
	
	newObject = Instantiate(enemyType);
	myScript = gameObject.GetComponent("MonsterScript");
	newScript = newObject.GetComponent("MonsterScript");
	
	newObject.transform.position.x = myX;
	newObject.transform.position.z = myZ;
	newScript.nextPointIndex = myScript.nextPointIndex;
	
	GamePlay.activeEnemies.Add(newObject);
}