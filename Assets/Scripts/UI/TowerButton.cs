#region Copyright
// <copyright file="TowerButton.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/07/2016, 9:17 AM </date>
#endregion
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour, IPointerClickHandler
{
    private CircleCollider2D imageCollider;
    public Image image;
    private Image costBackground;
    public Text cost;
    private int requiredCost;
    protected int towerIndex = 0;
    private Color disableColor = new Color(0.2f,0.2f,0.2f);
    protected GameObject buttonRoot;

    protected virtual void Awake()
    {
        imageCollider = this.GetComponent<CircleCollider2D>();

        Image imageRenderer = this.GetComponent<Image>();
        imageCollider.radius = 0.5f * imageRenderer.rectTransform.rect.width;

        costBackground = cost.transform.parent.GetComponent<Image>();

        buttonRoot = transform.parent.gameObject;
    }


    public void Activate(Sprite sprite, int _towerIdx, float _cost)
    {
        towerIndex = _towerIdx;
        requiredCost = (int) _cost;

        cost.text = requiredCost.ToString();

        image.sprite = sprite;

        this.CheckAvailability();
        buttonRoot.SetActive(true);
    }

    public void CheckAvailability()
    {
        if (requiredCost > Currency.instance.coins)
        {
            DisableButton();
            StartCoroutine(CheckCurrency());
        }
        else {
            this.EnableButton();
        }

    }
    protected IEnumerator CheckCurrency()
    {
        while (requiredCost > Currency.instance.coins){
            yield return new WaitForEndOfFrame();
        }
        this.EnableButton();
        yield return null;
    }

    protected virtual void EnableButton()
    {
        imageCollider.enabled = true;
        cost.color = Color.black;
        costBackground.color = image.color = Color.white;
    }

    protected virtual void DisableButton()
    {
        imageCollider.enabled = false;
        cost.color = Color.white;
        costBackground.color = image.color = disableColor;
    }

    public virtual void Deactivate()
    {
        buttonRoot.SetActive(false);
        this.StopAllCoroutines();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        
    }
}
