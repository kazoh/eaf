using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Kazoh.Table;

public class Component_Map_TalkMap2 : Component_Map_TalkMap
{
    public Component_Map_NPC KeyNpc;
    private Component_Map_NPC curNpc;

    protected override void TalkStart(Component_Map_NPC npc)
    {
        base.TalkStart(npc);
        curNpc = npc;
    }

    protected override void TalkEnd()
    {
        if(curNpc == KeyNpc) OnGameClear();
    }
}
