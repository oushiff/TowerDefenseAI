#region Copyright
// <copyright file="BuildTowerUI.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 04/16/2016, 8:43 AM </date>
#endregion
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class BuildTowerUI : MonoBehaviour
{
    public List<BuildTowerButton> towerButtons;
    public Transform towersRoot;

    private RectTransform uiTransform;
    private Vector2 initialSize;
    private CanvasGroup canvasGroup;
    private GameObject selectedPlane;

    void Awake()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();

        uiTransform = this.GetComponent<RectTransform>();
        initialSize = uiTransform.rect.size;
    }

    void Start() {
#if UNITY_WEBGL
        StartCoroutine(LoadAvailableTowers());
#else
        List<TowerData.Level> availableTowers = GameData.instance.GetCurrentLevel().towers;
        this.GenerateUI(availableTowers);
#endif
        this.HideBuildTowerUI(0);
    }

    System.Collections.IEnumerator LoadAvailableTowers()
    {
        LevelData levelData = GameData.instance.GetCurrentLevel();

        WaitForSeconds waitTime = new WaitForSeconds(0.2f);
        while (levelData.loaded == false)
        {
            yield return waitTime;
        }
        GenerateUI(levelData.towers);
    }

    void GenerateUI(List<TowerData.Level> availableTowers)
    {
        for (int i = 0; i < availableTowers.Count; i++)
        {
            TowerData.Level towerInstance = GameData.instance.GetTower(availableTowers[i].tower.name, 0);
            if (towerInstance != null)
            {
                GameObject availableTowerGO = towerInstance.tower.GetPrefab();
                Tower availableTower = availableTowerGO.GetComponent<Tower>();
                towerButtons[i].Activate(availableTower.sprite, i, towerInstance.cost);
            }
            else {
                towerButtons[i].Deactivate();
            }
        }
    }

    public void SelectTower(int index)
    {
        UIManager.instance.RegisterUIClick();

        TowerData.Level selectedTower = GameData.instance.GetCurrentLevel().towers[index];
        // Calculate cost for new tower
        int towerCost = (int) selectedTower.GetProperty(GameData.GameProperies.COST);
        if (towerCost == 0)
            towerCost = 100;

        // If there are enough money to build the tower, deduct them
        if (Currency.instance.UseCoins(towerCost))
        {
            // Build the tower
            GameObject towerObject = Instantiate(selectedTower.tower.GetPrefab(), selectedPlane.transform.position, Quaternion.identity) as GameObject;
            Tower tower = towerObject.GetComponent<Tower>();
            tower.transform.parent = towersRoot;
            selectedPlane.tag = Grid.PLANE_NO_HOVER;

            // Place the tower on the grid
            GamePlay.instance.activeTowers.Add(tower);
        }

        HideBuildTowerUI();
    }

    public void ShowBuildTowerUI(GameObject _selectedPlane, float time = .1f)
    {
        if (UIManager.instance.ClickOnUI()) return;
        selectedPlane = _selectedPlane;

        var screenPosition = Camera.main.WorldToScreenPoint(selectedPlane.transform.position);
        Vector3 uiPosition = Vector3.zero;
        uiPosition.x = screenPosition.x;
        uiPosition.y = screenPosition.y;

        uiTransform.position = uiPosition;

        for (int i = 0; i < towerButtons.Count; i++)
            towerButtons[i].CheckAvailability();

        canvasGroup.blocksRaycasts = true;
        LeanTween.value(gameObject, this.UpdateCanvas, 0, 1, time);
    }

    public void HideBuildTowerUI(float time = .1f)
    {
        UIManager.instance.RegisterUIClick();
        canvasGroup.blocksRaycasts = false;

        LeanTween.value(gameObject, this.UpdateCanvas, 1, 0, time);
    }

    private void UpdateCanvas(float alpha)
    {
        uiTransform.sizeDelta = initialSize*alpha;
        canvasGroup.alpha = alpha;
    }
}
