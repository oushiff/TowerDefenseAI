#region Copyright
// <copyright file="SelectEnemy.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/08/2016, 11:37 PM </date>
#endregion
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectEnemy : MonoBehaviour
{
    public static SelectEnemy instance;

    public List<Tower> towers;
    public Transform towersRoot;
    public float cameraSpeed = 0.5f;

    private SoulTower soulTower;
    Material originalMat;
    public Material hovMaterial;
    int layerMask;
    bool linkingTower;
    
    //variable to store the state of the tower menu
    bool showTowerMenu = false;
    bool showUpgradeMenu = false;
    GameObject hoveredPlane;
    GameObject hoveredTower;
    GameObject selectedTower;
    
    //variable to store clicked/tapped enemy
    public GameObject selectedEnemy = null;
    
    //custom box skin
    public GUISkin boxSkin;

    public LayerMask towerLayr;
    public LayerMask enemyLayer;

    void Awake()
    {
        instance = this;
    }

    void Start () 
    {
        int layer = LayerMask.NameToLayer(Grid.RAYCAST_LAYER);
        layerMask = 1 << layer;
    }

    // Update is called once per frame
    void Update () {
        // User has just clicked the mouse button
        if(Input.GetMouseButtonDown(0))
        {
            // Check were the user clicked
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 1000, enemyLayer))
            {
                if(hit.collider.gameObject.CompareTag("Enemy"))
                {
                    Enemy enemy = hit.collider.GetComponent<Enemy> ();
                    //if (enemy == null && hit.collider.transform.parent != null)
                    //	enemy = hit.collider.transform.parent.GetComponent<Enemy> ();
                    
                    if (enemy != null)
                        selectedEnemy = enemy.gameObject;
                }
            }

            // Empty plane was selected, show menu to build a tower
            if(hoveredPlane != null && hoveredPlane.tag == Grid.PLANE_HOVER)
            {
                showTowerMenu = true;;
                selectedTower = hoveredPlane;
            }

            // Tower was selected, show menu for upgrade
            if(hoveredTower)
            {
                showUpgradeMenu = true;
            }
        }		
        
        // User holds the mouse button
        if (Input.GetMouseButton (0)) {
            // Drag the camera - No entity (Tower / Plane / Enemy) selected
            if (!hoveredPlane && !hoveredTower) {
                float h = cameraSpeed * Input.GetAxis ("Mouse X"); //The horizontal movement - could use "Horizontal"
                float v = cameraSpeed * Input.GetAxis ("Mouse Y"); //The vertical movement - could use "Vertical"
                Vector3 move = new Vector3 (-h, 0, -v);
                Camera.main.transform.Translate (move, Space.World);
            }
        } else { // Mouse is not clicked / User is hovering over things		
            // If neither Tower Build or Tower Upgrade screen are visible continue
            if(!showTowerMenu && !showUpgradeMenu)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, 100000, layerMask))
                {
                    // User hovering over empty plane
                    if(hit.collider.gameObject.tag == Grid.PLANE_HOVER)
                    {
                        if(hoveredPlane)
                        {
                            hoveredPlane.GetComponent<Renderer>().material = originalMat;
                        }
                        hoveredPlane = hit.collider.gameObject;
                        originalMat = hoveredPlane.GetComponent<Renderer>().material;
                        hoveredPlane.GetComponent<Renderer>().material = hovMaterial;
                        hoveredTower = null;
                    }
                    // User hovering over tower
                    else if(hit.collider.gameObject.tag == "Tower")
                    {
                        //assign the tower hovering on
                        hoveredTower = hit.collider.gameObject;
                        //empty the hoveredTower
                        if(hoveredPlane)
                        {
                            hoveredPlane.GetComponent<Renderer>().material = originalMat;
                            hoveredPlane = null;
                        }
                    }
                    // User hovering over nothing interesting
                    else
                    {
                        //empty the hoveredTower 
                        if(hoveredPlane)
                        {
                            hoveredPlane.GetComponent<Renderer>().material = originalMat;
                            hoveredPlane = null;
                        }
                        //empty towerSelected
                        hoveredTower = null;
                    }	
                }
                else
                {
                    //empty the hoveredTower
                    if(hoveredPlane)
                    {
                        hoveredPlane.GetComponent<Renderer>().material = originalMat;
                        hoveredPlane = null;
                    }
                    //empty the towerSelected
                    hoveredTower = null;
                }
            }
        }
    }

    void OnGUI()
    {
        GUI.skin = boxSkin;
        if (showTowerMenu && hoveredPlane)
        {
            Vector3 boxPosition = Camera.main.WorldToScreenPoint(selectedTower.transform.position);

            int boxWidth = 100;
            int boxHeight = 100;
            int boxX = (int) (boxPosition.x - boxWidth * 0.5f);
            int boxY = (int) (Screen.height - boxPosition.y - boxHeight * 0.5f);
            GUI.Box (new Rect (boxX, boxY, boxWidth, boxHeight), "");
            
            for (int i = 0; i < GameData.instance.GetCurrentLevel().towers.Count; i++){
                switch (GameData.instance.GetCurrentLevel().towers[i].index)
                {
                    case 1:
                        if (GUI.Button(new Rect(boxX, boxY, 20, 20), "1")){
                            showTowerMenu = false;
                            SpawnTower(0);
                        }
                        break;
                    case 2:
                        if (GUI.Button(new Rect(boxX + 40, boxY, 20, 20), "2")){
                            showTowerMenu = false;
                            SpawnTower(1);
                        }
                        break;
                    case 3:
                        if (GUI.Button(new Rect(boxX + 80, boxY, 20, 20), "3")){
                            showTowerMenu = false;
                            SpawnTower(2);
                        }
                        break;
                    case 4:
                        if (GUI.Button(new Rect(boxX, boxY + 80, 20, 20), "4")){
                            showTowerMenu = false;
                            SpawnTower(3);
                        }
                        break;
                    case 5:
                        if (GUI.Button(new Rect(boxX + 40, boxY + 80, 20, 20), "5")){
                            showTowerMenu = false;
                            SpawnTower(4);
                        }
                        break;
                    case 6:
                        if (GUI.Button(new Rect(boxX + 80, boxY + 80, 20, 20), "6"))
                        {
                            showTowerMenu = false;
                            SpawnTower(5);
                        }
                        break;
                }
            }

            if (GUI.Button(new Rect(boxX + 40, boxY + 40, 20, 20), "X")){
                showTowerMenu = false;
                hoveredPlane.GetComponent<Renderer>().material = originalMat;
                hoveredPlane = null;
            }
            //GENERATE Generic Tower
            /*if (GUI.Button (new Rect (boxX+80,boxY+120,20,20), "G")) 
            {
                showTowerMenu = false;
                SpawnTower(6);
            }*/
            
            //GENERATE SOUL WELL
            if (GUI.Button (new Rect (boxX,boxY+120,20,20), "S")) 
            {
                showTowerMenu = false;
                SpawnTower(7);
            }
        }
        
        else if (showUpgradeMenu && hoveredTower)
        {
            Vector3 upgradeBoxPosition = Camera.main.WorldToScreenPoint(hoveredTower.transform.position);

            int upgradeBoxWidth = 100;
            int upgradeBoxHeight = 100;
            int upgradeBoxX = (int) (upgradeBoxPosition.x - upgradeBoxWidth * 0.5f);
            int upgradeBoxY = (int) (Screen.height - upgradeBoxPosition.y - upgradeBoxHeight * 0.5f);
            GUI.Box (new Rect (upgradeBoxX,upgradeBoxY,upgradeBoxWidth,upgradeBoxHeight), "");
            
            if(!linkingTower)
            {
                if (hoveredTower.transform.parent.gameObject.tag == "SoulWell") 
                {
                    if (GUI.Button (new Rect (upgradeBoxX+15,upgradeBoxY-35,70,20), "Link")) 
                    {
                        soulTower = hoveredTower.GetComponent<SoulTower>();
                        linkingTower = true;
                        hoveredTower = null;
                        showUpgradeMenu = false;
                    }
                }
                
                if (getTowerLevel() >= 3)
                {
                    hoveredTower = null;
                    showUpgradeMenu = false;
                }
                else
                {
                    if (getTowerLevel() < 2)
                    {
                        if (GUI.Button (new Rect (upgradeBoxX+15,upgradeBoxY+15,70,20), "Upgrade")) 
                        {
                            UpgradeTower();
                            hoveredTower = null;
                            showUpgradeMenu = false;
                        }
                    }
                    else{
                        if (GUI.Button (new Rect (upgradeBoxX+15,upgradeBoxY+15,70,20), "Special1"))
                        {
                            UpgradeTower();
                            hoveredTower = null;
                            showUpgradeMenu = false;
                        }
                        
                        if (GUI.Button (new Rect (upgradeBoxX+15,upgradeBoxY+40,70,20), "Special2"))
                        {
                            UpgradeTower(2);
                            hoveredTower = null;
                            showUpgradeMenu = false;
                        }
                    }
                    
                    if (GUI.Button (new Rect (upgradeBoxX+15,upgradeBoxY+65,70,20), "Cancel")) 
                    {
                        hoveredTower = null;
                        showUpgradeMenu = false;
                    }
                }
            }
            else
            {
                linkTower();
                showUpgradeMenu = false;
                hoveredTower = null;
                linkingTower = false;
            }
        }
    }
    
    void SpawnTower(int towerIndex)
    {
        // Calculate cost for new tower
        double towerCost = GameData.instance.GetTower(towerIndex, 0).GetProperty(GameData.GameProperies.COST);
        if (towerCost == 0.0f)
            towerCost = 100.0f;

        // If there are enough money to build the tower, deduct them
        if (ReduceMoney (towerCost)) {
            // Build the tower
            Tower tower = Instantiate (towers [towerIndex], selectedTower.transform.position, Quaternion.identity) as Tower;
            tower.transform.parent = towersRoot;
            selectedTower.tag = Grid.PLANE_NO_HOVER;

            // Place the tower on the grid
            GamePlay.instance.activeTowers.Add (tower);
        }
    }

    Tower GetTower(GameObject go)
    {
        Tower tower = go.GetComponent<Tower> ();
        if (tower == null)
            tower = go.transform.parent.GetComponent<Tower> ();
        return tower;
    }

    void UpgradeTower(int upgradeAmount = 1)
    {
        Tower tower = GetTower(hoveredTower);
        if (tower == null) {
            Debug.LogWarning("Tower not found");
            return;
        }
        // If tower can be upgraded and we have the money for it
        if (tower.IsTowerUpgradable() && ReduceMoney(GameData.instance.GetTower(tower.type, tower.level + upgradeAmount).GetProperty(GameData.GameProperies.COST)))
            // Upgrade the tower
            tower.UpgradeTower(upgradeAmount);
    }
    
    void linkTower()
    {
        if (soulTower != null)
            soulTower.LinkTower(GetTower(selectedTower));
    }
    
    int getTowerLevel()
    {
        Tower tower = GetTower(hoveredTower);
        if (tower != null)
            return tower.level;
        return 0;
    }
    
    bool ReduceMoney(double towerCost)
    {
        return Currency.instance.UseCoins ((int) towerCost);
    }
}
