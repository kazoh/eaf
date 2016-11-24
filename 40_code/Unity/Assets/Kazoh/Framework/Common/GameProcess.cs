using UnityEngine;
using System.Collections;
using System;

using Kazoh.Table;

public class GameProcess : MonoBehaviour
{
    public enum Server
    {
        DEV = 0,
        TEST,
        LIVE,
    }

    const string PAGE_URL = "https://play.google.com/apps/testing/com.mtor.eaf";

    public Server TargetServer;
    public string Version;          // 플레이어 세팅의 앱 버전과 동일하게 입력할 것.
    public string ShortVersion;     // 플레이어 세팅의 숏버전과 동일하게 입력할 것.
    public int VersionCode;         // 플레이어 세팅의 버전 코드와 동일하게 입력할 것.
    public int ConfigVersion;       // 밸런스 테이블 및 AI 테이블 데이터 버전.
    public int TipNum;

    public bool ClearCache;
    public SystemLanguage Language = SystemLanguage.Korean;

    public UIPanel LoadingPanel;
    public UIPanel PopupPanel;
    public GameObject PrefLoading;
    public GameObject PrefServerLoading;
    public GameObject PrefPopup;
    public Component_UI_Loading Loader { get; private set; }
    public Component_UI_ServerLoading ServerLoader { get; private set; }
    public Component_Popup_Common Popup { get; private set; }

    private SoundManager soundManager;
    //private Transform layerRoot;

    //private FBManager fbManager;

    //private GPGSManager gpgsManager;

    private GameSettingManager settingManager;
    private GameDataManager gdm;
    private ResourceManager rm;
    private Manager_Mission mm;
    private HttpManager hm;
    private IAPManager iap;

    private GameConfig config;

    private static GameProcess instance;
    public static GameProcess Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject go = GameObject.Find("Process");
                if(go == null)
                {
                    go = (GameObject)Instantiate(Resources.Load("Prefabs/pfGameProcess"));
                    if (go != null) go.name = "Process";
                }
                instance = go.GetComponent<GameProcess>();
                instance.Init();
            }

            return instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Init()
    {

#if UNITY_EDITOR
        if (ClearCache) PlayerPrefs.DeleteAll();
#endif

        /* 절전 모드 해제 */
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        /* 프레임 고정 */
        Application.targetFrameRate = 60;

        /* 암호화 키 초기화 */
        EncryptedPlayerPrefs.InitKey();

        /* 레이어 루트 생성 */
        //GameObject go = new GameObject("LayerRoot");
        //layerRoot = go.transform;
        //layerRoot.parent = transform; 

        /* 팝업 패널 초기화 */
        GameObject go = NGUITools.AddChild(PopupPanel.gameObject, PrefPopup);
        Popup = go.GetComponent<Component_Popup_Common>();
        if (Popup != null)
        {
            Popup.Init();
        }

        /* 로딩 패널 초기화 */
        go = NGUITools.AddChild(LoadingPanel.gameObject, PrefLoading);
        Loader = go.GetComponent<Component_UI_Loading>();
        if (Loader != null)
        {
            Loader.Init();
            Loader.Hide();
        }

        /* 서버로딩 패널 초기화 */
        go = NGUITools.AddChild(LoadingPanel.gameObject, PrefServerLoading);
        ServerLoader = go.GetComponent<Component_UI_ServerLoading>();
        if (ServerLoader != null)
        {
            ServerLoader.Init();
            ServerLoader.Hide();
        }

        /* 사운드 매니저 생성 */
        GameObject sm = (GameObject)Instantiate(Resources.Load("Prefabs/pfSoundManager")); //, typeof(GameObject)));
        if (sm != null)
        {
            sm.name = "SoundManager";
            sm.transform.parent = transform;
            soundManager = sm.GetComponent<SoundManager>();
        }

        /* 리소스 매니저 생성 */
        rm = gameObject.AddComponent<ResourceManager>();
        rm.Init();

        /* 세팅 매니저 생성 */
        settingManager = new GameSettingManager();

        /* 유저 데이터 매니저 생성 */
        gdm = new GameDataManager();
        gdm.Init();
#if UNITY_EDITOR
        //gdm.TestSetUserData(5000000, 50, 5, 100, 3, 15);
        //gdm.TestMapClear(50000, 3);
        //gdm.TestMapClear(50001, 2);
        //gdm.TestMapClear(50002, 1);
#endif

        /* 미션 매니저 생성 */
        mm = new Manager_Mission();
        mm.Init();

        /* http 매니저 생성 */
        hm = gameObject.AddComponent<HttpManager>();
        hm.Init();

        /* 게임 설정 정보 생성 */
        config = new GameConfig();

        /* 결제 매니저 생성 */
        iap = gameObject.AddComponent<IAPManager>();
        iap.Init();

        ///* 페이스북 메니저 설정 */
        //if (fbManager == null)
        //{
        //    fbManager = new FBManager();
        //    fbManager.Init();
        //}

        /* 구글 플레이 서비스 매니저 생성 */
        //if (gpgsManager == null)
        //{
        //    gpgsManager = gameObject.AddComponent<GPGSManager>();
        //    gpgsManager.Init();
        //}
    }

    void OnDestroy()
    {
#if UNITY_EDITOR || DEBUG_MODE
        Debug.Log("게임 프로세스 소멸!!");
#endif
    }

    void OnApplicationQuit()
    {
#if UNITY_EDITOR || DEBUG_MODE
        Debug.Log("Application Quit.");
#endif
    }

    void OpenUrlAndQuit()
    {
        Application.OpenURL(PAGE_URL);
        Quit();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public static void LoadTable()
    {
        try
        {
            TableManager.Load(GameProcess.Instance.ConfigVersion);
        }
        catch(Exception e)
        {
            throw e;
        }
    }

    #region // Sound Func

    public static void PlaySound(SOUND_EFFECT type, string bgm = "")
    {
        if (string.IsNullOrEmpty(bgm)) Instance.soundManager.PlaySound(type);
        else Instance.soundManager.PlaySound(type, bgm);
    }

    public static void StopBGM()
    {
        Instance.soundManager.StopBGM();
    }

    public static float GetEffectLength(SOUND_EFFECT type)
    {
        return Instance.soundManager.EffectLength(type);
    }

    public void SetVolumnBGM(float value)
    {
        soundManager.SetVolumeBGM(value);
    }

    public void SetVolumnEffect(float value)
    {
        soundManager.SetVolumeEffect(value);
    }
    #endregion // Sound Func

    public static GameSettingManager GetSettingManager()
    {
        return Instance.settingManager;
    }

    public static GameDataManager GetGameDataManager()
    {
        return Instance.gdm;
    }

    public static ResourceManager GetResourceManager()
    {
        return Instance.rm;
    }

    public static Manager_Mission GetMissionManager()
    {
        return Instance.mm;
    }

    public static HttpManager GetHttpManager()
    {
        return Instance.hm;
    }

    public static GameConfig GetGameConfig()
    {
        return Instance.config;
    }

    public static void SetGameConfig(string _data)
    {
        try
        {
            Instance.config.SetGameConfig(_data);
        }
        catch(Exception e)
        {
            throw e;
        }
    }

    public static void OpenReview()
    {
#if UNITY_EDITOR
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.mtor.kksjp");
#elif UNITY_ANDROID
        Application.OpenURL("market://details?id=com.mtor.kksjp");
#elif UNITY_IPHONE
        Application.OpenURL("itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?id=1110153576&onlyLatestVersion=true&pageNumber=0&sortOrdering=1&type=Purple+Software");
#endif
    }

    public static void OpenFacebook()
    {
        Application.OpenURL("https://www.facebook.com/kingofhighschools");
    }

    public string GetTipName()
    {
        int tipNum = TipNum > 0 ? UnityEngine.Random.Range(1, TipNum) : 1;
        return string.Format("STR_TIP_{0:00}", tipNum);
    }

    public static void ShowError(GameException e)
    {
        string title = string.Format("{0}({1})", TableManager.GetString("STR_TITLE_ERROR") , (int)e.Code);
        string msg = e.Msg;
        string buttonText = TableManager.GetString("STR_UI_OK");
        Action callback = null;
        if (e.Code == GameException.ErrorCode.InvalidConfigVer) callback = Instance.OpenUrlAndQuit;
        else if(e.Code == GameException.ErrorCode.Unknown)
        {
            callback = Instance.Quit;
            string log = e.StackTrace;
            DBManager.SaveErrorLog(log);
        }
        else if (e.IsCritical) callback = Instance.Quit;
        Instance.Popup.Show(NoticeType.OK, title, msg, buttonText, callback);
    }

    public static void ShowError(GameException e, string title, string text)
    {
        string msg = e.Msg;
        Action callback = null;
        if (e.Code == GameException.ErrorCode.InvalidConfigVer) callback = Instance.OpenUrlAndQuit;
        else if (e.Code == GameException.ErrorCode.Unknown)
        {
            callback = Instance.Quit;
            string log = e.StackTrace;
            DBManager.SaveErrorLog(log);
        }
        else if (e.IsCritical) callback = Instance.Quit;
        Instance.Popup.Show(NoticeType.OK, title, msg, text, callback);
    }

    public static void ShowLoading()
    {
        if (Instance.Loader != null) Instance.Loader.Show();
    }

    public static void ShowServerLoading()
    {
        if (Instance.ServerLoader != null) Instance.ServerLoader.Show();
    }

    public static void ShowPopup(NoticeType type, string title, string msg, string text, Action callback, NGUIText.Alignment alignment = NGUIText.Alignment.Automatic)
    {
        Instance.Popup.Show(type,title,msg,text,callback,alignment);
    }

    public static void ShowPopup(NoticeType type, string title, string msg, string text1, string text2, Action callback1, Action callback2, NGUIText.Alignment alignment = NGUIText.Alignment.Automatic)
    {
        Instance.Popup.Show(type, title, msg, text1, callback1, text2, callback2, alignment);
    }

    public static void HideLoading()
    {
        if (Instance.Loader != null) Instance.Loader.Hide();
    }

    public static void HideServerLoading()
    {
        if (Instance.ServerLoader != null) Instance.ServerLoader.Hide();
    }

    #region // 구글 플레이 서비스 Function
    //public static bool ShowLB()
    //{
    //    return Instance.gpgsManager.ShowLeaderboard();
    //}

    //public static bool ShowAchievement()
    //{
    //    return Instance.gpgsManager.ShowAchievement();
    //}

    //public static bool PostScoreTurn(int score)
    //{
    //    Instance.gpgsManager.Post(GPGSManager.TaskType.LeaderBoard, GPGSIds.leaderboard_hall_of_fame, score);
    //    return true;
    //}

    //public static bool PostScoreAtk(int score)
    //{
    //    Instance.gpgsManager.Post(GPGSManager.TaskType.LeaderBoard, GPGSIds.leaderboard_atk_rank, score);
    //    return true;
    //}

    //public static bool PostScoreDef(int score)
    //{
    //    Instance.gpgsManager.Post(GPGSManager.TaskType.LeaderBoard, GPGSIds.leaderboard_def_rank, score);
    //    return true;
    //}

    //public static bool PostScoreGold(int score)
    //{
    //    Instance.gpgsManager.Post(GPGSManager.TaskType.LeaderBoard, GPGSIds.leaderboard_richest_rank, score);
    //    return true;
    //}

    //public static bool PostAchivementAtk(int score)
    //{
    //    if (score > 499) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_a_novice_fighter, 100);
    //    if (score > 999) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_strong_man, 100);
    //    if (score > 1999) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_street_fighter, 100);
    //    if (score > 2999) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_the_wind_of_fighter, 100);
    //    if (score > 4999) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_reaper, 100);
    //    return true;
    //}

    //public static bool PostAchivementDef(int score)
    //{
    //    if (score > 499) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_a_novice_tanker, 100);
    //    if (score > 999) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_crusaders, 100);
    //    if (score > 1999) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_aegis, 100);
    //    if (score > 2999) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_adamant, 100);
    //    if (score > 4999) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_immortal, 100);
    //    return true;
    //}

    //public static bool PostAchivementGold(int score)
    //{
    //    if (score > 9999999) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_rich_person_i, 100);
    //    if (score > 99999999) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_rich_person_ii, 100);
    //    if (score > 299999999) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_rich_person_iii, 100);
    //    if (score > 499999999) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_rich_person_iv, 100);
    //    if (score > 999999998) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_rich_person_v, 100);
    //    return true;
    //}

    //public static bool PostAchivementTurn(int id)
    //{
    //    Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_the_first_unification, 100);
    //    Instance.gpgsManager.Post(GPGSManager.TaskType.IncreaseAchievement, GPGSIds.achievement_famous_in_tokyo, 1);
    //    Instance.gpgsManager.Post(GPGSManager.TaskType.IncreaseAchievement, GPGSIds.achievement_legend_of_tokyo, 1);
    //    Instance.gpgsManager.Post(GPGSManager.TaskType.IncreaseAchievement, GPGSIds.achievement_king_of_tokyo, 1);

    //    switch (id)
    //    {
    //        case 10001: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_oda, 100); break;
    //        case 10002: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_setagaya, 100); break;
    //        case 10003: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_meguro, 100); break;
    //        case 10004: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_shinagawa, 100); break;
    //        case 10005: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_minato, 100); break;
    //        case 10006: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_shibuya, 100); break;
    //        case 10007: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_suginami, 100); break;
    //        case 10008: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_nakano, 100); break;
    //        case 10009: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_shinjuku, 100); break;
    //        case 10010: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_chiyoda, 100); break;
    //        case 10011: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_chuo, 100); break;
    //        case 10012: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_koto, 100); break;
    //        case 10013: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_edogawa, 100); break;
    //        case 10014: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_sumida, 100); break;
    //        case 10015: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_taito, 100); break;
    //        case 10016: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_bunkyo, 100); break;
    //        case 10017: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_dosima, 100); break;
    //        case 10018: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_nerima, 100); break;
    //        case 10019: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_itabashi, 100); break;
    //        case 10020: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_kita, 100); break;
    //        case 10021: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_arakawa, 100); break;
    //        case 10022: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_katsushika, 100); break;
    //        case 10023: Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_lord_of_the_adachi, 100); break;
    //    }
    //    return true;
    //}

    //public static bool PostAchivementCmd(GameEnum.CommandType _type)
    //{
    //    switch (_type)
    //    {
    //        case GameEnum.CommandType.SUPPORT:
    //            Instance.gpgsManager.Post(GPGSManager.TaskType.IncreaseAchievement, GPGSIds.achievement_aid_100_times, 1);
    //            Instance.gpgsManager.Post(GPGSManager.TaskType.IncreaseAchievement, GPGSIds.achievement_aid_1000_times, 1);
    //            Instance.gpgsManager.Post(GPGSManager.TaskType.IncreaseAchievement, GPGSIds.achievement_aid_10000_times, 1);
    //            break;

    //        case GameEnum.CommandType.COLLECTE:
    //            Instance.gpgsManager.Post(GPGSManager.TaskType.IncreaseAchievement, GPGSIds.achievement_collect_100_times, 1);
    //            Instance.gpgsManager.Post(GPGSManager.TaskType.IncreaseAchievement, GPGSIds.achievement_collect_1000_times, 1);
    //            Instance.gpgsManager.Post(GPGSManager.TaskType.IncreaseAchievement, GPGSIds.achievement_collect_10000_times, 1);
    //            break;

    //        case GameEnum.CommandType.ALLY:
    //            Instance.gpgsManager.Post(GPGSManager.TaskType.IncreaseAchievement, GPGSIds.achievement_ally_100_times, 1);
    //            Instance.gpgsManager.Post(GPGSManager.TaskType.IncreaseAchievement, GPGSIds.achievement_ally_1000_times, 1);
    //            Instance.gpgsManager.Post(GPGSManager.TaskType.IncreaseAchievement, GPGSIds.achievement_ally_10000_times, 1);
    //            break;

    //        case GameEnum.CommandType.BREAK:
    //            Instance.gpgsManager.Post(GPGSManager.TaskType.IncreaseAchievement, GPGSIds.achievement_break_100_times, 1);
    //            Instance.gpgsManager.Post(GPGSManager.TaskType.IncreaseAchievement, GPGSIds.achievement_break_1000_times, 1);
    //            Instance.gpgsManager.Post(GPGSManager.TaskType.IncreaseAchievement, GPGSIds.achievement_break_10000_times, 1);
    //            break;

    //        case GameEnum.CommandType.ATTACK:
    //            Instance.gpgsManager.Post(GPGSManager.TaskType.IncreaseAchievement, GPGSIds.achievement_attack_100_times, 1);
    //            Instance.gpgsManager.Post(GPGSManager.TaskType.IncreaseAchievement, GPGSIds.achievement_attack_1000_times, 1);
    //            Instance.gpgsManager.Post(GPGSManager.TaskType.IncreaseAchievement, GPGSIds.achievement_attack_10000_times, 1);
    //            break;

    //    }
    //    return true;
    //}

    //public static bool PostAchivementAddOption(TableEnum.OptionCategory _type, int _id, int _count)
    //{
    //    switch (_type)
    //    {
    //        case TableEnum.OptionCategory.Human:
    //            if (_id == 50800) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_hire_i, 100);
    //            else if (_id == 50801) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_hire_ii, 100);
    //            else if (_id == 50802) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_hire_iii, 100);
    //            if (_count > 2) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_hire_iv, 100);
    //            break;

    //        case TableEnum.OptionCategory.Structure:
    //            if (_id == 50803) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_build_i, 100);
    //            else if (_id == 50804) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_build_ii, 100);
    //            else if (_id == 50805) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_build_iii, 100);
    //            if (_count > 2) Instance.gpgsManager.Post(GPGSManager.TaskType.Achievement, GPGSIds.achievement_build_iv, 100);
    //            break;
    //    }
    //    return true;
    //}
    #endregion // 구글 플레이 서비스 Function

    #region // Facebook Function
    public void Share(string region, string force, int value, System.Action callback)
    {
#if UNITY_ANDROID
        //switch (Application.internetReachability)
        //{
        //    case NetworkReachability.NotReachable:
        //        callback();
        //        break;

        //    case NetworkReachability.ReachableViaCarrierDataNetwork:
        //    case NetworkReachability.ReachableViaLocalAreaNetwork:
        //        if (lastShareDate == null || (System.DateTime.Now - lastShareDate).TotalHours > 4)
        //        {
        //            string format = Kazoh.Table.TableManager.GetString("FEED_LINK_DESC");
        //            string msg = string.Format(format, region, force, value);
        //            fbManager.Share(msg, callback);
        //        }
        //        else
        //        {
        //            callback();
        //        }
        //        break;
        //}
        callback();
#else
        callback();
#endif
    }

    public void ShareGameOver(string msg, System.Action callback)
    {
#if UNITY_ANDROID
        //switch (Application.internetReachability)
        //{
        //    case NetworkReachability.NotReachable:
        //        callback();
        //        break;

        //    case NetworkReachability.ReachableViaCarrierDataNetwork:
        //    case NetworkReachability.ReachableViaLocalAreaNetwork:
        //        fbManager.Share(msg, callback);
        //        break;
        //}
        callback();
#else
        callback();
#endif
    }

    public bool CanShare()
    {
#if UNITY_ANDROID
        //switch (Application.internetReachability)
        //{
        //    case NetworkReachability.NotReachable: return false;

        //    case NetworkReachability.ReachableViaCarrierDataNetwork:
        //    case NetworkReachability.ReachableViaLocalAreaNetwork:
        //        if (lastShareDate == null || (System.DateTime.Now - lastShareDate).TotalHours > 4) return true;
        //        return false;
        //    default: return false;
        //}
        return false;
#else
        return false;
#endif
    }
    #endregion // Facebook Function

    #region // IAP
    public static void BuyInappItem(UnityEngine.Purchasing.ProductType _type, int _idx, Action _callback)
    {
        try
        {
            switch(_type)
            {
                case UnityEngine.Purchasing.ProductType.Consumable:
                    Instance.iap.BuyConsumable(_idx, delegate ()
                    {
                        if (_callback != null) _callback();
                    });
                    break;
                case UnityEngine.Purchasing.ProductType.NonConsumable:
                    Instance.iap.BuyNonConsumable(_idx, delegate ()
                    {
                        if (_callback != null) _callback();
                    });
                    break;
                case UnityEngine.Purchasing.ProductType.Subscription:
                    Instance.iap.BuySubscription(_idx, delegate ()
                    {
                        if (_callback != null) _callback();
                    });
                    break;
            }
        }
        catch (GameException e)
        {
            ShowError(e);
        }
        catch (Exception e)
        {
            ShowError(new GameException(GameException.ErrorCode.NotPurchasingProduct, e.Message));
        }
    }
    #endregion    
}
