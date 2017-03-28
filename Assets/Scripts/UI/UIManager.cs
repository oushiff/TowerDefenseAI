#region Copyright
// <copyright file="UIManager.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 04/06/2016, 8:50 AM </date>
#endregion
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using EndLevelUI;


public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private List<HealthBar> availableHealthBars, usedHealthBars;

    public Transform healthBarsRoot;
    public HealthBar characterInfoPrefab;
    public BuildTowerUI buildTowerUi;
    public UpgradeTowerUI upgradeTowerUi;
    private bool clickedUI = false;

    public LayerMask enemyLayer;
    private int layerMask;

    private Enemy selectedEnemy;
    private Tower selectedTower;

    public GameObject levelLoading;

    void Awake()
    {
        instance = this;
        this.availableHealthBars = new List<HealthBar>();
        this.usedHealthBars = new List<HealthBar>();
    }

    void Start()
    {
        int layer = LayerMask.NameToLayer(Grid.RAYCAST_LAYER);
        layerMask = 1 << layer;
#if UNITY_WEBGL
        levelLoading.SetActive(true);
        StartCoroutine(WaitLevelLoad());
#else
        levelLoading.SetActive(false);
#endif
    }

#if UNITY_WEBGL
    public IEnumerator WaitLevelLoad()
    {
        LevelData levelData = GameData.instance.GetCurrentLevel();

        WaitForSeconds waitTime = new WaitForSeconds(0.2f);
        while (levelData.loaded == false)
        {
            yield return waitTime;
        }

        levelLoading.SetActive(false);

    }
#endif

    public void RegisterUIClick()
    {
        clickedUI = true;
    }

    public bool ClickOnUI()
    {
        return clickedUI;
    }

    // Update is called once per frame
    //void Update()
    //{
    //}

    void LateUpdate()
    {
        // User has just clicked the mouse button
        if (Input.GetMouseButtonUp(0) && clickedUI == false)
        {
            // Check were the user clicked
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, enemyLayer | layerMask))
            {
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    selectedEnemy = hit.collider.GetComponent<Enemy>();
                }
                // Empty Tower Plane
                else if (hit.collider.gameObject.tag == Grid.PLANE_HOVER)
                {
                    buildTowerUi.ShowBuildTowerUI(hit.collider.gameObject);
                }
                // User hovering over tower
                else if (hit.collider.gameObject.tag == "Tower")
                {
                    selectedTower = hit.collider.GetComponent<Tower>();
                    if (selectedTower == null && hit.collider.transform.parent != null)
                    {
                        selectedTower = hit.collider.transform.parent.GetComponent<Tower>();
                    }
                    upgradeTowerUi.ShowUpgradeTowerUI(selectedTower);
                }
            }
        }

        clickedUI = false;
    }

    public void ClearSelectedTower()
    {
        selectedTower = null;
    }

    public Tower GetSelectedTower()
    {
        return selectedTower;
    }

    public Enemy GetSelectedEnemy()
    {
        return selectedEnemy;
    }

    public HealthBar GetHealthBar()
    {
        if (availableHealthBars.Count == 0){
            HealthBar newHealthBar = Instantiate(characterInfoPrefab, - Vector3.one * 1000, Quaternion.identity) as HealthBar;
            newHealthBar.transform.SetParent(healthBarsRoot);

            availableHealthBars.Add(newHealthBar);
        }
        
        HealthBar availableHealthBar = availableHealthBars[0];
        availableHealthBars.RemoveAt(0);
        usedHealthBars.Add(availableHealthBar);
        availableHealthBar.Activate();
        return availableHealthBar;
    }

    public void ReleaseHealthBar(HealthBar healthBar)
    {
        healthBar.transform.position = -Vector3.one*1000;
        healthBar.Deactivate();
        usedHealthBars.Remove(healthBar);
        availableHealthBars.Add(healthBar);
    }

    public void ShowEndOfLevel() {
		

//		EndLevelUI endUI = new EndLevelUI ();
//		endUI.ShowUI ();
//		endUI.EndGame ();


	}
}
