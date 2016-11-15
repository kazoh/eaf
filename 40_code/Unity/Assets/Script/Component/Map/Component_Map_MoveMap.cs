using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Kazoh.Table;

public class Component_Map_MoveMap : Component_Map
{
    public override void Init(TableData_Map _data, Data_UserCharacter _cha)
    {
        try
        {
            base.Init(_data, _cha);
            SetGoalObj();
        }    
        catch(Exception e)
        {
            throw e;
        }
    }

    void SetGoalObj()
    {
        foreach(Component_Map_Object c in ObjList)
        {
            if(c is Component_Map_Object_GoalArea)
            {
                (c as Component_Map_Object_GoalArea).EnterAreaEvent += OnGameClear;
            }
        }
    }
}
