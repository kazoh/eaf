using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DBManager {

    public enum FuncCode
    {
        NONE = 0,
        CREATE_ACCOUNT = 1,
        SIGN_IN = 2,
        CHANGE_PW = 3,
        FIND_PW = 4,
        SELECT_USER_DATA = 11,
        UPDATE_USER_DATA = 12,
        NEW_USER_DATA = 13,
        CHECK_LOG_SERVER = 21,
        SAVE_LOG = 22,
        GET_CONFIG = 31,
        GET_SERVER_STATE = 41,
        SAVE_ERROR_LOG = 51,
        SELECT_RANK = 61,
        UPDATE_RANK = 62,
    }

    private HttpManager httpManager;

    readonly private string URL_ACCOUNT;
    readonly private string URL_USER_DATA;
    readonly private string URL_LOG;
    readonly private string URL_CONFIG;
    readonly private string URL_SERVER_CHECK;
    readonly private string URL_ERROR_LOG;
    readonly private string URL_RANK;

    private static DBManager instance;
    public static DBManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DBManager();
                instance.httpManager = GameProcess.GetHttpManager();
            }

            return instance;
        }
    }

    private DBManager()
    {
        switch(GameProcess.Instance.TargetServer)
        {
            case GameProcess.Server.DEV:
                URL_ACCOUNT = "https://script.google.com/macros/s/AKfycbx7ALLUBytLFxUGJmFzC6i4M8iwJaYxwChzxylZ6QIo6wA6eA/exec";
                URL_USER_DATA = "https://script.google.com/macros/s/AKfycbx2WcgVg61bWMeI-y7JcqMDZFh3E4HEF4TnmXw-in-EEamXcA/exec";
                URL_LOG = "https://script.google.com/macros/s/AKfycbwP9LD8OBJ6Ai7LG2fhRn9d9b6DIAtiPVWb6GVoGEPnFQXUyuU/exec";
                URL_CONFIG = "https://script.google.com/macros/s/AKfycbzzWKD1bfzwGJu3-9tMmfSAxOhs4L4yDuUtf4QZvsjMY9rOop8/exec";
                URL_SERVER_CHECK = "https://script.google.com/macros/s/AKfycbzXB04VJ8oWcdL6CRwdMPRiZfC42Pk-WW9KbmsN9CcVO-sfXA/exec";
                URL_ERROR_LOG = "https://script.google.com/macros/s/AKfycbx6KCBTuufwIliEWCkQxm1m0YksXovtv7x1Q4rD3dsDLe5eMG8/exec";
                URL_RANK = "https://script.google.com/macros/s/AKfycbwe_WuY1zHO9LNpjTMqaSJWs87kOb7ckjUYrgPXWleTD9WjezA/exec";
                break;
            case GameProcess.Server.TEST:
                URL_ACCOUNT = "https://script.google.com/macros/s/AKfycbwX8UhLmtDvN1KLy13_YbA_yYDuhxOitfRkCORU7ZT1TID-mmg/exec";
                URL_USER_DATA = "https://script.google.com/macros/s/AKfycbwfEUmrd799lW5hR_J18Olormn-h3PnEaoFFcVUDVm7lQXQhkw/exec";
                URL_LOG = "https://script.google.com/macros/s/AKfycbzhd9n-9elZ1FsMKLMUtTH2geo9MEM5OtZurTX3Ckw5-hK14aM/exec";
                URL_CONFIG = "https://script.google.com/macros/s/AKfycbzA3lXzT7l-9TebXIovk-K4MTd2mlfn_sXs4GdS-AFYJTqFQWU/exec";
                URL_SERVER_CHECK = "https://script.google.com/macros/s/AKfycby6_J_VJXWzaVpUWeuKuLY1_ulhRusK5erFFI9Yzhh8IGsdug/exec";
                URL_ERROR_LOG = "https://script.google.com/macros/s/AKfycbxOtCIkkjkTu_b6pDfCLiTKIVariBc5R4qCb7w7OH3xrr0CPME/exec";
                URL_RANK = "https://script.google.com/macros/s/AKfycbxl6pU7kI3t0L49-FPBCfuQuBBH69AMbrUacvBcXbJDp0o3TQ/exec";
                break;
            case GameProcess.Server.LIVE:
                URL_ACCOUNT = "https://script.google.com/macros/s/AKfycbzxD4JPBHzBPPyC4IX09jVS7KdFdeWVj0cwSN7sYeQlIs7nIg/exec";
                URL_USER_DATA = "https://script.google.com/macros/s/AKfycbyHcz4UNn6TsfnIvEblEFJCkycL6jehl0hZyjhV1cgJ6FYA4wY/exec";
                URL_LOG = "https://script.google.com/macros/s/AKfycbyLAKBvXPqzeo8vMsMlLpGwKacQkYuKuw9Kq6MOEIMHzR27Lw/exec";
                URL_CONFIG = "https://script.google.com/macros/s/AKfycbxdcJI3rMNv_55bEUf75Bpeu60A3XhQkgXAppu_wlqW409m6w/exec";
                URL_SERVER_CHECK = "https://script.google.com/macros/s/AKfycbyXHYLCNM6rynSiuM5JJozAnCQI-fDQ--Dnetvm7z27oAK6Og/exec";
                URL_ERROR_LOG = "https://script.google.com/macros/s/AKfycbw8sHytkQ6RexTwncFp1xSFqzYWA_WOfXfwbHuDBIGe_fUPHA/exec";
                URL_RANK = "https://script.google.com/macros/s/AKfycbyr-r9ezPy1gTyjX8GUEaaSHqgE6qZ8NrrsN7NwLTctgB3cDSQ/exec";
                break;
        }
    }

    public static void SignIn(string _email, string _pw, Action<int,string> _callback)
    {
        if (!CheckNetwork()) throw new GameException(GameException.ErrorCode.NoNetwork);

        string pw = EncryptedPlayerPrefs.Md5(_pw);
        string _url = string.Format("{0}?CODE={1}&EMAIL={2}&PW={3}", Instance.URL_ACCOUNT, (int)FuncCode.SIGN_IN, _email, pw);

        Instance.httpManager.CallRequest(_url, true, delegate (WWW www)
        {
            IDictionary dict = MiniJSON.Json.Deserialize(www.text) as IDictionary;
            if (CheckResponse(dict))
            {
                int guid = 0;
                string timeStr = "";

                if (dict.Contains("guid")) guid = Convert.ToInt32(dict["guid"]);
                if (dict.Contains("tick")) timeStr = Convert.ToString(dict["tick"]);

                if (_callback != null) _callback(guid, timeStr);
            }
        });
    }

    public static void CreateAccount(string _email, string _pw, Action<int,string> _callback)
    {
        if (!CheckNetwork()) throw new GameException(GameException.ErrorCode.NoNetwork);

        string pw = EncryptedPlayerPrefs.Md5(_pw);
        string _url = string.Format("{0}?CODE={1}&EMAIL={2}&PW={3}",Instance.URL_ACCOUNT,(int)FuncCode.CREATE_ACCOUNT,_email,pw);
        Instance.httpManager.CallRequest(_url, true, delegate (WWW www)
         {
             IDictionary dict = MiniJSON.Json.Deserialize(www.text) as IDictionary;
             if(CheckResponse(dict))
             {
                 int guid = 0;
                 string timeStr = "";

                 if (dict.Contains("guid")) guid = Convert.ToInt32(dict["guid"]);
                 if (dict.Contains("tick")) timeStr = Convert.ToString(dict["tick"]);

                 if (guid > 0)
                 {
                     string _userId = EncryptedPlayerPrefs.Md5(guid + _email);

                     // 유저 데이터 테이블에 등록.
                     _url = string.Format("{0}?CODE={1}&ID={2}&DATA={3}", Instance.URL_USER_DATA, (int)FuncCode.NEW_USER_DATA, _userId,"");
                     Instance.httpManager.CallRequest(_url, true, delegate (WWW www2)
                     {
                         dict = MiniJSON.Json.Deserialize(www2.text) as IDictionary;
                         if (CheckResponse(dict))
                         {
                             if (_callback != null) _callback(guid, timeStr);
                         }
                     });
                 }
                 else if (_callback != null) _callback(guid, timeStr);
             }
         });
    }

    public static void ChangePw(int _id, string _pw, string _pw2, Action<bool> _callback)
    {
        if (!CheckNetwork()) throw new GameException(GameException.ErrorCode.NoNetwork);

        string pw = EncryptedPlayerPrefs.Md5(_pw);
        string pw2 = EncryptedPlayerPrefs.Md5(_pw2);
        string _url = string.Format("{0}?CODE={1}&ID={2}&PW={3}&PW2={4}", Instance.URL_ACCOUNT, (int)FuncCode.CHANGE_PW, _id, pw, pw2);
        Instance.httpManager.CallRequest(_url, true, delegate (WWW www)
        {
            IDictionary dict = MiniJSON.Json.Deserialize(www.text) as IDictionary;
            if (CheckResponse(dict))
            {
                bool fail = false;

                if (dict["result"].Equals("fail"))
                {
                    fail = true;
                }

                if (_callback != null) _callback(fail);
            }
        });
    }

    public static void FindPw(string _email, string _pw, string _title, string _msg, Action<bool> _callback)
    {
        if (!CheckNetwork()) throw new GameException(GameException.ErrorCode.NoNetwork);

        string pw = EncryptedPlayerPrefs.Md5(_pw);
        _title = WWW.EscapeURL(_title);
        _msg = WWW.EscapeURL(_msg);
        string _url = string.Format("{0}?CODE={1}&EMAIL={2}&PW={3}&TITLE={4}&MSG={5}", Instance.URL_ACCOUNT, (int)FuncCode.FIND_PW, _email, pw, _title, _msg);
        Debug.Log(_url);
        Instance.httpManager.CallRequest(_url, true, delegate (WWW www)
        {
            IDictionary dict = MiniJSON.Json.Deserialize(www.text) as IDictionary;
            if (CheckResponse(dict))
            {
                bool fail = false;

                if (dict["result"].Equals("fail"))
                {
                    fail = true;
                }

                if (_callback != null) _callback(fail);
            }
        });
    }

    public static void SelectData(string _guid, Action<string, string> _callback)
    {
        if (!CheckNetwork()) throw new GameException(GameException.ErrorCode.NoNetwork);

        string _url = string.Format("{0}?CODE={1}&ID={2}&KEY={3}", Instance.URL_USER_DATA, (int)FuncCode.SELECT_USER_DATA, _guid, GameProcess.GetGameDataManager().GetToken());
        Instance.httpManager.CallRequest(_url, true, delegate (WWW www)
        {
            IDictionary dict = MiniJSON.Json.Deserialize(www.text) as IDictionary;
            if (CheckResponse(dict))
            {
                string data = "";
                string timeStr = "";

                if (dict["result"].Equals("fail") || dict["result"].Equals("error"))
                {
                    GameException.ErrorCode errCode = (GameException.ErrorCode)Convert.ToInt32(dict["code"]);
                    GameProcess.ShowError(new GameException(errCode));
                    return;
                }

                if (dict.Contains("data")) data = Convert.ToString(dict["data"]);
                if (dict.Contains("tick")) timeStr = Convert.ToString(dict["tick"]);

                if (_callback != null) _callback(data, timeStr);
            }
        });
    }

    public static void UpdateData(string _guid, string _data, Action<string> _callback)
    {
        if (!CheckNetwork()) throw new GameException(GameException.ErrorCode.NoNetwork);

        _data = WWW.EscapeURL(_data);
        string _url = string.Format("{0}?CODE={1}&ID={2}&DATA={3}&KEY={4}", Instance.URL_USER_DATA, (int)FuncCode.UPDATE_USER_DATA, _guid, _data, GameProcess.GetGameDataManager().GetToken());
        Instance.httpManager.CallRequest(_url, false, delegate (WWW www)
        {
            IDictionary dict = MiniJSON.Json.Deserialize(www.text) as IDictionary;
            if (CheckResponse(dict))
            {
                string timeStr = "";

                if (dict["result"].Equals("fail") || dict["result"].Equals("error"))
                {
                    GameException.ErrorCode errCode = (GameException.ErrorCode)Convert.ToInt32(dict["code"]);
                    GameProcess.ShowError(new GameException(errCode));
                    return;
                }
                
                if (dict.Contains("tick")) timeStr = Convert.ToString(dict["tick"]);

                if (_callback != null) _callback(timeStr);
            }
        });
    }

    public static void CheckLogServer(Action<bool, string> _callback)
    {
        if (!CheckNetwork()) throw new GameException(GameException.ErrorCode.NoNetwork);

        string _url = string.Format("{0}?CODE={1}", Instance.URL_LOG, (int)FuncCode.CHECK_LOG_SERVER);
        Instance.httpManager.CallRequest(_url, true, delegate (WWW www)
        {
            IDictionary dict = MiniJSON.Json.Deserialize(www.text) as IDictionary;
            if (CheckResponse(dict))
            {
                string timeStr = "";
                bool fail = false;

                if (dict["result"].Equals("fail") || dict["result"].Equals("error"))
                {
                    fail = true;
                }

                if (dict.Contains("tick")) timeStr = Convert.ToString(dict["tick"]);

                if (_callback != null) _callback(fail, timeStr);
            }
        });
    }

    public static void SaveLog(string _data, Action<bool> _callback)
    {
        if (!CheckNetwork()) throw new GameException(GameException.ErrorCode.NoNetwork);

        _data = WWW.EscapeURL(_data);
        string _url = string.Format("{0}?CODE={1}&LOG={2}", Instance.URL_LOG, (int)FuncCode.SAVE_LOG, _data);
        Instance.httpManager.CallRequest(_url, true, delegate (WWW www)
        {
            IDictionary dict = MiniJSON.Json.Deserialize(www.text) as IDictionary;
            if (CheckResponse(dict))
            {
                bool fail = false;
                if (dict["result"].Equals("fail") || dict["result"].Equals("error"))
                {
                    fail = true;
                }
                if (_callback != null) _callback(fail);
            }
        });
    }

    public static void GetConfig(Action<bool,string> _callback)
    {
        if (!CheckNetwork()) throw new GameException(GameException.ErrorCode.NoNetwork);
        string _url = string.Format("{0}?CODE={1}", Instance.URL_CONFIG, (int)FuncCode.GET_CONFIG);
        Instance.httpManager.CallRequest(_url, true, delegate (WWW www)
        {
            IDictionary dict = MiniJSON.Json.Deserialize(www.text) as IDictionary;
            if (CheckResponse(dict))
            {
                bool fail = false;
                if (dict["result"].Equals("fail") || dict["result"].Equals("error"))
                {
                    fail = true;
                }
                if (_callback != null) _callback(fail, www.text);
            }
        });
    }

    public static void GetServerState(Action<int> _callback)
    {
        if (!CheckNetwork()) throw new GameException(GameException.ErrorCode.NoNetwork);
        string _url = string.Format("{0}?CODE={1}", Instance.URL_SERVER_CHECK, (int)FuncCode.GET_SERVER_STATE);
        Instance.httpManager.CallRequest(_url, true, delegate (WWW www)
        {
            IDictionary dict = MiniJSON.Json.Deserialize(www.text) as IDictionary;
            if (CheckResponse(dict))
            {
                int state = -1;
                if (dict.Contains("state")) state = Convert.ToInt32(dict["state"]);
                if (_callback != null) _callback(state);
            }
        });
    }

    public static void SaveErrorLog(string _data)
    {
        if (!CheckNetwork()) throw new GameException(GameException.ErrorCode.NoNetwork);

        _data = WWW.EscapeURL(_data);
        string _url = string.Format("{0}?CODE={1}&LOG={2}", Instance.URL_ERROR_LOG, (int)FuncCode.SAVE_ERROR_LOG, _data);
        Instance.httpManager.CallRequest(_url, true, null);
    }

    public static void GetRankData(Action<bool, string> _callback)
    {
        if (!CheckNetwork()) throw new GameException(GameException.ErrorCode.NoNetwork);

        string _url = string.Format("{0}?CODE={1}", Instance.URL_RANK, (int)FuncCode.SELECT_RANK);
        Instance.httpManager.CallRequest(_url, true, delegate (WWW www)
        {
            IDictionary dict = MiniJSON.Json.Deserialize(www.text) as IDictionary;
            if (CheckResponse(dict))
            {
                string data = "";
                bool fail = false;

                if (dict["result"].Equals("fail") || dict["result"].Equals("error"))
                {
                    fail = true;
                }

                if (dict.Contains("data")) data = Convert.ToString(dict["data"]);

                if (_callback != null) _callback(fail, data);
            }
        });
    }

    public static void UpdateRankData(int _id, string _name, int _chaId, int _score, Action<bool, string> _callback)
    {
        if (!CheckNetwork()) throw new GameException(GameException.ErrorCode.NoNetwork);

        string _url = string.Format("{0}?CODE={1}&ID={2}&NAME={3}&CHA_ID={4}&SCORE={5}", Instance.URL_RANK, (int)FuncCode.UPDATE_RANK, _id, _name, _chaId, _score);
        Instance.httpManager.CallRequest(_url, true, delegate (WWW www)
        {
            Debug.Log(www.text);
            IDictionary dict = MiniJSON.Json.Deserialize(www.text) as IDictionary;
            if (CheckResponse(dict))
            {
                string data = "";
                bool fail = false;

                if (dict["result"].Equals("fail") || dict["result"].Equals("error"))
                {
                    fail = true;
                }

                if (dict.Contains("data")) data = Convert.ToString(dict["data"]);

                if (_callback != null) _callback(fail, data);
            }
        });
    }

    static bool CheckResponse(IDictionary _dict)
    {
        if (_dict == null)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.UnknownServerError));
            return false;
        }

        if (!_dict.Contains("result"))
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.UnknownServerError));
            return false;
        }

        return true;
    }

    static bool CheckNetwork()
    {
        switch(Application.internetReachability)
        {
            case NetworkReachability.NotReachable: return false;
            default: return true;
        }
    }
}
