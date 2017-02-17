#region Copyright
// <copyright file="HealthBar.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/04/2016, 7:32 AM </date>
#endregion
using System.CodeDom;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Text character;
    public Image foreground;

    private float healthBarWidth;// = 40;
    private float healthBarHeight;// = 20;

    private Vector3 healthBarPosition;

    RectTransform rectTransform;
    Vector3 barPosition;

    void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
        gameObject.SetActive(false);
        
        healthBarWidth = foreground.rectTransform.rect.width;
        healthBarHeight = foreground.rectTransform.rect.height;
    }

    public void UpdateName(string _name)
    {
        this.character.text = _name;
    }

    public void UpdateGUI(/*string type, */Vector3 worldPosition, float hpPercentage/*, int level*/, float heightOffset = 1.5f)
    {
        // Computer healthBar position
        var screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        healthBarPosition.x = screenPosition.x;
        healthBarPosition.y = screenPosition.y + heightOffset * healthBarHeight;
        
        rectTransform.position = healthBarPosition;

        // Update Percentage
        foreground.fillAmount = hpPercentage;
    }

    public void UpdateHp(float hpPercentage)
    {
        foreground.fillAmount = hpPercentage;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}