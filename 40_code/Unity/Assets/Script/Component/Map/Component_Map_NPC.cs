using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_Map_NPC : Component_Map_Object, ITalkable
{
    public Action<Component_Map_NPC> TalkEvent;

    public int NpcId;
    public int Range;
    public bool HasShadow = true;
    public Component_Map_Cell LinkedCell;
    public UISprite NpcSprite;    

    public List<string> DialogList;
    public TableData_Npc Data { get; protected set; }

    private string spriteFormat;
    private UISprite shadowSprite;

    public override void Init()
    {
        base.Init();
        Data = TableManager.GetGameData(NpcId) as TableData_Npc;
        if (Data == null) throw new GameException(GameException.ErrorCode.NoGameData);
        Name = TableManager.GetString(Data.Str);
        DialogList = new List<string>();
        foreach(string dialog in Data.DialogList)
        {
            if(!string.IsNullOrEmpty(dialog)) DialogList.Add(TableManager.GetString(dialog));
        }
        spriteFormat = Data.SpriteName + "_{0:00}";
        LookAt(GameEnum.Direction.down);
        Transform shadowTransform = transform.FindChild("shadow");
        if (shadowTransform != null)
        {
            if (HasShadow)
            {
                shadowSprite = shadowTransform.GetComponent<UISprite>();
                if (shadowSprite != null) shadowSprite.alpha = 1f;
            }
            else
            {
                shadowSprite = shadowTransform.GetComponent<UISprite>();
                if (shadowSprite != null) shadowSprite.alpha = 0f;
            }
        }
    }

    bool CheckDistant()
    {
        float dist = Mathf.Pow(map.Player.Pos.y - Pos.y, 2) + Mathf.Pow(map.Player.Pos.x - Pos.x, 2);
        if (dist > Mathf.Pow(Range, 2)) return false;

        return true;
    }

    public virtual void Talk(Component_Map_Player player)
    {
        LookAt(player.Pos);
        StartCoroutine(WaitTalk());
    }

    void LookAt(Vector3 _pos)
    {
        GameEnum.Direction dir = GameEnum.Direction.down;
        if (_pos.x > transform.localPosition.x) dir = GameEnum.Direction.right;
        if (_pos.x < transform.localPosition.x) dir = GameEnum.Direction.left;
        if (_pos.y > transform.localPosition.y) dir = GameEnum.Direction.up;

        LookAt(dir);
    }

    void LookAt(GameEnum.Direction dir)
    {
        NpcSprite.spriteName = string.Format(spriteFormat, (1 + (int)dir));
    }

    void OnTalk()
    {
        if (TalkEvent != null) TalkEvent(this);
    }

    bool isLock;
    void OnClick()
    {
        if (isLock) return;
        isLock = true;
        if (CheckDistant()) map.Player.OnClickObject(this);
        else LinkedCell.gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
        isLock = false;
    }

    IEnumerator WaitTalk()
    {
        yield return null;
        OnTalk();
    }

    public void SetDepth(int _depth)
    {
        if (NpcSprite.depth != _depth)
        {
            NpcSprite.depth = _depth;
            //if (shadowSprite != null) shadowSprite.depth = _depth - 1;
        }
    }

    public float GetPosY()
    {
        return Pos.y;
    }
}
