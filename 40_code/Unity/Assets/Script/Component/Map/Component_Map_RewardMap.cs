using UnityEngine;
using System.Collections;
using System;

public class Component_Map_RewardMap : Component_Map_MoveMap
{

    protected override void SetTimer()
    {
        Timer = TopPanel.GetComponentInChildren<Component_Map_Timer>();
        if (Timer == null) throw new GameException(GameException.ErrorCode.NoTimer);
        Timer.Init(this, PlayTime);
        Timer.TimeOverEvent += OnGameClear;
    }

    protected override int GetCrown()
    {
        float t = Timer.TotTime - Timer.CurTime;
        if (t < Data.Time_GCrown) return 3;
        if (t < Data.Time_SCrown) return 2;
        return 1;
    }
}
