using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_Object_Door : Component_Map_Object, IManagedDepth
{
    public Action<int, int, Data_Reward> OpenEvent;

    public int NpcId;
    public int Range;
    public Component_Map_Cell LinkedCell;
    public UISprite NpcSprite;
    public UISprite BlideSprite;
    public bool IsLocked;
    public bool IsOpened;

    public TableData_Npc Data { get; protected set; }

    private string spriteFormat;
    private UISprite shadowSprite;

    public override void Init()
    {
        base.Init();
        Data = TableManager.GetGameData(NpcId) as TableData_Npc;
        if (Data == null) throw new GameException(GameException.ErrorCode.NoGameData);

        spriteFormat = Data.SpriteName + "_{0:00}";
        NpcSprite.spriteName = string.Format(spriteFormat, 1);
        if (BlideSprite != null) BlideSprite.alpha = 1f;
        Transform shadowTransform = transform.FindChild("shadow");
        if (shadowTransform != null) shadowSprite = shadowTransform.GetComponent<UISprite>();
    }

    bool CheckDistant()
    {
        float dist = Mathf.Pow(map.Player.Pos.y - Pos.y, 2) + Mathf.Pow(map.Player.Pos.x - Pos.x, 2);
        if (dist > Mathf.Pow(Range, 2)) return false;

        return true;
    }

    bool isLock;
    void OnClick()
    {
        if (isLock) return;
        isLock = true;
        if (CheckDistant())
        {
            Open();
        }
        else
        {
            LinkedCell.gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
        }
        isLock = false;
    }

    void Open()
    {
        if (!IsLocked)
        {
            IsOpened = !IsOpened;
            StopCoroutine("PlayAnim_Open");
            StopCoroutine("PlayAnim_Close");
            GameProcess.PlaySound(SOUND_EFFECT.OPEN);
            if (IsOpened) StartCoroutine("PlayAnim_Open");
            else StartCoroutine("PlayAnim_Close");
        }
    }

    void UnLock()
    {
        IsLocked = false;
    }

    IEnumerator PlayAnim_Open()
    {
        for(int i=0; i<3; ++i)
        {
            NpcSprite.spriteName = string.Format(spriteFormat, i % 3 + 1);
            yield return new WaitForSeconds(0.1f);
        }

        if (BlideSprite != null)
        {
            while(BlideSprite.alpha > 0)
            {
                BlideSprite.alpha -= 0.2f;
            }
            BlideSprite.alpha = 0f;
        }
        LinkedCell.IsBlock = false;
    }

    IEnumerator PlayAnim_Close()
    {
        for (int i = 2; i > -1; --i)
        {
            NpcSprite.spriteName = string.Format(spriteFormat, i % 3 + 1);
            yield return new WaitForSeconds(0.1f);
        }

        if (BlideSprite != null)
        {
            while (BlideSprite.alpha < 1)
            {
                BlideSprite.alpha += 0.2f;
            }
            BlideSprite.alpha = 1f;
        }
        LinkedCell.IsBlock = true;
    }

    public void SetDepth(int _depth)
    {
        if (NpcSprite.depth != _depth)
        {
            NpcSprite.depth = _depth;
            if (shadowSprite != null) shadowSprite.depth = _depth - 1;
            if (BlideSprite != null) BlideSprite.depth = _depth + 1;
        }
    }

    public float GetPosY()
    {
        return Pos.y;
    }
}
