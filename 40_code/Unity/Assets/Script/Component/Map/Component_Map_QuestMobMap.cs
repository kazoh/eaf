using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Kazoh.Table;

public class Component_Map_QuestMobMap : Component_Map
{
    public int TargetItemId;
    public int NeedItemNum;

    protected List<Component_Map_Monster> mobList;

    public override void Init(TableData_Map _data, Data_UserCharacter _cha)
    {
        try
        {
            base.Init(_data, _cha);
            SetMob();
        }    
        catch(Exception e)
        {
            throw e;
        }
    }

    void SetMob()
    {
        mobList = new List<Component_Map_Monster>();

        foreach(Component_Map_Object c in ObjList)
        {
            if(c is Component_Map_Monster)
            {
                Component_Map_Monster mob = c as Component_Map_Monster;
                mob.DieEvent += OnDieMonster;
                mobList.Add(mob);
            }
        }
    }

    void OnDieMonster(Component_Map_Monster mob)
    {
        if (mobList.Contains(mob)) mobList.Remove(mob);
        if (mobList.Count == 0)
        {
            if (CheckGettingItem()) OnGameClear();
            else OnGameOver();
        }
    }

    bool CheckGettingItem()
    {
        if (RewardInfo == null || RewardInfo.ItemRewardList == null) return false;
        int num = 0;
        for (int i = 0; i < RewardInfo.ItemRewardList.Count; ++i)
        {
            if (RewardInfo.ItemRewardList[i].ItemId == TargetItemId)
            {
                num += RewardInfo.ItemRewardList[i].Num;
            }
        }

        return num >= NeedItemNum;
    }
}
