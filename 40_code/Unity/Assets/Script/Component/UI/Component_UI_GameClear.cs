using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_UI_GameClear : GameComponent
{
    public Action ClickedEvent;

    public UILabel GameClearLabel;
    public UILabel ExitLabel;
    public UISprite CrownSprite;
    public UILabel GoldLabel;
    public UIScrollView ScrollView;
    public UIGrid ItemGrid;
    public GameObject PrefItem;

    private Animation anim;
    private List<Component_Item_RewardItem> itemList;

    public override void Init()
    {
        anim = GetComponent<Animation>();
        itemList = new List<Component_Item_RewardItem>();
        SetItem();
        GameClearLabel.text = TableManager.GetString("STR_TITLE_GAMECLEAR");
        base.Init();
    }

    public void Show(int crown, Data_Reward reward)
    {
        isLock = true;
        CrownSprite.spriteName = "crown_0"+crown;
        GoldLabel.text = string.Format("{0:###,###,##0}", reward.Gold);
        ExitLabel.text = "";
        for(int i=0; i < itemList.Count; ++i)
        {
            if (i < reward.ItemRewardList.Count)
            {
                TableData_Item item = TableManager.GetGameData(reward.ItemRewardList[i].ItemId) as TableData_Item;
                if (item != null)
                {
                    itemList[i].Show(item, reward.ItemRewardList[i].Num);
                }
            }
            else itemList[i].Hide();
        }
        base.Show();
        ItemGrid.Reposition();
        ScrollView.ResetPosition();
        if (anim != null) anim.Play();
    }

    void SetItem()
    {
        for (int i = 0; i < 10; ++i)
        {
            GameObject go = NGUITools.AddChild(ItemGrid.gameObject, PrefItem);
            Component_Item_RewardItem component = go.GetComponent<Component_Item_RewardItem>();
            if (component != null)
            {
                component.Init();
                component.Hide();
                itemList.Add(component);
            }
            else go.SetActive(false);
        }
        ItemGrid.Reposition();
        ScrollView.ResetPosition();
    }

    bool isLock;
    void OnClick()
    {
        if (isLock) return;
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        Hide();
        if (ClickedEvent != null) ClickedEvent();
    }

    public void PlaySound()
    {
        GameProcess.PlaySound(SOUND_EFFECT.SPAWN);
    }

    public void OnFinish()
    {
        anim.Stop();
        StartCoroutine(WaitFinish());
    }

    IEnumerator WaitFinish()
    {
        GameProcess.PlaySound(SOUND_EFFECT.WIN);
        yield return new WaitForSeconds(GameProcess.GetEffectLength(SOUND_EFFECT.WIN));
        ExitLabel.text = TableManager.GetString("STR_MSG_TOUCH");
        isLock = false;
    }
}
