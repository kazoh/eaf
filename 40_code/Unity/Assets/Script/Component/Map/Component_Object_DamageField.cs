using UnityEngine;
using System.Collections;
using System;

using Kazoh.Table;

public class Component_Object_DamageField : Component_Map_Object
{
    public int NpcId;
    public int Width;
    public int Height;
    public int StartNum;
    public int SpriteNum;
    public float WaitTime;

    public TableData_Npc Data { get; protected set; }

    private UISprite npcSprite;
    private string spriteFormat;
    private float delay;
    private bool isTrace;

    public override void Init()
    {
        npcSprite = gameObject.GetComponentInChildren<UISprite>();
        Data = TableManager.GetGameData(NpcId) as TableData_Npc;
        if(Data != null)
        {
            delay = Data.ASpd * 0.001f;
            spriteFormat = Data.SpriteName + "_{0:00}";
            npcSprite.spriteName = string.Format(spriteFormat, StartNum);
        }

        StartCoroutine(Trace());
        StartCoroutine(PlayAnimation());
        base.Init();
    }

    IEnumerator PlayAnimation()
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);

            isTrace = true;

            for (int i=0; i < SpriteNum; ++i)
            {
                npcSprite.spriteName = string.Format(spriteFormat, StartNum + i);
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(WaitTime);
            isTrace = false;
            npcSprite.spriteName = string.Format(spriteFormat, StartNum);
        }
    }

    IEnumerator Trace()
    {
        while (true)
        {
            if (isTrace && EnterArea())
            {
                isTrace = false;
                OnEvent(map.Player as IAttackable);
                
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

    void OnEvent(IAttackable target)
    {
        if (target != null) target.Attacked(transform.localPosition, Data.SAtk, false, false); 
    }
}
