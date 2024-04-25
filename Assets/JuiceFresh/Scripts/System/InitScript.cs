﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using JuiceFresh.Scripts.System;
#if UNITY_ADS
using JuiceFresh.Scripts.Integrations;
using UnityEngine.Advertisements;
#endif

#if CHARTBOOST_ADS
using ChartboostSDK;
#endif
#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
#endif
using UnityEngine.EventSystems;
using UnityEngine.UI;
using YG;

public enum Target
{
    SCORE,
    COLLECT,
    ITEMS,
    BLOCKS,
    CAGES,
    BOMBS,
}

public enum LIMIT
{
    MOVES,
    TIME
}

public enum Ingredients
{
    None = 0,
    Ingredient1,
    Ingredient2,
    Ingredient3,
    Ingredient4
}

public enum CollectItems
{
    None = 0,
    Item1,
    Item2,
    Item3,
    Item4,
    Item5,
    Item6
}

public enum CollectStars
{
    STAR_1 = 1,
    STARS_2 = 2,
    STARS_3 = 3
}


public enum RewardedAdsType
{
    GetLifes,
    GetGems,
    GetGoOn
}

public class InitScript : MonoBehaviour
{
    public static InitScript Instance;
    public static int openLevel;


    public static float RestLifeTimer;
    public static string DateOfExit;
    public static DateTime today;
    public static DateTime DateOfRestLife;
    public static string timeForReps;
    private static int Lifes;

    public List<CollectedIngredients> collectedIngredients = new List<CollectedIngredients>();

    public RewardedAdsType currentReward;

    public static int lifes
    {
        get { return InitScript.Lifes; }
        set { InitScript.Lifes = value; }
    }

    public int CapOfLife = 5;
    public float TotalTimeForRestLifeHours = 0;
    public float TotalTimeForRestLifeMin = 15;
    public float TotalTimeForRestLifeSec = 60;
    public int FirstGems = 20;
    public static int Gems;
    public static int waitedPurchaseGems;
    private int BoostExtraMoves;
    private int BoostPackages;
    private int BoostStripes;
    private int BoostExtraTime;
    private int BoostBomb;
    private int BoostColorful_bomb;
    private int BoostHand;
    private int BoostRandom_color;
    public List<AdEvents> adsEvents = new List<AdEvents>();

    public static bool sound = false;
    public static bool music = false;
    private bool adsReady;
    public bool enableUnityAds;
    public bool enableGoogleMobileAds;
    public bool enableChartboostAds;
    public string rewardedVideoZone;
    public string nonRewardedVideoZone;
    public int ShowChartboostAdsEveryLevel;
    public int ShowAdmobAdsEveryLevel;
    private bool leftControl;
#if GOOGLE_MOBILE_ADS
	private InterstitialAd interstitial;
	private AdRequest requestAdmob;
#endif
    public string admobUIDAndroid;
    public string admobUIDIOS;

    public int ShowRateEvery;
    public string RateURL;
    public string RateURLIOS;
    private GameObject rate;
    public int rewardedGems = 4;
    public bool losingLifeEveryGame;
    public static Sprite profilePic;

    public GameObject facebookButton;

    //1.3.3
    public string admobRewardedUIDAndroid;
    public string admobRewardedUIDIOS;

    // Use this for initialization
    void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;
        RestLifeTimer = YandexGame.savesData.restLifeTimer;
        //		if (Application.isEditor)//TODO comment it
        //			PlayerPrefs.DeleteAll ();

        DateOfExit = YandexGame.savesData.dateOfExit;
        Gems = YandexGame.savesData.gems;
        lifes = YandexGame.savesData.lives;
        if (YandexGame.savesData.launched == 0)
        {
            //First lauching
            lifes = CapOfLife;
            Gems = FirstGems;
            YandexGame.savesData.lives =  lifes;
            YandexGame.savesData.lives =  Gems;
            YandexGame.savesData.launched = 1;
            YandexGame.SaveProgress();
        }

        rate = GameObject.Find("CanvasGlobal").transform.Find("Rate").gameObject;
        rate.SetActive(false);
        //rate.transform.SetParent(GameObject.Find("CanvasGlobal").transform);
        //rate.transform.localPosition = Vector3.zero;
        //rate.GetComponent<RectTransform>().anchoredPosition = (Resources.Load("Prefabs/Rate") as GameObject).GetComponent<RectTransform>().anchoredPosition;
        //rate.transform.localScale = Vector3.one;
        gameObject.AddComponent<InternetChecker>();
        GameObject.Find("Music").GetComponent<AudioSource>().volume = YandexGame.savesData.music;
        SoundBase.Instance.GetComponent<AudioSource>().volume = YandexGame.savesData.sound;

        Transform canvas = GameObject.Find("CanvasGlobal").transform;
        foreach (Transform item in canvas)
        {
            item.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            leftControl = true;
        if (Input.GetKeyUp(KeyCode.LeftControl))
            leftControl = false;

        if (Input.GetKeyUp(KeyCode.U))
        {
            for (int i = 1; i < GameObject.Find("Levels").transform.childCount; i++)
            {
                SaveLevelStarsCount(i, 1);
            }
        }
    }

    public void SaveLevelStarsCount(int level, int starsCount)
    {
        Debug.Log($"Stars count {starsCount} of level {level} saved.");
        YandexGame.savesData.LevelStars[GetLevelKey(level)] = starsCount;
        YandexGame.SaveProgress();
    }

    private string GetLevelKey(int number)
    {
        return $"Level.{number:000}.StarsCount";
    }


    public void ShowRewardedAds()
    {
        YandexGame.RewVideoShow(0);
    }

    public void CheckAdsEvents(GameState state)
    {
        foreach (AdEvents item in adsEvents)
        {
            if (item.gameEvent == state)
            {
                if ((LevelManager.THIS.gameStatus == GameState.GameOver ||
                     LevelManager.THIS.gameStatus == GameState.Pause ||
                     LevelManager.THIS.gameStatus == GameState.Playing ||
                     LevelManager.THIS.gameStatus == GameState.PrepareGame ||
                     LevelManager.THIS.gameStatus == GameState.PreWinAnimations ||
                     LevelManager.THIS.gameStatus == GameState.RegenLevel ||
                     LevelManager.THIS.gameStatus == GameState.Win))
                {
                    item.calls++;
                    if (item.calls % item.everyLevel == 0)
                        ShowAdByType(item.adType);
                    // } else {
                    // ShowAdByType (item.adType);
                }
            }
        }
    }

    void ShowAdByType(AdType adType)
    {
        if (adType == AdType.AdmobInterstitial)
            ShowAds(false);
        else if (adType == AdType.UnityAdsVideo)
            ShowVideo();
        else if (adType == AdType.ChartboostInterstitial)
            ShowAds(true);
    }

    public void ShowVideo()
    {
    }

    public void ShowAds(bool chartboost = true)
    {
    }

    public void ShowRate()
    {
    }


    public void CheckRewardedAds(int id)
    {
        if (id != 0) return;

        RewardIcon reward = GameObject.Find("CanvasGlobal").transform.Find("Reward").GetComponent<RewardIcon>();
        if (currentReward == RewardedAdsType.GetGems)
        {
            reward.SetIconSprite(0);

            reward.gameObject.SetActive(true);
            AddGems(rewardedGems);
            GameObject.Find("CanvasGlobal").transform.Find("GemsShop").GetComponent<AnimationManager>().CloseMenu();
        }
        else if (currentReward == RewardedAdsType.GetLifes)
        {
            reward.SetIconSprite(1);
            reward.gameObject.SetActive(true);
            RestoreLifes();
            GameObject.Find("CanvasGlobal").transform.Find("LiveShop").GetComponent<AnimationManager>().CloseMenu();
        }
        else if (currentReward == RewardedAdsType.GetGoOn)
        {
            GameObject.Find("CanvasGlobal").transform.Find("MenuFailed").GetComponent<AnimationManager>().GoOnFailed();
        }
    }

    public void SetGems(int count)
    {
        //1.3.3
        Gems = count;
        YandexGame.savesData.gems = Gems;
        YandexGame.SaveProgress();
    }

    public void AddGems(int count)
    {
        Gems += count;
        YandexGame.savesData.gems = Gems;
        YandexGame.SaveProgress();
#if PLAYFAB || GAMESPARKS
		NetworkManager.currencyManager.IncBalance (count);
#endif
    }

    public void SpendGems(int count)
    {
        SoundBase.Instance.PlaySound(SoundBase.Instance.cash);
        Gems -= count;
        YandexGame.savesData.gems = Gems;
        YandexGame.SaveProgress();
#if PLAYFAB || GAMESPARKS
		NetworkManager.currencyManager.DecBalance (count);
#endif
    }


    public void RestoreLifes()
    {
        lifes = CapOfLife;
        
        YandexGame.savesData.lives = lifes;
        YandexGame.SaveProgress();
    }

    public void AddLife(int count)
    {
        lifes += count;
        if (lifes > CapOfLife)
            lifes = CapOfLife;
        YandexGame.savesData.lives = lifes;
        YandexGame.SaveProgress();
    }

    public int GetLife()
    {
        if (lifes > CapOfLife)
        {
            lifes = CapOfLife;
            YandexGame.savesData.lives = lifes;
            YandexGame.SaveProgress();
        }

        return lifes;
    }

    public void PurchaseSucceded()
    {
        AddGems(waitedPurchaseGems);
        waitedPurchaseGems = 0;
    }

    public void SpendLife(int count)
    {
        if (lifes > 0)
        {
            lifes -= count;
            YandexGame.savesData.lives = lifes;
            YandexGame.SaveProgress();
        }
        //else
        //{
        //    GameObject.Find("Canvas").transform.Find("RestoreLifes").gameObject.SetActive(true);
        //}
    }

    public void BuyBoost(BoostType boostType, int price, int count)
    {
        YandexGame.savesData.BoostCount["" + boostType] = count;
        Debug.Log(YandexGame.savesData.BoostCount["" + boostType]);
        YandexGame.SaveProgress();
#if PLAYFAB || GAMESPARKS
		NetworkManager.dataManager.SetBoosterData ();
#endif

        //   ReloadBoosts();
    }

    public void SpendBoost(BoostType boostType)
    {
        YandexGame.savesData.BoostCount["" + boostType] = YandexGame.savesData.BoostCount.GetValueOrDefault("" + boostType) - 1;
        YandexGame.SaveProgress();
#if PLAYFAB || GAMESPARKS
		NetworkManager.dataManager.SetBoosterData ();
#endif
    }
    //void ReloadBoosts()
    //{
    //    BoostExtraMoves = PlayerPrefs.GetInt("" + BoostType.ExtraMoves);
    //    BoostPackages = PlayerPrefs.GetInt("" + BoostType.Packages);
    //    BoostStripes = PlayerPrefs.GetInt("" + BoostType.Stripes);
    //    BoostExtraTime = PlayerPrefs.GetInt("" + BoostType.ExtraTime);
    //    BoostBomb = PlayerPrefs.GetInt("" + BoostType.Bomb);
    //    BoostColorful_bomb = PlayerPrefs.GetInt("" + BoostType.Colorful_bomb);
    //    BoostHand = PlayerPrefs.GetInt("" + BoostType.Hand);
    //    BoostRandom_color = PlayerPrefs.GetInt("" + BoostType.Random_color);

    //}
    //public void onMarketPurchase(PurchasableVirtualItem pvi, string payload, Dictionary<string, string> extra)
    //{
    //    PurchaseSucceded();
    //}

    void OnApplicationFocus(bool focusStatus)
    {
        //1.3.3
        if (MusicBase.Instance)
        {
            MusicBase.Instance.GetComponent<AudioSource>().Play();
        }
    }


    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            if (RestLifeTimer > 0)
            {
                YandexGame.savesData.restLifeTimer = RestLifeTimer;
            }

            YandexGame.savesData.lives = lifes;
            YandexGame.savesData.dateOfExit = DateTime.Now.ToString();
            YandexGame.SaveProgress();
        }
    }

    void OnApplicationQuit()
    {
        //1.4  added 
        if (RestLifeTimer > 0)
        {
            YandexGame.savesData.restLifeTimer = RestLifeTimer;
        }

        YandexGame.savesData.lives = lifes;
        YandexGame.savesData.dateOfExit = DateTime.Now.ToString();
        YandexGame.SaveProgress();
    }

    public void OnLevelClicked(object sender, LevelReachedEventArgs args)
    {
        if (EventSystem.current.IsPointerOverGameObject(-1))
            return;
        if (!GameObject.Find("CanvasGlobal").transform.Find("MenuPlay").gameObject.activeSelf &&
            !GameObject.Find("CanvasGlobal").transform.Find("GemsShop").gameObject.activeSelf &&
            !GameObject.Find("CanvasGlobal").transform.Find("LiveShop").gameObject.activeSelf)
        {
            YandexGame.savesData.openLevel = args.Number;
        
            YandexGame.SaveProgress();
            LevelManager.THIS.MenuPlayEvent();
            LevelManager.THIS.LoadLevel();
            openLevel = args.Number;
            //  currentTarget = targets[args.Number];
            GameObject.Find("CanvasGlobal").transform.Find("MenuPlay").gameObject.SetActive(true);
        }
    }

    void OnEnable()
    {
        LevelsMap.LevelSelected += OnLevelClicked;
        YandexGame.RewardVideoEvent += CheckRewardedAds;
    }

    void OnDisable()
    {
        LevelsMap.LevelSelected -= OnLevelClicked;
        YandexGame.RewardVideoEvent -= CheckRewardedAds;

        //		if(RestLifeTimer>0){
       
        YandexGame.savesData.restLifeTimer = RestLifeTimer;
        YandexGame.savesData.lives = lifes; 
        YandexGame.savesData.dateOfExit = DateTime.Now.ToString();
        YandexGame.SaveProgress();
#if GOOGLE_MOBILE_ADS
		interstitial.OnAdLoaded -= HandleInterstitialLoaded;
		interstitial.OnAdFailedToLoad -= HandleInterstitialFailedToLoad;
#endif
    }
}