#region Copyright
// <copyright file="PauseLevelUI.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/07/2016, 1:02 PM </date>
#endregion
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseLevelUI : MonoBehaviour
{
    public GameObject popup;

    void Awake()
    {
        BoxCollider2D backgroundCollider = popup.GetComponent<BoxCollider2D>();
        RectTransform popupTransform = popup.GetComponent<RectTransform>();
        backgroundCollider.size = popupTransform.rect.size;
        
        popup.transform.localScale = Vector3.zero;
        popup.SetActive(false);

    }

    public void EndGame()
    {
        UIManager.instance.RegisterUIClick();
        GamePlay.instance.Pause();
        SceneManager.LoadScene(0);
    }

    public void ContinueGame()
    {
        UIManager.instance.RegisterUIClick();
        LeanTween.scale(popup, Vector3.zero, 0.1f).setOnComplete(() =>
                                                                            {
                                                                                popup.SetActive(false);
                                                                            });
        GamePlay.instance.Pause();
    }

    public void PauseGame()
    {
        UIManager.instance.RegisterUIClick();
        popup.SetActive(true);

        LeanTween.scale(popup, Vector3.one, 0.1f).setOnComplete(() =>
        {
            GamePlay.instance.Pause();
        });
    }
}
