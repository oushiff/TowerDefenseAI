#pragma strict

//assigning the level before instantiating it
var enemyIndex:int = 0;

var damage:double;
//var armorPenetration:double;
var range:double;

//not coming from excel
var speed: float = 3;
internal var distance: float;

internal var excel : ExcelReader;

function Awake()
{
	//Get the ExcelReader Script
	excel = GameObject.FindGameObjectWithTag("ExcelReader").GetComponent("ExcelReader");
}

function Start () {
	damage = excel.enemyStatsArray[enemyIndex, excel.E_DAMAGE];
	range = excel.enemyStatsArray[enemyIndex, excel.E_RANGE];
	//armorPenetration = excel.gunTowerArray[level, excel.ARMOR_PENETRATION];
}

function Update () {
	transform.Translate(Vector3.forward * Time.deltaTime * speed);
	distance += Time.deltaTime * speed;
	if( distance >= range)
	{
		Destroy(gameObject);
	}
}