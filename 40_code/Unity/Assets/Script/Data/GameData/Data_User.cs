using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using MiniJSON;
using Kazoh.Table;

public class Data_User
{
    public Action<Data_User> ChangedEvent;

    public string Initial { get; private set; }

    public int Gold { get; private set; }
    public int Coin { get; private set; }
    public int Key { get; private set; }
    public int Cash { get; private set; }

    public int ChaSlotNum { get; private set; }
    public int InventorySlotNum { get; private set; }

    public int DailyCount { get; private set; }
    public long DailyTick { get; private set; }

    public int PurchaseAmount { get; private set; } 

    public Data_User()
    {
        Initial = "AAA";
        Gold = GameProcess.GetGameConfig().DefaultGold;
        Coin = GameProcess.GetGameConfig().DefaultCoin;
        Key = 10;
        Cash = 0;
        ChaSlotNum = GameProcess.GetGameConfig().DefaultChaSlotNum;
        InventorySlotNum = GameProcess.GetGameConfig().DefaultItemSlotNum;
        DailyCount = 0;
        DailyTick = 0;
        PurchaseAmount = 0;
    }

    public Data_User(int _gold, int _coin, int _key, int _cash, int _slot, int _inven) : this()
    {
        Gold = _gold;
        Coin = _coin;
        Key = _key;
        Cash = _cash;
        ChaSlotNum = _slot;
        InventorySlotNum = _inven;
    }

    public Data_User(IDictionary dict) : this()
    {
        if (dict.Contains("INITIAL")) Initial = System.Convert.ToString(dict["INITIAL"]);
        if (dict.Contains("GOLD")) Gold = System.Convert.ToInt32(dict["GOLD"]);
        if (dict.Contains("COIN")) Coin = System.Convert.ToInt32(dict["COIN"]);
        if (dict.Contains("KEY")) Key = System.Convert.ToInt32(dict["KEY"]);
        if (dict.Contains("CASH")) Cash = System.Convert.ToInt32(dict["CASH"]);
        if (dict.Contains("SLOT_NUM")) ChaSlotNum = System.Convert.ToInt32(dict["SLOT_NUM"]);
        if (dict.Contains("INVEN_NUM")) InventorySlotNum = System.Convert.ToInt32(dict["INVEN_NUM"]);
        if (dict.Contains("DAILY_TICK"))
        {
            DailyTick = System.Convert.ToInt64(dict["DAILY_TICK"]);

            DateTime last = new DateTime(DailyTick);
            DateTime now = GameProcess.GetGameDataManager().GetLogInTime();

            // 새로운 달이 시작되면 카운트를 초기화 한다.
            if (now.Year == last.Year && now.Month == last.Month && dict.Contains("DAILY_COUNT")) DailyCount = System.Convert.ToInt32(dict["DAILY_COUNT"]);
        }
        if (dict.Contains("PURCHASE_AMOUNT")) PurchaseAmount = System.Convert.ToInt32(dict["PURCHASE_AMOUNT"]);
    }

    public Dictionary<string, object> GetHash()
    {
        Dictionary<string, object> hash = new Dictionary<string, object>();
        hash.Add("INITIAL", Initial);
        hash.Add("GOLD", Gold);
        hash.Add("COIN", Coin);
        hash.Add("KEY", Key);
        hash.Add("CASH", Cash);
        hash.Add("SLOT_NUM", ChaSlotNum);
        hash.Add("INVEN_NUM", InventorySlotNum);
        hash.Add("DAILY_COUNT", DailyCount);
        hash.Add("DAILY_TICK", DailyTick);
        hash.Add("PURCHASE_AMOUNT", PurchaseAmount);

        return hash;
    }

    void OnChange()
    {
        if (ChangedEvent != null) ChangedEvent(this);
    }

    public void AddGold(int value)
    {
        Gold += value;
        OnChange();
    }

    public void AddCoin(int value)
    {
        Coin += value;
        OnChange();
    }

    public void AddKey(int value)
    {
        Key += value;
        OnChange();
    }

    public void AddCash(int value)
    {
        Cash += value;
        OnChange();
    }

    public void AddPurcase(int value)
    {
        PurchaseAmount += value;
    }

    public void AddChaSlot(int value)
    {
        ChaSlotNum += value;
    }

    public void AddItemSlot(int value)
    {
        InventorySlotNum += value;
    }

    public void EditInitial(string _initial)
    {
        Initial = _initial;
    }

    public bool HasDailyReward()
    {
        DateTime last = new DateTime(DailyTick);
        DateTime now = GameProcess.GetGameDataManager().GetLogInTime();
        
        // 날짜가 바뀌면 보상을 지급한다.
        if(now.Day != last.Day)
        {
            DailyCount++;
            DailyTick = now.Ticks;
            return true;
        }

        return false;
    }
}
