using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_Object_Box : Component_Map_Object, IManagedDepth
{
    public Action<int, int, Data_Reward> OpenEvent;

    public int NpcId;
    public int Range;
    public Component_Map_Cell LinkedCell;
    public UISprite NpcSprite;
    public GameObject PfCoinEffect;
    public GameObject PfItemEffect;

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
            map.Player.OnClickObject(this);
            Open();
        }
        else
        {
            LinkedCell.gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
            isLock = false;
        }
    }

    void Open()
    {
        StartCoroutine(PlayAnim());
    }

    IEnumerator PlayAnim()
    {
        float delay = GameProcess.GetEffectLength(SOUND_EFFECT.OPEN);
        GameProcess.PlaySound(SOUND_EFFECT.OPEN);
        NpcSprite.spriteName = string.Format(spriteFormat, 2);
        yield return new WaitForSeconds(delay);

        if(Data.SAtk > 0)
        {
            IAttackable target = map.Player;
            if (target != null) target.Attacked(transform.localPosition, Data.SAtk, false, false);
        }

        Data_Reward reward = Data_Reward.DoDrop(Data.DropList);
        if (OpenEvent != null)
        {
            OpenEvent(0, 0, reward);
        }
        if(reward.ItemRewardList.Count > 0)
        {
            TableData_Item item = TableManager.GetGameData(reward.ItemRewardList[0].ItemId) as TableData_Item;
            if(item != null)
            {
                ShowEffect(item.IconName,PfItemEffect);
            }
        }
        else if(reward.Gold > 0)
        {
            ShowEffect("coin_01",PfCoinEffect);
        }
        else if(reward.Coin > 0)
        {
            ShowEffect("coin_02",PfCoinEffect);
        }
        Hide();
    }

    public override void Hide()
    {
        BoxCollider coll = GetComponent<BoxCollider>();
        if (coll != null) coll.enabled = false;
        StartCoroutine(Despawn());
    }

    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(1);
        while(NpcSprite.alpha > 0)
        {
            NpcSprite.alpha -= 0.1f;
            yield return null;
        }
        LinkedCell.IsBlock = false;
    }

    void ShowEffect(string _sprite, GameObject _pf)
    {
        GameObject go = NGUITools.AddChild(transform.parent.gameObject, _pf);
        Component_Effect_Coin effect = go.GetComponent<Component_Effect_Coin>();
        if (effect != null)
        {
            go.transform.position = transform.position;
            effect.Init(_sprite);
            effect.Play();
        }
        else Destroy(go);
    }

    public void SetDepth(int _depth)
    {
        if (NpcSprite.depth != _depth)
        {
            NpcSprite.depth = _depth;
            if (shadowSprite != null) shadowSprite.depth = _depth - 1;
        }
    }

    public float GetPosY()
    {
        return Pos.y;
    }
}
