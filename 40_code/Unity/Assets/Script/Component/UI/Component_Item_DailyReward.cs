using UnityEngine;
using System.Collections;
using System;

using Kazoh.Table;

public class Component_Item_DailyReward : GameComponent
{
    public Action<Component_Item_DailyReward> ClickedEvent;

    public UILabel DayLabel;
    public UILabel TextLabel;
    public UISprite ItemIconSprite;
    public UISprite ItemGradeSprite;
    public UISprite CoinAndGoldIconSprite;
    public GameObject ItemRoot;
    public GameObject CoinAndGoldRoot;
    public UISprite MarkSprite;

    public TableData_DailyReward Data { get; private set; }

    private Animation anim;

    public bool IsRewarded { get; private set; }

    public void Init(TableData_DailyReward _data, int _count)
    {
        if(_data == null)
        {
            Hide();
            return;
        }

        Data = _data;

        anim = GetComponent<Animation>();

        if (_data.Id > _count)
        {
            IsRewarded = false;
            MarkSprite.alpha = 0;
        }
        else
        {
            IsRewarded = true;
            MarkSprite.alpha = 1;
        }

        DayLabel.text = string.Format(TableManager.GetString("STR_UI_DAILY"), Data.Id);

        ItemRoot.SetActive(false);
        CoinAndGoldRoot.SetActive(false);

        switch (Data.Type)
        {
            case TableEnum.MissionType.Coin:
                CoinAndGoldIconSprite.spriteName = "coin_02";
                TextLabel.text = string.Format("x {0}", Data.Reward);
                CoinAndGoldRoot.SetActive(true);
                break;
            case TableEnum.MissionType.Gold:
                CoinAndGoldIconSprite.spriteName = "coin_01";
                TextLabel.text = string.Format("{0:###,###,##0}", Data.Reward);
                CoinAndGoldRoot.SetActive(true);
                break;
            case TableEnum.MissionType.Item:
                TableData_Item _item = TableManager.GetGameData(Data.Reward) as TableData_Item;
                if(_item == null)
                {
                    Debug.LogError("잘못된 아이템 아이디:" + Data.Reward);
                    Hide();
                    break;
                }
                ItemIconSprite.spriteName = _item.IconName;
                ItemGradeSprite.spriteName = "grade_0" + _item.Grade;
                TextLabel.text = TableManager.GetString(_item.Str);
                ItemRoot.SetActive(true);
                break;
        }

        Show();
    }

    public void PlaySound()
    {
        GameProcess.PlaySound(SOUND_EFFECT.SPAWN);
    }

    public void ShowMark()
    {
        IsRewarded = true;
        if (anim != null) anim.Play();
    }
}