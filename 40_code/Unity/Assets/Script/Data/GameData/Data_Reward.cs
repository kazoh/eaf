using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Kazoh.Table;

public class Data_Reward
{
    public class Reward
    {
        public int ItemId;
        public int Num;

        public Reward(int _id)
        {
            ItemId = _id;
            Num = 1;
        }
    }

    public int Gold { get; private set; }
    public int Coin { get; private set; }

    public List<Reward> ItemRewardList;
    
    public Data_Reward()
    {
        Gold = 0;
        Coin = 0;
        ItemRewardList = new List<Reward>();
    }

    public void SetReward(int _gold, int _coin, List<TableData_Drop> _list = null)
    {
        try
        {
            Data_Reward drop = Data_Reward.DoDrop(_list);
            Gold += _gold + drop.Gold;
            Coin += _coin + drop.Coin;
            ItemRewardList.AddRange(drop.ItemRewardList);

#if UNITY_EDITOR
            Debug.Log(GetInfo());
#endif
        }
        catch(System.Exception e)
        {
            throw e;
        }
    }

    public void SetReward(int _gold, int _coin, Data_Reward _reward)
    {
        try
        {
            Gold += _gold;
            Coin += _coin;
            if (_reward != null)
            {
                Gold += _reward.Gold;
                Coin += _reward.Coin;
                ItemRewardList.AddRange(_reward.ItemRewardList);
            }

#if UNITY_EDITOR
            Debug.Log(GetInfo());
#endif
        }
        catch (System.Exception e)
        {
            throw e;
        }
    }

    public static Data_Reward DoDrop(string _dropName)
    {
        TableData_Drop _drop = TableManager.GetGameData(_dropName) as TableData_Drop;
        return DoDrop(_drop);
    }

    public static Data_Reward DoDrop(List<string> _dropNameList)
    {
        List<TableData_Drop> _dropList = new List<TableData_Drop>();
        if (_dropNameList != null)
        {
            for (int i = 0; i < _dropNameList.Count; ++i)
            {
                TableData_Drop _drop = TableManager.GetGameData(_dropNameList[i]) as TableData_Drop;
                if (_drop != null) _dropList.Add(_drop);
            }
        }
        return DoDrop(_dropList);
    }

    public static Data_Reward DoDrop(TableData_Drop _data)
    {
        List<TableData_Drop> list = new List<TableData_Drop>();
        if(_data != null) list.Add(_data);
        return DoDrop(list);
    }

    public static Data_Reward DoDrop(List<TableData_Drop> _list)
    {
        Data_Reward reward = new Data_Reward();
        if (_list != null)
        {
            for (int i = 0; i < _list.Count; ++i)
            {
                if (_list[i] == null) continue;

                reward.Gold += Random.Range(_list[i].MinGold, _list[i].MaxGold);
                reward.Coin += Random.Range(_list[i].MinCoin, _list[i].MaxCoin);

                string itemName = _list[i].Drop();
                if (!string.IsNullOrEmpty(itemName))
                {
                    TableData_Item item = TableManager.GetGameData(itemName) as TableData_Item;
                    if (item != null)
                    {
                        reward.ItemRewardList.Add(new Reward(item.Id));
                    }
                }
            }
        }

        return reward;
    }

    public string GetInfo()
    {
        string info = string.Format("골드 : {0} 코인 : {1} 아이템 : ", Gold, Coin);
        for(int i=0; i < ItemRewardList.Count; ++i)
        {
            info += ItemRewardList[i].ItemId + ",";
        }

        return info;
    }
}
