using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Kazoh.Table;

public class Component_Map_GettingItemMap : Component_Map
{
    public int TargetItemId;
    public int NeedItemNum;

    protected override void OnReward(int _gold, int _coin, Data_Reward _reward)
    {
        base.OnReward(_gold, _coin, _reward);
        if (_reward == null || _reward.ItemRewardList == null) return;
        for(int i=0; i<_reward.ItemRewardList.Count; ++i)
        {
            if(_reward.ItemRewardList[i].ItemId == TargetItemId)
            {
                NeedItemNum -= _reward.ItemRewardList[i].Num;
            }
        }

        if (NeedItemNum <= 0) OnGameClear();
    }
}
