using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;
using MiniJSON;

public class GameDataManager
{
    public Action ChangedCurCharacterEvent;
    public Action ChangeUserMapListEvent;
    public Action AddedInventorySizeEvent;
    public Action<Data_User> ChangedChaSlotEvent;
    public Action NewMapEvent;
    public Action NewChaEvent;
    public Action NewItemEvent;

    public Data_User UserData { get; private set; }
    public bool HasNoCharacter
    {
        get
        {
            if (chaSlotList == null) return true;
            for(int i=0; i < chaSlotList.Count; ++i)
            {
                if (!chaSlotList[i].IsEmpty) return false;
            }
            return true;
        }
    }

    public Slot_Character CurCharacterSlot
    {
        get
        {
            return chaSlotList[curChaIdx];
        }
    }

    public int CurCharacterSlotId
    {
        get
        {
            return curChaIdx;
        }
    }

    private int curChaIdx;
    private List<Slot_Character> chaSlotList;
    private List<Slot_Item> itemSlotList;
    private List<Slot_Item> tempItemSlotList;
    private List<Data_UserMap> listUserMap;

    private List<int> listClearMission;
    private List<int> listIapHistory;
    private DateTime logInTime;
    private int guid;
    private string keyData;

    public void Init()
    {
        keyData = EncryptedPlayerPrefs.keys[2];
#if UNITY_EDITOR
        Debug.Log("게임 데이터 매니저 초기화!!!");
#endif
    }

    public void Load(int _guid = 0,string _timeStr = "", Action _callback = null)
    {
        try
        {
            guid = _guid;
            if (!_timeStr.Equals(string.Empty)) SetLogInTime(_timeStr);

            if(EncryptedPlayerPrefs.HasKey(keyData))
            {
                DBManager.UpdateData(guid, EncryptedPlayerPrefs.GetString(keyData), delegate (bool _isFail, string _time)
                {
                    if (_isFail) GameProcess.ShowError(new GameException(GameException.ErrorCode.FailToUpdateData));
                    else
                    {
                        EncryptedPlayerPrefs.DeleteKey(keyData);
                        DBManager.SelectData(guid, delegate (bool _isFail2, string _data, string _time2)
                        {
                            if (_isFail2) GameProcess.ShowError(new GameException(GameException.ErrorCode.FailToGetGameData));
                            else
                            {
                                SetGameData(_data, _time2);
                                if (_callback != null) _callback();
                            }
                        });
                    }
                });
            }
            else
            {
                DBManager.SelectData(guid, delegate (bool _isFail, string _data, string _time)
                {
                    if (_isFail) GameProcess.ShowError(new GameException(GameException.ErrorCode.FailToGetGameData));
                    else
                    {
                        SetGameData(_data, _time);

                        /* 최초 등록이면 저장한다. */
                        if (string.IsNullOrEmpty(_data)) Save();
                        if (_callback != null) _callback();
                    }
                });
            }
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    void SetGameData(string _data, string _timeStr)
    {
        IDictionary dict = null;
        if (!string.IsNullOrEmpty(_data)) dict = Json.Deserialize(_data) as IDictionary;

        /* 유저 데이터 세팅 */
        SetUserData(dict);

        /* 유저 아이템 데이터 세팅 */
        SetUserItemData(UserData.InventorySlotNum, dict);

        /* 유저 캐릭터 데이터 세팅 */
        SetUserChaData(UserData.ChaSlotNum, dict);

        /* 유저 맵 데이터 세팅 */
        SetUserMapData(dict);

        /* 유저 미션 데이터 세팅 */
        SetMissionData();

        /* 구매제한 상품 구매 내역 세팅 */
        SetIapHistory(dict);

#if UNITY_EDITOR
        Debug.Log("게임 데이터 로딩 완료!!!");
#endif
    }

    public void Save()
    {
        try
        {
            if (UserData == null)
            {
#if UNITY_EDITOR
                Debug.LogError("[GameDataManager] 유저 데이터가 생성되지 않아 데이터를 저장할 수 없습니다.");
#endif
                return;
            }

            Dictionary<string, object> hash = new Dictionary<string, object>();
            hash.Add("USER_DATA", UserData.GetHash());

            Dictionary<string, object> dicCha = new Dictionary<string, object>();
            dicCha.Add("CUR_CHA", curChaIdx);
            for (int i = 0; i < chaSlotList.Count; ++i)
            {
                if (chaSlotList[i].IsEmpty) continue;
                dicCha.Add("SLOT_" + i, chaSlotList[i].Data.GetHash());
            }
            hash.Add("CHARACTER", dicCha);

            Dictionary<string, object> dicItem = new Dictionary<string, object>();
            for (int i = 0; i < itemSlotList.Count; ++i)
            {
                if (itemSlotList[i].IsEmpty) continue;
                dicItem.Add("SLOT_" + i, itemSlotList[i].Data.GetHash());
            }
            hash.Add("INVENTORY", dicItem);

            ArrayList listMap = new ArrayList();
            foreach (Data_UserMap map in listUserMap)
            {
                listMap.Add(map.GetHash());
            }
            hash.Add("MAP_LIST", listMap);

            //ArrayList iapHistory = listIapHistory.ToArray( new ArrayList();
            //foreach (int productId in listIapHistory)
            //{
            //    listMap.Add(productId);
            //}
            hash.Add("IAP_HISTORY", listIapHistory.ToArray());

            string data = Json.Serialize(hash);
            EncryptedPlayerPrefs.SetString(keyData, data);

            DBManager.UpdateData(guid, data, delegate (bool _isFail, string _timeStr)
            {
                if (_isFail) Debug.LogError("Error: Fail to update game data.");
                else EncryptedPlayerPrefs.DeleteKey(keyData);
#if UNITY_EDITOR
                Debug.Log("SAVE: " + data);
#endif
            });
        }
        catch (GameException e)
        {
            if(e.Code != GameException.ErrorCode.FailToUpdateData) GameProcess.ShowError(e);
        }
        catch (Exception e)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
        }
    }

    #region // Test
    public void TestSetUserData(int _gold, int _coin, int _key, int _cash, int _slot, int _inven)
    {
        UserData = new Data_User(_gold, _coin, _key, _cash, _slot, _inven);
        Debug.Log("게임 데이터 매니저 초기화!!!");
    }

    public void TestMapClear(int _id, int _record)
    {
        listUserMap.Add(new Data_UserMap(_id, _record, false));
    }
    #endregion // Test

    #region // Set
    void SetUserData(IDictionary dict)
    {
        /* 유저 데이터 세팅 */
        if (dict == null || !dict.Contains("USER_DATA"))
        {
            UserData = new Data_User();
        }
        else
        {
            IDictionary dict2 = dict["USER_DATA"] as IDictionary;
            if (dict2 == null) UserData = new Data_User();
            else UserData = new Data_User(dict2);
        }

#if UNITY_EDITOR
        Debug.Log("[GDM] 유저 데이터 세팅 완료!!");
#endif
    }

    void SetUserChaData(int _slotNum, IDictionary dict)
    {
        chaSlotList = new List<Slot_Character>();
        for (int i = 0; i < _slotNum; ++i)
        {
            chaSlotList.Add(new Slot_Character());
            chaSlotList[i].ChangedEvent += OnChangeChaSlot;
        }

        curChaIdx = 0;
        
        if (dict != null && dict.Contains("CHARACTER"))
        {
            IDictionary dict2 = dict["CHARACTER"] as IDictionary;
            if (dict2 != null)
            {
                for(int i=0; i < chaSlotList.Count; ++i)
                {
                    if (dict2.Contains("SLOT_"+i))
                    {
                        IDictionary dict3 = dict2["SLOT_" + i] as IDictionary;
                        if(dict3 != null) chaSlotList[i].Put(dict3);
                    }
                }

                if (dict2.Contains("CUR_CHA")) curChaIdx = Convert.ToInt32(dict2["CUR_CHA"]);
                if (CurCharacterSlot.IsEmpty) curChaIdx = 0;
            }
        }

#if UNITY_EDITOR
        Debug.Log("[GDM] 유저 캐릭터 데이터 세팅 완료!!");
#endif
    }

    void SetUserItemData(int _slotNum, IDictionary dict)
    {
        tempItemSlotList = new List<Slot_Item>();
        itemSlotList = new List<Slot_Item>();
        for (int i = 0; i < _slotNum; ++i)
        {
            itemSlotList.Add(new Slot_Item(i));
        }
                
        if (dict != null && dict.Contains("INVENTORY"))
        {
            IDictionary dict2 = dict["INVENTORY"] as IDictionary;
            if (dict2 != null)
            {
                for (int i = 0; i < itemSlotList.Count; ++i)
                {
                    if (dict2.Contains("SLOT_" + i))
                    {
                        IDictionary dict3 = dict2["SLOT_" + i] as IDictionary;
                        if (dict3 != null) itemSlotList[i].Put(dict3);
                    }
                }
            }
        }

#if UNITY_EDITOR
        Debug.Log("[GDM] 유저 아이템 데이터 세팅 완료!!");
#endif
    }

    void SetUserMapData(IDictionary dict)
    {
        listUserMap = new List<Data_UserMap>();
        if (dict != null && dict.Contains("MAP_LIST"))
        {
            IList list = dict["MAP_LIST"] as IList;
            foreach (IDictionary dict2 in list)
            {
                Data_UserMap map = new Data_UserMap(dict2);
                listUserMap.Add(map);
            }
        }

#if UNITY_EDITOR
        Debug.Log("[GDM] 유저 맵 데이터 세팅 완료!!");
#endif
    }

    void SetMissionData()
    {
        listClearMission = new List<int>();
        if(listUserMap != null)
        {
            List<TableData_Mission> ml = TableManager.GetAllMission();
            foreach(TableData_Mission mission in ml)
            {
                Data_UserMap map = listUserMap.Find(x => x.DataId == mission.MapId);
                if(map != null && map.Record > 0) listClearMission.Add(mission.Id);
            }
        }

#if UNITY_EDITOR
        Debug.Log("[GDM] 유저 미션 데이터 세팅 완료!!");
#endif
    }

    void SetIapHistory(IDictionary dict)
    {
        listIapHistory = new List<int>();
        if (dict != null && dict.Contains("IAP_HISTORY"))
        {
            IList list = dict["IAP_HISTORY"] as IList;
            foreach (int productId in list)
            {
                listIapHistory.Add(productId);
            }
        }

#if UNITY_EDITOR
        Debug.Log("[GDM] 구매제한 상품 구매 내역 세팅 완료!!");
#endif
    }
    #endregion // Set


    #region // Character

    public List<Slot_Character> GetCharacterSlot()
    {
        List<Slot_Character> list = new List<Slot_Character>();
        list.AddRange(chaSlotList);
        return list;
    }

    public Slot_Character GetCurCharacter()
    {
        if (chaSlotList != null) return CurCharacterSlot;
        return new Slot_Character();
    }

    public int GetCharacterNum()
    {
        if (chaSlotList != null) return chaSlotList.FindAll(x => x.IsEmpty == false).Count;
        return 0;
    }

    public bool HasNewCharacter()
    {
        Slot_Character slot = chaSlotList.Find(x => x.IsNew);
        return slot == null ? false : true;
    }

    public void ConformCharacter()
    {
        for(int i=0; i < chaSlotList.Count; ++i)
        {
            chaSlotList[i].SetNew();
        }
    }

    public void SelectCharacter(Slot_Character _slot)
    {
        if (_slot == null)
        {
            throw new GameException(GameException.ErrorCode.InvalidParam);
        }
        else if (_slot.IsEmpty)
        {
            throw new GameException(GameException.ErrorCode.CanNotSelectCharacter);
        }
        else
        {
            try
            {
                int idx = chaSlotList.IndexOf(_slot);
                if (idx < 0) throw new GameException(GameException.ErrorCode.InvalidParam);
                else
                {
                    curChaIdx = idx;
                    OnChangeCurCharacter();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

    public void AddCharacter(int _id, Slot_Character _slot = null)
    {
        if (_slot == null || !_slot.IsEmpty)
        {
            _slot = chaSlotList.Find(x => x.IsEmpty);
        }

        if (_slot == null)
        {
            throw new GameException(GameException.ErrorCode.ChaSlotIsFull);
        }
        else
        {
            TableData_Character data = TableManager.GetGameData(_id) as TableData_Character;
            _slot.Put(data);
            _slot.SetNew();
        }
    }

    public TableData_Character[] SummonCharater(TableData_GoodsList _data, Slot_Character _slot = null)
    {
        if (UserData.Coin < _data.Price) throw new GameException(GameException.ErrorCode.NotEnoughCoin);
        if (_data.Num < 1) throw new GameException(GameException.ErrorCode.InvalidParam);

        List<Slot_Character> slotList = new List<Slot_Character>();
        if (_slot != null && _slot.IsEmpty) slotList.Add(_slot);
        for(int i=0; i < chaSlotList.Count; ++i)
        {
            if (slotList.Count == _data.Num) break;
            if (slotList.Contains(chaSlotList[i])) continue;
            if (chaSlotList[i].IsEmpty) slotList.Add(chaSlotList[i]);
        }

        if (slotList.Count < _data.Num)
        {
            throw new GameException(GameException.ErrorCode.ChaSlotIsFull);
        }
        else
        {
            try
            {
                TableData_Character[] data = new TableData_Character[slotList.Count];
                for(int i=0; i<data.Length; ++i)
                {
                    data[i] = Summon(_data);
                    slotList[i].Put(data[i]);
                }
                UserData.AddCoin((-1) * _data.Price);
                OnNewCharacter();
                return data;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

    TableData_Character Summon(TableData_GoodsList _data)
    {
        List<TableData_Character> list = new List<TableData_Character>();
        for (int i = 0; i < _data.SummonList.Count; ++i)
        {
            TableData_Character cha = TableManager.GetGameData(_data.SummonList[i]) as TableData_Character;
            if (cha != null) list.Add(cha);
        }

        if (list.Count == 0) throw new GameException(GameException.ErrorCode.InvalidParam);

        int dice = UnityEngine.Random.Range(0, 10000);
        dice %= list.Count;
        return list[dice];
    }

    public void DeleteCharacter(Slot_Character _slot)
    {
        try
        {
            if(_slot == CurCharacterSlot) throw new GameException(GameException.ErrorCode.CanNotDeleteCharacter);
            _slot.Discard();
            OnChangeChaSlot();
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public string EnchantCharacter(Slot_Character _slot)
    {
        try
        {
            int cost = GameProcess.GetGameConfig().CostEnchantCha;
            if (UserData.Gold < cost) throw new GameException(GameException.ErrorCode.NotEnoughGold);
            string str = _slot.Enchant();
            UserData.AddGold(-cost);
            OnChangeChaSlot();
            OnChangeCurCharacter();
            return str;
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public string DownGradeCharacter(Slot_Character _slot)
    {
        try
        {
            int cost = GameProcess.GetGameConfig().CostDownGradeCha;
            if (UserData.Coin < cost) throw new GameException(GameException.ErrorCode.NotEnoughCoin);
            string str = _slot.DownGrade();
            UserData.AddCoin(-cost);
            OnChangeChaSlot();
            OnChangeCurCharacter();
            return str;
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public void AddHp(Slot_Character _cha, int value)
    {
        try
        {
            _cha.AddHp(value);
            OnChangeChaSlot();
            OnChangeCurCharacter();
        }
        catch (Exception e)
        {
            throw e;
        }
    }
    #endregion // Character

    #region // Item
    public List<Slot_Item> GetInventory(bool _isSort = false)
    {
        List<Slot_Item> list = new List<Slot_Item>();
        list.AddRange(itemSlotList);
        if (_isSort) list.Sort(delegate (Slot_Item x, Slot_Item y)
         {
             if (x.IsEmpty && y.IsEmpty) return x.SlotId.CompareTo(y.SlotId);
             else if (x.IsEmpty) return 1;
             else if (y.IsEmpty) return -1;

             if (x.IsEquipped && y.IsEquipped) return 0;
             else if (x.IsEquipped) return -1;
             else if (y.IsEquipped) return 1;

             return y.ItemId.CompareTo(x.ItemId);
         });

        return list;
    }

    public Slot_Item GetInventory(int _idx)
    {
        if (itemSlotList != null && _idx < itemSlotList.Count) return itemSlotList[_idx];
        return null;
    }

    public bool HasNewItem()
    {
        Slot_Item slot = itemSlotList.Find(x => x.IsNew);
        return slot == null ? false : true;
    }

    public void CheckNewItem()
    {
        OnNewItem();
    }

    public int GetEmptyItemSlotNum()
    {
        return itemSlotList.FindAll(x => x.IsEmpty).Count;
    }

    public void BuyItem(TableData_SaleList _data)
    {
        try
        {
            TableData_Item _item = TableManager.GetGameData(_data.ItemName) as TableData_Item;
            if (_item == null)
            {
                throw new GameException(GameException.ErrorCode.NoGameData);
            }

            int num = _data.Amount;

            /* 슬롯 여분 확인 */
            if(itemSlotList.Find(x=>x.IsEmpty) == null)
            {
                if (!_item.IsStack) throw new GameException(GameException.ErrorCode.NotEnoughItemSlot);
                List<Slot_Item> list = itemSlotList.FindAll(x=> x.ItemId == _item.Id && x.RestCount > 0);
                if (list == null) throw new GameException(GameException.ErrorCode.NotEnoughItemSlot);
                int sum = 0;
                for(int i=0; i<list.Count; ++i)
                {
                    sum += list[i].RestCount;
                }
                if (sum < num) throw new GameException(GameException.ErrorCode.NotEnoughItemSlot);
            }

            /* 비용 확인 */
            if (_data.Type == TableEnum.MoneyType.Coin)
            {
                int price = num * _item.CoinPrice;
                if (UserData.Coin < price) throw new GameException(GameException.ErrorCode.NotEnoughCoin);

                AddItem(_item, num);
                UserData.AddCoin(-price);
            }
            else
            {
                int price = num * _item.Price;
                if (UserData.Gold < price) throw new GameException(GameException.ErrorCode.NotEnoughGold);

                AddItem(_item, num);
                UserData.AddGold(-price);
            }
            OnNewItem();
        }
        catch (Exception e)
        {
            throw e;
        }

    }

    /// <summary>
    /// 주의!! 여러번 호출될 수 있으므로 해당 함수 안에서는 Save()함수를 호출하지 않는다.
    /// </summary>
    /// <param name="_data"></param>
    /// <param name="num"></param>
    public void AddItem(TableData_Item _data, int num)
    {
        if (_data.IsStack)
        {
            List<Slot_Item> list = itemSlotList.FindAll(x => x.ItemId == _data.Id && x.RestCount > 0);
            if (list != null)
            {
                for(int i=0; i<list.Count; ++i)
                {
                    int n = Mathf.Min(num, list[i].RestCount);
                    list[i].Put(n);
                    num -= n;
                }
            }
        }
        if (num > 0)
        {
            Slot_Item slot = itemSlotList.Find(x => x.IsEmpty);
            if (slot != null)
            {
                slot.Put(_data, num);
            }
            else
            {
                AddItemToTempList(_data, num);
            }
        }
    }

    public void AddItem(Data_Reward.Reward _data)
    {
        TableData_Item _item = TableManager.GetGameData(_data.ItemId) as TableData_Item;
        if (_item != null) AddItem(_item, _data.Num);
        else throw new GameException(GameException.ErrorCode.NoGameData);
    }

    void AddItemToTempList(TableData_Item _data, int num)
    {
        if (_data.IsStack)
        {
            List<Slot_Item> list = tempItemSlotList.FindAll(x => x.ItemId == _data.Id && x.RestCount > 0);
            if (list != null)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    int n = Mathf.Min(num, list[i].RestCount);
                    list[i].Put(n);
                    num -= n;
                }
            }
        }
        if (num > 0)
        {
            Slot_Item slot = new Slot_Item(-1);
            slot.Put(_data, num);
            tempItemSlotList.Add(slot);
        }
    }

    public void SellItem(Slot_Item _slot, int num)
    {
        try
        {
            int gold = _slot.Data.Data.SalePrice * num;
            int max = GameProcess.GetGameConfig().MaxGold;
            if (UserData.Gold + gold > max) throw new GameException(GameException.ErrorCode.OverMaxGold);
            _slot.SellItem(num);
            UserData.AddGold(gold);
            MoveItemFromTempList();
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public void EquipItem(Slot_Item _item, Slot_Character _cha)
    {
        try
        {
            _cha.Equip(_item);
            OnChangeChaSlot();
            OnChangeCurCharacter();
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public void DismountItem(Slot_Item _item)
    {
        try
        {
            List<Slot_Character> list = chaSlotList;
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].IsEmpty) continue;
                if (!list[i].Data.EquipSlot.ContainsKey(_item.EquipSlotId)) continue;
                if (list[i].Data.EquipSlot[_item.EquipSlotId] == null) continue;
                if (list[i].Data.EquipSlot[_item.EquipSlotId] == _item)
                {
                    list[i].Dismount(_item.EquipSlotId);
                    _item.Equip(false);
                }
            }
            OnChangeChaSlot();
            OnChangeCurCharacter();
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public void EnchantItem(Slot_Item _slot)
    {
        try
        {
            int cost = GameProcess.GetGameConfig().CostEnchantItem;
            if (UserData.Gold < cost) throw new GameException(GameException.ErrorCode.NotEnoughGold);

            _slot.Enchant();
            UserData.AddGold(-cost);
            if (itemSlotList.Find(x => x.IsEmpty) != null) MoveItemFromTempList();
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    void MoveItemFromTempList()
    {
        Stack<Slot_Item> stack = new Stack<Slot_Item>(tempItemSlotList);
        while(stack.Count > 0)
        {
            Slot_Item slot = stack.Pop();
            Data_UserItem item = slot.Data;
            int num = slot.Data.Num;

            if (item.Data.IsStack)
            {
                List<Slot_Item> list = itemSlotList.FindAll(x => x.ItemId == slot.ItemId && x.RestCount > 0);
                if (list != null)
                {
                    for (int i = 0; i < list.Count; ++i)
                    {
                        int n = Mathf.Min(num, list[i].RestCount);
                        list[i].Put(n);
                        num -= n;
                    }
                }
            }
            if (num > 0)
            {
                Slot_Item itemSlot = itemSlotList.Find(x => x.IsEmpty);
                if (itemSlot != null)
                {
                    itemSlot.Put(item.Data, num);
                    tempItemSlotList.Remove(slot);
                }
                else
                {
                    slot.UseItem(slot.Data.Num - num);
                }
            }
        }
    }

    public void UseItem(Slot_Item _slot, Slot_Character target)
    {
        try
        {
            string title = TableManager.GetString("STR_UI_USE");
            string msg = "";
            string text1 = "";
            switch (_slot.Data.Data.EffectType)
            {
                case TableEnum.EffectType.ADD_HP:
                    if (target.Data.CurHp == target.Data.MaxHp)
                    {
                        GameProcess.ShowError(new GameException(GameException.ErrorCode.CanNotHeal));
                        return;
                    }
                    int value = _slot.Data.Data.EffectValue;
                    GameProcess.GetGameDataManager().AddHp(target, value);
                    GameProcess.PlaySound(SOUND_EFFECT.HEAL);
                    break;

                case TableEnum.EffectType.ADD_CHARACTER_SLOT:
                    GameProcess.GetGameDataManager().AddCharacterSlot(true);

                    msg = TableManager.GetString("STR_MSG_USE_3");
                    text1 = TableManager.GetString("STR_UI_OK");
                    GameProcess.ShowPopup(NoticeType.OK, title, msg, text1, null);
                    break;

                case TableEnum.EffectType.ADD_ITEM_SLOT:
                    GameProcess.GetGameDataManager().AddItemSlot(true);

                    msg = TableManager.GetString("STR_MSG_USE_4");
                    text1 = TableManager.GetString("STR_UI_OK");
                    GameProcess.ShowPopup(NoticeType.OK, title, msg, text1, null);
                    break;

                case TableEnum.EffectType.ADD_ITEM:
                    List<TableData_Drop> list = new List<TableData_Drop>();
                    for (int i = 0; i < _slot.Data.Data.DropList.Count; ++i)
                    {
                        TableData_Drop drop = TableManager.GetGameData(_slot.Data.Data.DropList[i]) as TableData_Drop;
                        if (drop == null) break;
                        list.Add(drop);
                    }
                    Data_Reward reward = Data_Reward.DoDrop(list);
                    GameProcess.GetGameDataManager().ExcuteReward(reward);
                    break;

                case TableEnum.EffectType.SUMMON:
                    AddCharacter(_slot.Data.Data.EffectValue);
                    break;
            }

            _slot.UseItem();
            MoveItemFromTempList();
        }
        catch(Exception e)
        {
            throw e;
        }
    }

    public void ExcuteReward(Data_Reward _data)
    {
        int maxGold = GameProcess.GetGameConfig().MaxGold;
        int maxCoin = GameProcess.GetGameConfig().MaxCoin;
        int gold = _data.Gold;
        int coin = _data.Coin;
        if (UserData.Gold + gold > maxGold) gold = maxGold - UserData.Gold;
        if (UserData.Coin + coin > maxCoin) coin = maxCoin - UserData.Coin;
        UserData.AddGold(gold);
        UserData.AddCoin(coin);
        for (int i = 0; i < _data.ItemRewardList.Count; ++i)
        {
            TableData_Item _item = TableManager.GetGameData(_data.ItemRewardList[i].ItemId) as TableData_Item;
            if (_item != null) AddItem(_item, _data.ItemRewardList[i].Num);
        }
        OnNewItem();
    }
    #endregion // Item

    #region // Map
    public bool IsLockMap(TableData_MapList _data)
    {
        TableData_Map map = TableManager.GetGameData(_data.MapName) as TableData_Map;
        if (map == null) return true;
        if (listUserMap.Find(x => x.DataId == map.Id) != null) return false;

        bool isLock = false;
        for (int i = 0; i < _data.UnlockCondList.Count; ++i)
        {
            if (isLock) break;
            if (_data.UnlockCondList[i].Cond == TableEnum.UnlockType.None) break;
            switch (_data.UnlockCondList[i].Cond)
            {
                case TableEnum.UnlockType.BCrownNum:
                    if (listUserMap.FindAll(x => x.Record > 0).Count < _data.UnlockCondList[i].Value) isLock = true;
                    break;
                case TableEnum.UnlockType.SCrownNum:
                    if (listUserMap.FindAll(x => x.Record > 1).Count < _data.UnlockCondList[i].Value) isLock = true;
                    break;
                case TableEnum.UnlockType.GCrownNum:
                    if (listUserMap.FindAll(x => x.Record > 2).Count < _data.UnlockCondList[i].Value) isLock = true;
                    break;
                case TableEnum.UnlockType.BCrownMapId:
                    if (listUserMap.Find(x => x.DataId == _data.UnlockCondList[i].Value && x.Record > 0) == null) isLock = true;
                    break;
                case TableEnum.UnlockType.SCrownMapId:
                    if (listUserMap.Find(x => x.DataId == _data.UnlockCondList[i].Value && x.Record > 1) == null) isLock = true;
                    break;
                case TableEnum.UnlockType.GCrownMapId:
                    if (listUserMap.Find(x => x.DataId == _data.UnlockCondList[i].Value && x.Record > 2) == null) isLock = true;
                    break;
            }

        }

        if (!isLock) SaveMapRecord(map.Id, 0);
        return isLock;
    }

    public int GetMapRecord(int _id)
    {
        Data_UserMap map = listUserMap.Find(x => x.DataId == _id);
        return map == null ? 0 : map.Record;
    }

    public bool IsNewMap(int _id)
    {
        Data_UserMap map = listUserMap.Find(x => x.DataId == _id);
        return map == null ? false : map.IsNew;
    }

    public bool HasNewMap()
    {
        Data_UserMap map = listUserMap.Find(x => x.IsNew);
        return map == null ? false : true;
    }

    public void PlayMap(int _id)
    {
        Data_UserMap map = listUserMap.Find(x => x.DataId == _id);
        if (map != null)
        {
            map.SetNew();
            OnNewMap();
        }
    }

    public int GetMapNum(int _record)
    {
        List<Data_UserMap> list = listUserMap.FindAll(x => x.Record >= _record);
        return list == null ? 0 : list.Count;
    }

    public void SaveMapRecord(int _id, int _record)
    {
        Data_UserMap map = listUserMap.Find(x => x.DataId == _id);
        if (map == null)
        {
            listUserMap.Add(new Data_UserMap(_id, _record, true));
        }
        else if (_record > map.Record)
        {
            listUserMap.Remove(map);
            listUserMap.Add(new Data_UserMap(_id, _record, false));
        }

        GameProcess.GetMissionManager().CompleteMission(_id, (TableEnum.MapClearGrade)_record);

        OnChangeUserMapList();
        OnNewMap();
    }
    #endregion // Map

    #region // UserData
    public void AddCharacterSlot(bool _useItem)
    {
        try
        {
            int max = GameProcess.GetGameConfig().MaxChaSlotNum;
            int cost = _useItem ? 0 : GameProcess.GetGameConfig().CostAddChaSlot;

            if (UserData.ChaSlotNum > GameProcess.GetGameConfig().MaxChaSlotNum) throw new GameException(GameException.ErrorCode.OverMaxCharacter);
            else if (UserData.Coin < cost) throw new GameException(GameException.ErrorCode.NotEnoughCoin);

            UserData.AddCoin(-cost);
            Slot_Character slot = new Slot_Character();
            slot.ChangedEvent += OnChangeChaSlot;
            chaSlotList.Add(slot);
            UserData.AddChaSlot(1);
            OnChangeChaSlot();
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public void AddItemSlot(bool _useItem)
    {
        try
        {
            int max = GameProcess.GetGameConfig().MaxItemSlotNum - GameProcess.GetGameConfig().AddedItemSlotNum;
            int num = GameProcess.GetGameConfig().AddedItemSlotNum;
            int cost = _useItem ? 0 : GameProcess.GetGameConfig().CostAddChaSlot;

            if (UserData.InventorySlotNum > max) throw new GameException(GameException.ErrorCode.OverMaxInventory);
            else if (UserData.Coin < cost) throw new GameException(GameException.ErrorCode.NotEnoughCoin);

            UserData.AddCoin(-cost);

            int count = itemSlotList.Count;
            for(int i=0; i < num; ++i)
            {
                Slot_Item slot = new Slot_Item(count + i);
                itemSlotList.Add(slot);
            }

            UserData.AddItemSlot(num);
            MoveItemFromTempList();

            OnChangeItemSlot();
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public void ExchangeGold(TableData_GoodsList _data, Action _callback)
    {
        if (_data == null) throw new GameException(GameException.ErrorCode.InvalidParam);
        if (UserData.Cash < _data.Price) throw new GameException(GameException.ErrorCode.NotEnoughCash);
        int max = GameProcess.GetGameConfig().MaxGold;
        if (UserData.Gold + _data.Num > max) throw new GameException(GameException.ErrorCode.OverMaxGold);

        try
        {
            DBManager.CheckLogServer(delegate (bool _isFail, string _tick)
            {
                if (_isFail) GameProcess.ShowError(new GameException(GameException.ErrorCode.CanNotConnectServer));
                else
                {
                    UserData.AddCash((-1) * _data.Price);
                    UserData.AddGold(_data.Num);
                    if (_callback != null) _callback();

                    /* 로그 처리 */
                    Dictionary<string, object> hash = new Dictionary<string, object>();
                    hash.Add("TYPE", "use");
                    hash.Add("GUID", guid);
                    hash.Add("ID", _data.Id);
                    hash.Add("NAME", _data.Name);
                    hash.Add("PRICE", _data.Price);
                    string log = Json.Serialize(hash);
                    DBManager.SaveLog(log, delegate (bool _isFail2)
                    {
                        if (_isFail2) Debug.LogError("Error: 구매 로그 저장 실패");
                        else
                        {
#if UNITY_EDITOR
                            Debug.Log("IAP: " + log);
#endif
                        }
                    });
                }
            });
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public void ExchangeCoin(TableData_GoodsList _data, Action _callback)
    {
        if (_data == null) throw new GameException(GameException.ErrorCode.InvalidParam);
        if (UserData.Cash < _data.Price) throw new GameException(GameException.ErrorCode.NotEnoughCash);
        int max = GameProcess.GetGameConfig().MaxCoin;
        if (UserData.Coin + _data.Num > max) throw new GameException(GameException.ErrorCode.OverMaxCoin);

        try
        {
            DBManager.CheckLogServer(delegate (bool _isFail, string _tick)
            {
                if (_isFail) GameProcess.ShowError(new GameException(GameException.ErrorCode.CanNotConnectServer));
                else
                {
                    UserData.AddCash((-1) * _data.Price);
                    UserData.AddCoin(_data.Num);
                    if (_callback != null) _callback();

                    /* 로그 처리 */
                    Dictionary<string, object> hash = new Dictionary<string, object>();
                    hash.Add("TYPE", "use");
                    hash.Add("GUID", guid);
                    hash.Add("ID", _data.Id);
                    hash.Add("NAME", _data.Name);
                    hash.Add("PRICE", _data.Price);
                    string log = Json.Serialize(hash);
                    DBManager.SaveLog(log, delegate (bool _isFail2)
                    {
                        if (_isFail2) Debug.LogError("Error: 구매 로그 저장 실패");
                        else
                        {
#if UNITY_EDITOR
                            Debug.Log("IAP: " + log);
#endif
                        }
                    });
                }
            });
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public void Recharge(TableData_GoodsList _data)
    {
        if (_data == null) throw new GameException(GameException.ErrorCode.InvalidParam);
        if (UserData.Coin < _data.Price) throw new GameException(GameException.ErrorCode.NotEnoughCoin);
        if (UserData.Key + _data.Num > 10) throw new GameException(GameException.ErrorCode.OverMaxKey);
        UserData.AddCoin((-1) * _data.Price);
        UserData.AddKey(_data.Num);
    }

    public void BuyCash(TableData_GoodsList _data, Action _callback)
    {
        if (_data == null) throw new GameException(GameException.ErrorCode.InvalidParam);
        int max = GameProcess.GetGameConfig().MaxCash;
        if (UserData.Cash + _data.Num > max) throw new GameException(GameException.ErrorCode.OverMaxCash);

        /* 구매 로직이 들어가야 한다. */
        try
        {
            DBManager.CheckLogServer(delegate (bool _isFail, string _tick)
            {
                if (_isFail) GameProcess.ShowError(new GameException(GameException.ErrorCode.CanNotConnectServer));
                else
                {
                    GameProcess.BuyInappItem(UnityEngine.Purchasing.ProductType.Consumable, _data.ProductId, delegate ()
                    {
                        /* 구매 처리 */
                        UserData.AddCash(_data.Num);
                        UserData.AddPurcase(_data.Price_WON);
                        if (_data.PurchaseLimit > 0) listIapHistory.Add(_data.ProductId);
                        if (_callback != null) _callback();

                        /* 로그 처리 */
                        Dictionary<string, object> hash = new Dictionary<string, object>();
                        hash.Add("TYPE", "buy");
                        hash.Add("GUID", guid);
                        hash.Add("ID", _data.Id);
                        hash.Add("NAME", _data.Name);
                        hash.Add("PRICE", _data.Price_WON);
                        hash.Add("PRODUCT", _data.ProductId);
                        string log = Json.Serialize(hash);
                        DBManager.SaveLog(log, delegate (bool _isFail2)
                        {
                            if (_isFail2) Debug.LogError("Error: 구매 로그 저장 실패");
                            else
                            {
#if UNITY_EDITOR
                                Debug.Log("IAP: " + log);
#endif
                            }
                        });
                    });
                }
            });
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public void UseKey(int num)
    {
        if (UserData.Key < num) throw new GameException(GameException.ErrorCode.NotEnoughKey);
        UserData.AddKey(-num);
    }

    public int GetMembership()
    {
        int grade = 0;
        if (UserData != null)
        {
            int amount = UserData.PurchaseAmount;
            int[] array = GameProcess.GetGameConfig().VipGradeArray;
            for (int i = 0; i < array.Length; ++i)
            {
                if (amount < array[i])
                {
                    grade = i;
                    break;
                }
            }
        }

        return grade;
    }

    public string GetUserInitial()
    {
        return UserData != null ? UserData.Initial : "AAA";
    }

    public void SetUserInitial(string _initial)
    {
        if (UserData == null) throw new GameException(GameException.ErrorCode.UserDataIsNull);
        UserData.EditInitial(_initial);
    }

    public int GetGUID()
    {
        return guid;
    }

    public int GetPurchaseCount(int _productId)
    {
        int count = 0;
        for(int i=0; i<listIapHistory.Count; ++i)
        {
            if (listIapHistory[i] == _productId) count++;
        }
        return count;
    }
    #endregion // UserData

    #region // Mission
    public List<int> GetCompletedMissionList()
    {
        return new List<int>(listClearMission);
    }

    public void SetClearMission(int _id)
    {
        if (!listClearMission.Contains(_id)) listClearMission.Add(_id); 
    }

    public TableData_DailyReward GetDailyReward()
    {
        if(UserData.HasDailyReward())
        {
            List<TableData_DailyReward> list = TableManager.GetDailyList();
            TableData_DailyReward reward = list.Find(x => x.Id == UserData.DailyCount);
            return reward;
        }

        return null;
    }

    public void SetLogInTime(string _timeStr)
    {
        string[] t = _timeStr.Split('/');
        if(t.Length == 6)
        {
            int year = Convert.ToInt32(t[0]);
            int month = Convert.ToInt32(t[1]);
            int day = Convert.ToInt32(t[2]);
            int hour = Convert.ToInt32(t[3]);
            int minute = Convert.ToInt32(t[4]);
            int second = Convert.ToInt32(t[5]);
            logInTime = new DateTime(year, month, day, hour, minute, second);
        }
    }

    public DateTime GetLogInTime()
    {
        return logInTime;
    }
    #endregion // Mission

    #region // Event
    void OnChangeUserMapList()
    {
        if (ChangeUserMapListEvent != null) ChangeUserMapListEvent();
    }

    void OnChangeItemSlot()
    {
        if (AddedInventorySizeEvent != null) AddedInventorySizeEvent();
    }

    void OnChangeChaSlot()
    {
        if (ChangedChaSlotEvent != null) ChangedChaSlotEvent(this.UserData);
    }

    void OnChangeCurCharacter()
    {
        if (ChangedCurCharacterEvent != null) ChangedCurCharacterEvent();
    }

    void OnNewCharacter()
    {
        if (NewChaEvent != null) NewChaEvent();
    }

    void OnNewMap()
    {
        if (NewMapEvent != null) NewMapEvent();
    }

    void OnNewItem()
    {
        if (NewItemEvent != null) NewItemEvent();
    }
    #endregion // Event
}
