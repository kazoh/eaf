using UnityEngine;
using System.Collections;
using System;

using Kazoh.Table;

public class Component_Object_Hole : Component_Map_Object
{
    public int NpcId;
    public int Width;
    public int Height;
    public int StartNum;
    public int SpriteNum;

    public TableData_Npc Data { get; protected set; }

    private UISprite npcSprite;
    private string spriteFormat;

    public override void Init()
    {
        npcSprite = gameObject.GetComponentInChildren<UISprite>();
        Data = TableManager.GetGameData(NpcId) as TableData_Npc;
        if (Data != null)
        {
            spriteFormat = Data.SpriteName + "_{0:00}";
            //npcSprite.spriteName = string.Format(spriteFormat, StartNum);
        }

        StartCoroutine(Trace());
        base.Init();
    }

    IEnumerator PlayAnimation()
    {
        for (int i = 0; i < SpriteNum; ++i)
        {
            npcSprite.spriteName = string.Format(spriteFormat, StartNum + i);
            yield return null;
        }

        OnEvent(map.Player.gameObject);
    }

    IEnumerator Trace()
    {
        while (true)
        {
            if (EnterArea())
            {
                StartCoroutine(PlayAnimation());
                yield break;
            }
            yield return null;
        }
    }

    bool EnterArea()
    {
        if (map.Player.Pos.y > transform.localPosition.y + Height * 0.5f) return false;
        if (map.Player.Pos.y < transform.localPosition.y - Height * 0.5f) return false;
        if (map.Player.Pos.x > transform.localPosition.x + Width * 0.5f) return false;
        if (map.Player.Pos.x < transform.localPosition.x - Width * 0.5f) return false;

        return true;
    }

    void OnEvent(GameObject target)
    {
        if (target != null) target.SendMessage("Falling",SendMessageOptions.DontRequireReceiver);
    }
}
