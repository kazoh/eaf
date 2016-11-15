using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Kazoh.Table;

public class Component_Map_TalkMap : Component_Map
{
    public Component_Map_Dialog DialogUI;
    public override void Init(TableData_Map _data, Data_UserCharacter _cha)
    {
        try
        {
            base.Init(_data, _cha);
            SetNpc();
            SetDialog();
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

    protected virtual void TalkStart(Component_Map_NPC npc)
    {
        DialogUI.Show(npc.Name, npc.DialogList);
    }

    protected virtual void TalkEnd()
    {
        OnGameClear();
    }
}
