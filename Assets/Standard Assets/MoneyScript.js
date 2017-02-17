#pragma strict

public static var instance : MoneyScript;
public static var startingMoney:int = 10000;

function Awake() {
    instance = this;
}
function OnGUI()
{
    GUI.Label (Rect (10, 10, 100, 20), "Cash: "+startingMoney);
}