using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MiniJSON;

namespace Kazoh.Table{

    public class TableManager {

        private Dictionary<string, TableData> dicGameData;
        private Dictionary<int, string> dicDataIndex;
        private Dictionary<string, StringData> dicString;
        private Dictionary<string, ConfigData> dicConfig;
        private List<TableData> listGameData;

        private static TableManager instance = null;

        private int ver;

        public static void Load(int ver)
        {
            if (instance != null)
            {      
    #if DEBUG_MODE
                Debug.LogWarning("[TableManager] 테이블 매니저가 이미 로딩 되었습니다.");  
    #endif
                return;
            }

            instance = new TableManager();

            if (!instance.Init(ver))
            {
                Debug.LogError("[TableManager] 테이블 매니저 초기화 실패!!!");
                throw new GameException(GameException.ErrorCode.CanNotInitTableManager);
            }

            /* 테이블 데이터 로딩 */
            foreach (TablePath.DataTable name in Enum.GetValues(typeof(TablePath.DataTable)))
            {
			    if(instance.LoadData(name)){
    #if DEBUG_MODE || UNITY_EDITOR
                        Debug.Log("[TableManager] 테이블 데이터 로딩 성공. 테이블: "+name.ToString());
    #endif
			    }
                else
                {
                    Debug.LogError("[TableManager] 테이블 데이터 로딩 실패. 오류 테이블: " + name.ToString());
                    throw new GameException(GameException.ErrorCode.CanNotLoadTable);
                }
            }

            /* 테이블 리스트 로딩 */
            foreach (TablePath.ListTable name in Enum.GetValues(typeof(TablePath.ListTable)))
            {
                if (instance.LoadData(name))
                {
#if DEBUG_MODE || UNITY_EDITOR
                    Debug.Log("[TableManager] 테이블 데이터 로딩 성공. 테이블: " + name.ToString());
#endif
                }
                else
                {
                    Debug.LogError("[TableManager] 테이블 데이터 로딩 실패. 오류 테이블: " + name.ToString());
                    throw new GameException(GameException.ErrorCode.CanNotLoadTable);
                }
            }

            /* 문자열 로딩 */
            foreach (TablePath.StringTable name in Enum.GetValues(typeof(TablePath.StringTable)))
            {
                if (instance.LoadString(name))
                {
    #if DEBUG_MODE || UNITY_EDITOR
                        Debug.Log("[TableManager] 테이블 데이터 로딩 성공. 테이블: " + name.ToString());
    #endif
                }
                else
                {  
                    Debug.LogError("[TableManager] 테이블 데이터 로딩 실패. 오류 테이블: " + name.ToString());
                    throw new GameException(GameException.ErrorCode.CanNotLoadTable);
                }
            }

            /* 설정 로딩 */
    //        foreach (TablePath.ConfigTable name in Enum.GetValues(typeof(TablePath.ConfigTable)))
    //        {
    //            if (instance.LoadConfig(name))
    //            {
    //#if DEBUG_MODE || UNITY_EDITOR
    //                    Debug.Log("[TableManager] 테이블 데이터 로딩 성공. 테이블: " + name.ToString());
    //#endif
    //            }
    //            else
    //            {
    //                Debug.LogError("[TableManager] 테이블 데이터 로딩 실패. 오류 테이블: " + name.ToString());
    //                throw new GameException(GameException.ErrorCode.CanNotLoadTable);
    //            }
    //        }

            /* 설정 테이블 테스트 */
            //ConfigData data = GetConfigBalance();
            //if (data.Name.Equals("ver_999"))
            //{
            //    Debug.LogError("[TableManager] 밸런스 데이터 없음. ver: " + ver);
            //}
            //data = GetConfigAi();
            //if (data.Name.Equals("ver_999"))
            //{
            //    Debug.LogError("[TableManager] AI 데이터 없음. ver: " + ver);
            //}
        }

        bool Init(int _ver)
        {
            if (instance == null)
            {
                instance = new TableManager();
            }

            ver = _ver;
    #if UNITY_EDITOR
            Debug.Log("[TableManager] 버전 세팅 : Config Version is " + ver);
    #endif

            instance.dicGameData = new Dictionary<string, TableData>();
            instance.dicDataIndex = new Dictionary<int, string>();
            instance.dicString = new Dictionary<string, StringData>();
            instance.dicConfig = new Dictionary<string, ConfigData>();
            instance.listGameData = new List<TableData>();

            return true;
        }

        bool LoadData(TablePath.DataTable key)
        {
            string path = TablePath.GetPath(key);
            TextAsset asset = Resources.Load(path) as TextAsset;
            if (!asset)
            {
                Debug.LogWarning("[TableManager] 데이터 파일이 없음 - path: "+path);
                return false;
            }

            string jsonString = asset.text;

            IDictionary dict1 = Json.Deserialize(jsonString) as IDictionary;
            if (dict1 == null)
            {
                Debug.LogWarning("[TableManager] 데이터 파일이 손상되었음 - key: " + key);
                Resources.UnloadAsset(asset);
                return false;
            }

            IList objectList = dict1[key.ToString()] as IList;    
		    if(objectList == null){
                Debug.LogWarning("[TableManager] 키가 잘못되었음 - key: " + key);
                Resources.UnloadAsset(asset);
			    return false;
		    }

            foreach (IDictionary dict2 in objectList)
            {
                try
                {
                    TableData data = CreateGameData(key, dict2);
                    if (dicDataIndex.ContainsKey(data.Id))
                    {
                        Debug.LogWarning(string.Format("[TableManager] Id가 중복된 데이터 발견 - 테이블:{0} ID:{1}", key, data.Id));
                    }
                    else
                    {
                        dicDataIndex.Add(data.Id, data.Name);
                    }
                    if (dicGameData.ContainsKey(data.Name))
                    {
                        Debug.LogWarning(string.Format("[TableManager] name이 중복된 데이터 발견 - 테이블:{0} ID:{1} name:{2}", key, data.Id, data.Name));
                    }
                    else
                    {
                        dicGameData.Add(data.Name, data);
                    }
                }
                catch(Exception e)
                {
                    Debug.LogWarning("[TableManager] 잘못된 데이터 발견 - 테이블: " + key);
                    Debug.LogException(e);
                }
            }

            Resources.UnloadAsset(asset);
            return true;
        }

        bool LoadData(TablePath.ListTable key)
        {
            string path = TablePath.GetPath(key);
            TextAsset asset = Resources.Load(path) as TextAsset;
            if (!asset)
            {
                Debug.LogWarning("[TableManager] 데이터 파일이 없음 - path: " + path);
                return false;
            }

            string jsonString = asset.text;

            IDictionary dict1 = Json.Deserialize(jsonString) as IDictionary;
            if (dict1 == null)
            {
                Debug.LogWarning("[TableManager] 데이터 파일이 손상되었음 - key: " + key);
                Resources.UnloadAsset(asset);
                return false;
            }

            IList objectList = dict1[key.ToString()] as IList;
            if (objectList == null)
            {
                Debug.LogWarning("[TableManager] 키가 잘못되었음 - key: " + key);
                Resources.UnloadAsset(asset);
                return false;
            }

            foreach (IDictionary dict2 in objectList)
            {
                try
                {
                    TableData data = CreateGameData(key, dict2);
                    listGameData.Add(data);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("[TableManager] 잘못된 데이터 발견 - 테이블: " + key);
                    Debug.LogException(e);
                }
            }

            Resources.UnloadAsset(asset);
            return true;
        }

        bool LoadString(TablePath.StringTable key)
        {
            string path = TablePath.GetPath(key);
            TextAsset asset = Resources.Load(path) as TextAsset;
            if (!asset)
            {
                Debug.LogWarning("[TableManager] 데이터 파일이 없음 - path: " + path);
                return false;
            }

            string jsonString = asset.text;

            IDictionary dict1 = Json.Deserialize(jsonString) as IDictionary;
            IList objectList = dict1[key.ToString()] as IList;

            if (objectList == null)
            {
                Debug.LogWarning("[TableManager] 키가 잘못되었음 - key: " + key);
                Resources.UnloadAsset(asset);
                return false;
            }

            foreach (IDictionary dict2 in objectList)
            {
                try
                {
                    StringData data = CreateStringData(dict2);
                    if (dicString.ContainsKey(data.Name))
                    {
                        Debug.LogWarning(string.Format("[TableManager] name이 중복된 문자열 발견 - 테이블:{0} name:{1}", key, data.Name));
                    }
                    else
                    {
                        dicString.Add(data.Name, data);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning("[TableManager] 잘못된 데이터 발견 - 테이블: " + key);
                    Debug.LogException(e);
                }
            }

            Resources.UnloadAsset(asset);

            return true;
        }

        bool LoadConfig(TablePath.ConfigTable key)
        {
            string path = TablePath.GetPath(key);
            TextAsset asset = Resources.Load(path) as TextAsset;
            if (!asset)
            {
                Debug.LogWarning("[TableManager] 데이터 파일이 없음 - path: " + path);
                return false;
            }

            string jsonString = asset.text;

            IDictionary dict1 = Json.Deserialize(jsonString) as IDictionary;
            IList objectList = dict1[key.ToString()] as IList;

            if (objectList == null)
            {
                Debug.LogWarning("[TableManager] 키가 잘못되었음 - key: " + key);
                Resources.UnloadAsset(asset);
                return false;
            }

            foreach (IDictionary dict2 in objectList)
            {
                try
                {
                    ConfigData data = CreateConfigData(key,dict2);
                    if (dicConfig.ContainsKey(data.Type.ToString() + "_" + data.Name))
                    {
                        Debug.LogWarning(string.Format("[TableManager] name이 중복된 문자열 발견 - 테이블:{0} name:{1}", key, data.Name));
                    }
                    else
                    {
                        dicConfig.Add(data.Type.ToString()+"_"+ data.Name, data);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning("[TableManager] 잘못된 데이터 발견 - 테이블: " + key);
                    Debug.LogException(e);
                }
            }

            Resources.UnloadAsset(asset);
            return true;
        }

        TableData CreateGameData(TablePath.DataTable key, IDictionary dict)
        {
            switch(key)
            {
                case TablePath.DataTable.Table_Data_Character:
                    return new TableData_Character(dict);
                case TablePath.DataTable.Table_Data_Npc:
                    return new TableData_Npc(dict);
                case TablePath.DataTable.Table_Data_Item:
			        return new TableData_Item(dict);
                case TablePath.DataTable.Table_Data_Option:
                    return new TableData_Option(dict);
                case TablePath.DataTable.Table_Data_Map:
                    return new TableData_Map(dict);
                case TablePath.DataTable.Table_Data_Drop:
                    return new TableData_Drop(dict);
                case TablePath.DataTable.Table_Data_Mission:
                    return new TableData_Mission(dict);
                default:
                    return new TableData(dict);
            }
        }

        TableData CreateGameData(TablePath.ListTable key, IDictionary dict)
        {
            switch (key)
            {
                case TablePath.ListTable.Table_List_Sale:
                    return new TableData_SaleList(dict);
                case TablePath.ListTable.Table_List_Map:
                    return new TableData_MapList(dict);
                case TablePath.ListTable.Table_List_Goods:
                    return new TableData_GoodsList(dict);
                case TablePath.ListTable.Table_List_DailyReward:
                    return new TableData_DailyReward(dict);
                case TablePath.ListTable.Table_List_Region:
                    return new TableData_Region(dict);
                default:
                    return new TableData(dict);
            }
        }

        StringData CreateStringData(IDictionary dict)
        {
            return new StringData(dict);
        }

        ConfigData CreateConfigData(TablePath.ConfigTable key, IDictionary dict)
        {
            switch (key)
            {
                default: 
                    return new ConfigData(dict);
            }
        }

        List<T> GetList<T>() where T : TableData
        {
            List<T> list = new List<T>();

            for (int i = 0; i < instance.listGameData.Count; ++i)
            {
                if (instance.listGameData[i] is T)
                {
                    list.Add(instance.listGameData[i] as T);
                }
            }

            return list;

        }

        public static TableData GetGameData(string name)
        {
            if (instance == null)
            {
                Debug.LogError("[TableManager] 테이블 매니저가 초기화되지 않았습니다.");
                throw new GameException(GameException.ErrorCode.CanNotInitTableManager);
            }

            TableData data = null;
            if (string.IsNullOrEmpty(name) || !instance.dicGameData.TryGetValue(name, out data))
            {
                Debug.LogWarning("[TableManager] 데이터 없음 - " + name);
                throw new GameException(GameException.ErrorCode.NoGameData);
            }

            return data;
        }

        public static TableData GetGameData(int id)
        {
            if (instance == null)
            {
                Debug.LogError("[TableManager] 테이블 매니저가 초기화되지 않았습니다.");
                throw new GameException(GameException.ErrorCode.CanNotInitTableManager);
            }

            string key = string.Empty;
            if (!instance.dicDataIndex.TryGetValue(id, out key))
            {
                Debug.LogError("[TableManager] 게임 데이터 없음 - " + id);
                throw new GameException(GameException.ErrorCode.NoGameData);
            }

            return GetGameData(key);
        }

        public static string GetString(string name)
        {
            if (instance == null)
            {
                Debug.LogError("[TableManager] 테이블 매니저가 초기화되지 않았습니다.");
                throw new GameException(GameException.ErrorCode.CanNotInitTableManager);
            }

            if(string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }

            StringData data = null;
            if (!instance.dicString.TryGetValue(name, out data))
            {
                Debug.LogError("[TableManager] 문자열 데이터 없음 - " + name);
                return name;
            }

    #if UNITY_EDITOR
            switch(GameProcess.Instance.Language)
            {
                case SystemLanguage.Korean: return data.Kor;
                case SystemLanguage.Japanese: return data.Jap;
                case SystemLanguage.English: return data.Eng;
                default: return data.Eng;
            }
#else
            switch(Application.systemLanguage)
            {
                case SystemLanguage.Korean: return data.Kor;
                case SystemLanguage.Japanese: return data.Jap;
                case SystemLanguage.English: return data.Eng;
                default: return data.Eng;
                
            }
#endif
        }

        public static int GetVersion()
        {
            return instance.ver;
        }

        public static string GetVersionName()
        {
            return "ver_" + instance.ver;
        }

        public static List<TableData_SaleList> GetSaleList()
        {
            if (instance == null)
            {
                Debug.LogError("[TableManager] 테이블 매니저가 초기화되지 않았습니다.");
                throw new GameException(GameException.ErrorCode.CanNotInitTableManager);
            }

            return instance.GetList<TableData_SaleList>();
        }

        public static List<TableData_MapList> GetMapList()
        {
            if (instance == null)
            {
                Debug.LogError("[TableManager] 테이블 매니저가 초기화되지 않았습니다.");
                throw new GameException(GameException.ErrorCode.CanNotInitTableManager);
            }

            return instance.GetList<TableData_MapList>();
        }

        public static List<TableData_GoodsList> GetGoodsList()
        {
            if (instance == null)
            {
                Debug.LogError("[TableManager] 테이블 매니저가 초기화되지 않았습니다.");
                throw new GameException(GameException.ErrorCode.CanNotInitTableManager);
            }

            return instance.GetList<TableData_GoodsList>();
        }

        public static List<TableData_Mission> GetAllMission()
        {
            List<TableData_Mission> list = new List<TableData_Mission>();
            foreach (KeyValuePair<string, TableData> data in instance.dicGameData)
            {
                TableData_Mission item = data.Value as TableData_Mission;
                if (item == null) continue;
                if (item.Name.Contains("test_") || item.Name.Contains("_test")) continue;
                list.Add(item);
            }

            return list;
        }

        public static List<TableData_DailyReward> GetDailyList()
        {
            if (instance == null)
            {
                Debug.LogError("[TableManager] 테이블 매니저가 초기화되지 않았습니다.");
                throw new GameException(GameException.ErrorCode.CanNotInitTableManager);
            }

            return instance.GetList<TableData_DailyReward>();
        }

        public static List<TableData_Region> GetRegionList()
        {
            if (instance == null)
            {
                Debug.LogError("[TableManager] 테이블 매니저가 초기화되지 않았습니다.");
                throw new GameException(GameException.ErrorCode.CanNotInitTableManager);
            }

            return instance.GetList<TableData_Region>();
        }
        
    }
}
