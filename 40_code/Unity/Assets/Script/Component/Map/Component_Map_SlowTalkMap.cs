using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Kazoh.Table;

public class Component_Map_SlowTalkMap : Component_Map
{
    public Component_Map_Dialog DialogUI;

    private bool isTalkStart;

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

    void TalkStart(Component_Map_NPC npc)
    {
        if (isTalkStart) return;

        isTalkStart = true;

        List<string> list = new List<string>();
        for(int i=0; i < npc.DialogList.Count; ++i)
        {
            list.AddRange(SplitDialog(npc.DialogList[i]));
        }
        DialogUI.Show(npc.Name, list);
    }

    void TalkEnd()
    {
        OnGameClear();

        isTalkStart = false;
    }

    List<string> SplitDialog(string dialog)
    {
        List<string> list = new List<string>();
        if (string.IsNullOrEmpty(dialog)) return list;

        for (int i=0; i<dialog.Length; ++i)
        {
            list.Add(dialog.Substring(0, i + 1));
        }

        return list;
    }
}
