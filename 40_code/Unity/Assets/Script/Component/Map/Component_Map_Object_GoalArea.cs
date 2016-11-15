using UnityEngine;
using System.Collections;
using System;

public class Component_Map_Object_GoalArea : Component_Map_Object
{
    public Action EnterAreaEvent;

    public int Width;
    public int Height;

    private UISprite areaSprite;

    public override void Init()
    {
        areaSprite = gameObject.GetComponentInChildren<UISprite>();
        if (areaSprite != null)
        {
            areaSprite.width = Width;
            areaSprite.height = Height;
        }

        StartCoroutine(Trace());
        base.Init();
    }

    IEnumerator Trace()
    {
        while(true)
        {
            yield return null;
            if (EnterArea()) break;
        }
        
        if (EnterAreaEvent != null) EnterAreaEvent();
    }

    bool EnterArea()
    {
        if (map.Player.Pos.y > transform.localPosition.y + Height * 0.5f) return false;
        if (map.Player.Pos.y < transform.localPosition.y - Height * 0.5f) return false;
        if (map.Player.Pos.x > transform.localPosition.x + Width * 0.5f) return false;
        if (map.Player.Pos.x < transform.localPosition.x - Width * 0.5f) return false;

        return true;
    }
}
