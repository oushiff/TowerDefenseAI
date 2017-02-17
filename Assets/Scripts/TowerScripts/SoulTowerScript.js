#pragma strict

import SimpleJSON;
import System.IO;

//variable to store the current level of upgrades from 0 to 4
 var level:int = 0;
//variables to store the properties of the tower
 var damage: double;
 var range: double;
 var rateOfFire: double;
 var cost: double;
 var healTime: double;
 var splashArea: double;
 var numKills : int;
 var requiredKills: int;
 var powershotPower: double;
 
 public var flying:boolean;
 
//not being used right now
internal var firePauseTime:int = 0;
internal var rotationSpeed:double = 5;

//variables used in calculation
internal var desiredRotation: Quaternion;
internal var nextFireTime: float;
internal var nextMoveTime: float;
internal var colliderList: ArrayList;
internal var linkedTowers: ArrayList;

//health bar GUI
 var cameraObj: Camera;
 var healthBarWidth: int = 40;
 var healthBarHeight: int = 20;
 var healthBarPosition: Vector3;
 var healthBarTexture: Texture;
var speed: float = 1; // projectile speed
var dist: float = 0; // spawn point distance from the turret


//variable to store the placement on the grid
internal var gridObj: GameObject;

internal var excel : ExcelReader;

internal var stats: TowerStats;

var N : JSONNode;

function Awake()
{
    //Get the ExcelReader Script
    excel = GameObject.Find("Scripts").GetComponent("ExcelReader");
    stats = this.GetComponent("TowerStats");
}

function Start () {
    //Initialize JSONreader
    var jsonString :String; 
    //jsonString = File.ReadAllText(Application.dataPath +  "/soultowerstats.json");
    return;
    N = JSON.Parse(jsonString);
    
    //load the parameters from the excelreader based on the level
    loadTowerParameters();
    stats.online = true;
    cameraObj = excel.cameraObj;
    //assign the towerRange to the sphere collider radius
    this.GetComponent(SphereCollider).radius = 2 * range;
    nextFireTime = Time.time + rateOfFire;
    colliderList = new ArrayList();
    linkedTowers = new ArrayList();
}
function Regenerate ()
{
    if (!stats.online)
    {
        stats.hp += stats.maxHp/healTime;
    }
    
    if (stats.hp >= stats.maxHp)
    {
        stats.hp = stats.maxHp;
        stats.online = true;
        gameObject.transform.localScale = Vector3(0.5,0.5,0.5);
        CancelInvoke("Regenerate");
    
    }

}

function Update () {
    if(stats.hp <= 0 && stats.online)
    { 
        //gridObj.tag = excel.PLANE_HOVER;
        //Destroy(gameObject);
        stats.hp = 0;
        gameObject.transform.localScale = Vector3(0.2,0.2,0.2);
        stats.online = false;
        InvokeRepeating("Regenerate",1,1);
    }
}

function OnTriggerEnter(intruder: Collider)
{
    if(intruder.gameObject.tag == "Enemy")
    {
        var other:MonsterScript = intruder.gameObject.GetComponent(MonsterScript);
        other.AddSoulWell(gameObject);
        colliderList.Add(intruder.gameObject);
    }
}

function OnTriggerExit(intruder: Collider)
{
    if(intruder.gameObject.tag == "Enemy")
    {
        var other:MonsterScript = intruder.gameObject.GetComponent(MonsterScript);
        other.RemoveSoulWell(gameObject);
        colliderList.Add(intruder.gameObject);
    }
}

function getRotation(targetPos: Vector3)
{
    //CalculateAimError();
    //var aimPoint = Vector3(targetPos.x+aimError, targetPos.y+aimError, targetPos.z+aimError);
    //aimPoint = aimPoint - transform.position;
    var aimPoint = targetPos - transform.position;
//	print(aimPoint);
    desiredRotation = Quaternion.LookRotation(aimPoint);
}

function loadTowerParameters()
{
    damage = N[level]["damage"].AsDouble;
    stats.hp = N[level]["hp"].AsDouble;
    stats.armor = N[level]["armor"].AsDouble;
    range = N[level]["range"].AsDouble;
    rateOfFire = N[level]["rateOfFire"].AsDouble;
    cost = N[level]["cost"].AsDouble;
    stats.maxHp = stats.hp;
    splashArea = N[level]["splashArea"].AsDouble;
    healTime = N[level]["healTime"].AsDouble;
    requiredKills = N[level]["requiredKills"].AsInt;
    flying = N[level]["flying"].AsBool;
    powershotPower = N[level]["powershotPower"].AsDouble;
}

function linkTower(go : GameObject)
{
    if(go != gameObject)
    {
        var gts : GenericTowerScriptJS = go.GetComponent("GenericTowerScriptJS");
        if(gts.supportsSoulTower)
        {
            linkedTowers.Add(go);
            gts.colourChangeTimeout = 1;
            Debug.Log("Linked Tower" + go.name);
        }
    }
}

function notifyDied(go: GameObject)
{
    colliderList.Remove(go);
    
    if(++numKills == requiredKills)
    {
        Debug.Log("Super Power! Die MOFOS!");
        numKills = 0;
        sendSuperPower();
    }
}

function sendSuperPower()
{
    for( tower in linkedTowers)
    {
        //Send out event to the different towers based on their type.
        var parentObj = tower as GameObject;
        
        if(parentObj.tag == "GunTower")
        {
            var gunTowerScript:GunTowerScript = parentObj.GetComponent("GunTowerScript");
            //gunTowerScript.powerShot(0.8/linkedTowers.Count);
        }
        else if(parentObj.tag == "CanonTower")
        {
            var cannonTowerScript:CannonTowerScript = parentObj.GetComponent("CannonTowerScript");
            cannonTowerScript.powerShot(powershotPower/linkedTowers.Count);
        }
        else if(parentObj.tag == "ThresherTower")
        {
            var thresherTowerScript:ThresherTowerScript = parentObj.GetComponent("ThresherTowerScript");
            //thresherTowerScript.powerShot(0.8/linkedTowers.Count);
        }
        else if(parentObj.tag == "ElectricalTower")
        {
            var electricalTowerScript:ElectricalTowerScript = parentObj.GetComponent("ElectricalTowerScript");
            //electricalTowerScript.powerShot(); TODO
        }
        else if(parentObj.tag == "ScytheTower")
        {
            var scytheTowerScript:ScytheTowerScript = parentObj.GetComponent("ScytheTowerScript");
            //scytheTowerScript.powerShot();
        }
        else if(parentObj.tag == "TowernautTower")
        {
            var towernautTowerScript:TowernautTowerScript = parentObj.GetComponent("TowernautTowerScript");
            //towernautTowerScript.powerShot(); TODO
        }
        else if(parentObj.tag == "GenericTower")
        {
            var genericTowerScript:GenericTowerScriptJS = parentObj.GetComponent("GenericTowerScriptJS");
            genericTowerScript.powerShot(powershotPower/linkedTowers.Count);
        }
    }
}

function causeDamage(damageamount:double)
{
    if(stats.online)
    {
        if(damageamount > stats.armor)
        {
            stats.hp-=(damageamount - stats.armor);
        }
        if(stats.hp <= 0.0f)
        {
            stats.hp = 0.0f;
            //this.transform.FindChild("Cube").renderer.material.color = Color.gray;
        }
    }
}
function OnGUI()
{
    var enemyPosition = cameraObj.WorldToScreenPoint(this.transform.position);
    healthBarPosition.y = Screen.height - enemyPosition.y - 30;
    healthBarPosition.x = enemyPosition.x - healthBarWidth/2;
    var barPercentage: float = stats.hp * healthBarWidth/ stats.maxHp;
    GUI.DrawTexture(Rect(healthBarPosition.x, healthBarPosition.y, barPercentage, healthBarHeight), healthBarTexture, ScaleMode.ScaleToFit, true, 0);
    GUI.Label (Rect (healthBarPosition.x, healthBarPosition.y - 20, 150, 20), "Soul Tower:"+level);
}