#pragma strict

var enemiesInRange:ArrayList;
var splashArea:double;
var level:int;

internal var excel : ExcelReader;

function Awake()
{
	//Get the ExcelReader Script
	excel = GameObject.FindGameObjectWithTag("ExcelReader").GetComponent("ExcelReader");
}

function Start () {
	enemiesInRange = new ArrayList();
	splashArea = excel.cannonTowerArray[level, excel.ARMOR_PENETRATION];
	this.GetComponent(SphereCollider).radius = splashArea;
}

function Update () {

}

function OnTriggerEnter(intruder: Collider)
{
	if(intruder.tag == "Enemy")
	{
		enemiesInRange.Add(intruder.gameObject);
	}
}

function OnTriggerExit(intruder: Collider)
{
	if(intruder.tag == "Enemy")
	{
		enemiesInRange.Remove(intruder.gameObject);
		/*
		var index:int = 0;
		for( var enemy:GameObject in enemiesInRange)
		{
			if( intruder.GetInstanceID() == enemy.GetInstanceID() )
			{
				enemiesInRange.Rem
			}
			index++;
		}*/
	}
}