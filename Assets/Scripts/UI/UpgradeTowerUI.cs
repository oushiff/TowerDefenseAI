#region Copyright
// <copyright file="UpgradeTowerUI.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/06/2016, 7:44 AM </date>
#endregion
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UpgradeTowerUI : MonoBehaviour
{
    private RectTransform uiTransform;
    private Vector2 initialSize;
    private CanvasGroup canvasGroup;

    public Image towerImage;
    public UpgradeTowerButton specialTower1, specialTower2, upgradeTower;

    void Awake()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();

        uiTransform = this.GetComponent<RectTransform>();
        initialSize = uiTransform.rect.size;
    }

    void Start()
    {
        this.HideUpgradeTowerUI(0);
    }

    public void UpgradeTower(int nextTowerIndex)
    {
        UIManager.instance.RegisterUIClick();
        UIManager.instance.GetSelectedTower().UpgradeToTower(nextTowerIndex);

        this.HideUpgradeTowerUI();
    }

    public void ShowUpgradeTowerUI(Tower tower, float time = .1f)
    {
        if (UIManager.instance.ClickOnUI()) return;

        if (tower.IsTowerUpgradable())
        {
            towerImage.sprite = tower.sprite;

            if (tower.IsNextUpgradeSpecial()){
                specialTower1.Activate(towerImage.sprite, tower.level + 1, tower.GetUpgrade(1).cost);
                specialTower2.Activate(towerImage.sprite, tower.level + 2, tower.GetUpgrade(2).cost);
                upgradeTower.Deactivate();
            }
            else{
                specialTower1.Deactivate();
                specialTower2.Deactivate();
                upgradeTower.Activate(towerImage.sprite, tower.level + 1, tower.GetUpgrade(1).cost);
            }

            var screenPosition = Camera.main.WorldToScreenPoint(tower.transform.position);
            Vector3 uiPosition = Vector3.zero;
            uiPosition.x = screenPosition.x;
            uiPosition.y = screenPosition.y;

            uiTransform.position = uiPosition;

            canvasGroup.blocksRaycasts = true;
            LeanTween.value(gameObject, this.UpdateCanvas, 0, 1, time);
        }
    }

    public void HideUpgradeTowerUI(float time = .1f)
    {
        UIManager.instance.RegisterUIClick();
        UIManager.instance.ClearSelectedTower();

        canvasGroup.blocksRaycasts = false;

        LeanTween.value(gameObject, this.UpdateCanvas, 1, 0, time);
    }

    private void UpdateCanvas(float alpha)
    {
        uiTransform.sizeDelta = initialSize * alpha;
        canvasGroup.alpha = alpha;
    }
}
