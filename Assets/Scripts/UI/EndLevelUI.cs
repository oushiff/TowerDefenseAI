#region Copyright
// <copyright file="EndLevelUI.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/07/2016, 1:03 PM </date>
#endregion
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndLevelUI : MonoBehaviour {

    void Awake()
    {
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    void ShowUI()
    {
        UIManager.instance.RegisterUIClick();
        LeanTween.scale(gameObject, Vector3.one, 0.1f);
    }

    public void EndGame()
    {
        UIManager.instance.RegisterUIClick();
        SceneManager.LoadScene(0);
    }

}
