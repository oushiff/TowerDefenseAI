#pragma strict

internal var startingMoney:int = 1000;

function OnGUI()
{
	GUI.Label (Rect (10, 10, 100, 20), "Cash: "+startingMoney);
}