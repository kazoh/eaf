using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Kazoh.Table;

public class Component_Map_QuestTalkMap : Component_Map
{
    public Component_Map_Dialog DialogUI;
    public Component_Map_NPC NpcWhenHasItem;
    public Component_Map_NPC NpcWhenHasNotItem;
    public int QuestItemId;
    public int NeedItemNum;

    private bool hasItem;

    public override void Init(TableData_Map _data, Data_UserCharacter _cha)
    {
        try
        {
            base.Init(_data, _cha);
            SetNpc();
            SetDialog();
            CheckQuestItem();
        }    
        catch(Exception e)
        {
            throw e;
        }
    }

    void SetNpc()
    {
        foreach(Component_Map_Object c in ObjList)
        {
            if(c is Component_Map_NPC)
            {
                (c as Component_Map_NPC).TalkEvent += TalkStart;
            }
        }
    }

    void SetDialog()
    {
        DialogUI.Init();
        DialogUI.CloseEvent += TalkEnd;
    }

    void CheckQuestItem()
    {
        List<Slot_Item> list = GameProcess.GetGameDataManager().GetInventory();
        for(int i=0; i<list.Count; ++i)
        {
            if (list[i].IsEmpty) continue;
            if (list[i].ItemId != QuestItemId || list[i].Data.Num < NeedItemNum) continue;
            hasItem = true;
            NpcWhenHasNotItem.Hide();
            return;
        }

        NpcWhenHasItem.Hide();
    }

    protected virtual void TalkStart(Component_Map_NPC npc)
    {
        DialogUI.Show(npc.Name, npc.DialogList);
    }

    protected virtual void TalkEnd()
    {
        if (hasItem)
        {
            GameProcess.GetGameDataManager().UseQuestItem(QuestItemId, NeedItemNum);
            OnGameClear();
        }
        else OnGameOver();
    }
}
