#region Copyright
// <copyright file="LevelsMenu.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 04/13/2016, 8:27 AM </date>
#endregion
using UnityEngine;
using System.Collections;
using System; 
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class LevelsMenu : MonoBehaviour {
    public Transform buttonsRoot;
    public GameObject levelButton;

#if UNITY_WEBGL
    public string baseUrl = "http://www-scf.usc.edu/~frangoud/";
#endif
    #region Initialize
    void Awake()
    {
        HideErrorMessage ();
        HideInfoMessage ();
    }

    void Start()
    {
#if UNITY_WEBGL
        StartCoroutine(PopulateWebLevels());
#else
        this.PopulateLevels();
#endif
    }

#if UNITY_WEBGL
    private IEnumerator PopulateWebLevels()
    {
        WWW www = null;
        int nextLevel = 0;

#if !UNITY_EDITOR
        int indexPageIndex = Application.absoluteURL.IndexOf("index.html");
        if (indexPageIndex < 0) indexPageIndex = Application.absoluteURL.Length;
        baseUrl = Application.absoluteURL.Substring(0, indexPageIndex-1);
#endif

        do
        {
            nextLevel++;
            try{
                www = new WWW(string.Format("{0}/Data/Level{1}.xml", this.baseUrl, nextLevel));
            }
            catch (Exception e){Debug.LogWarning(e);}
            yield return www;
        } while (string.IsNullOrEmpty(www.error));

        int noLevels = nextLevel - 1;
        for (int i = 1; i <= noLevels; i++)
        {
            GameObject newLevelButton = Instantiate(levelButton) as GameObject;
            newLevelButton.transform.SetParent(buttonsRoot);
            newLevelButton.transform.localScale = Vector3.one;

            newLevelButton.GetComponentInChildren<Text>().text = "Level " + i.ToString();
            LevelButtonUI buttonUI = newLevelButton.GetComponent<LevelButtonUI>();
            buttonUI.level = i;
            buttonUI.levelMenu = this;
        }
    }
#endif

    private void PopulateLevels()
    {
        int noLevels = DataReader.instance.GetNolevels();

        for (int i = 1; i <= noLevels; i++) {
            GameObject newLevelButton = Instantiate(levelButton) as GameObject;
            newLevelButton.transform.SetParent(buttonsRoot);
            newLevelButton.transform.localScale = Vector3.one;
            
            newLevelButton.GetComponentInChildren<Text>().text = "Level " + i.ToString();
            LevelButtonUI buttonUI = newLevelButton.GetComponent<LevelButtonUI>();
            buttonUI.level = i;
            buttonUI.levelMenu = this;
        }
    }
#endregion

#region Events
    public void SelectLevel(int level)
    {
        buttonsRoot.gameObject.SetActive (false);
        ShowInfoMessage ("Level is loading ...");
        PlayerPrefs.SetInt ("Level", level);
        //Application.LoadLevel (Application.loadedLevel + 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
#endregion
    
#region Info Message
    public Text infoMessage;
    
    void ShowInfoMessage(string msg)
    {
        infoMessage.text = msg;
        infoMessage.gameObject.SetActive (true);
    }
    
    void HideInfoMessage()
    {
        infoMessage.gameObject.SetActive (false);
    }
#endregion
    
#region Error Message
    public Text errorMessage;
    
    void ShowErrorMessage(string msg)
    {
        errorMessage.text = msg;
        errorMessage.gameObject.SetActive (true);
    }
    
    void HideErrorMessage()
    {
        errorMessage.gameObject.SetActive (false);
    }
#endregion
}
