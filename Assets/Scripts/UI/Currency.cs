#region Copyright
// <copyright file="Currency.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 04/08/2016, 8:36 AM </date>
#endregion
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Currency : MonoBehaviour {

    public static Currency instance;
    public Text text;

    public int startingCoins = 1000;
    private int _coins = 1000;
    public int coins
    {
        set
        {
            _coins = value;
            text.text = _coins.ToString();
        }
        get { return _coins; }
    }

    // Use this for initialization
    void Awake () {
        instance = this;
    }

    void Start()
    {
#if UNITY_WEBGL
        StartCoroutine(GetStartingMoney());
#else
        startingCoins = GameData.instance.GetCurrentLevel().startingMoney;

        if (startingCoins > 0)
            coins = startingCoins;
#endif
    }

    IEnumerator GetStartingMoney()
    {
        LevelData levelData = GameData.instance.GetCurrentLevel();

        WaitForSeconds waitTime = new WaitForSeconds(0.2f);
        while (levelData.loaded == false)
        {
            yield return waitTime;
        }
        startingCoins = levelData.startingMoney;

        if (startingCoins > 0)
            coins = startingCoins;
    }

    public void GainCoins(int value)
    {
        coins += value;
    }

    public bool UseCoins(int value)
    {
        if (coins - value >= 0) {
            coins -= value;
            return true;
        }
        return false;
    }
}
