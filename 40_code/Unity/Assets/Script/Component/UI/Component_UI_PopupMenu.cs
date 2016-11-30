using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_UI_PopupMenu : GameComponent
{
    public Action<GameEnum.Menu> ClickedEvent;

    public UILabel TitleLabel;
    public UILabel CancelBtnLabel;

    public UILabel ChaLabel;
    public UILabel MapLabel;
    public UILabel InvenLabel;
    public UILabel ShopLabel;
    public UILabel CashLabel;
    public UILabel SummonLabel;
    public UILabel ExchargeLabel;
    public UILabel RankLabel;
    public UILabel DailyLabel;
    public UILabel LikeLabel;
    public UILabel ReviewLabel;
    public UILabel BugLabel;
    public UILabel SettingLabel;
    public UILabel ExitLabel;

    public override void Init()
    {
        base.Init();
        SetConstantUI();
    }

    void SetConstantUI()
    {
        TitleLabel.text = TableManager.GetString("STR_TITLE_POPUP_MENU");
        CancelBtnLabel.text = TableManager.GetString("STR_UI_CLOSE");

        ChaLabel.text = TableManager.GetString("STR_MENU_01");
        MapLabel.text = TableManager.GetString("STR_MENU_02");
        InvenLabel.text = TableManager.GetString("STR_MENU_03");
        ShopLabel.text = TableManager.GetString("STR_MENU_04");
        CashLabel.text = TableManager.GetString("STR_MENU_05");
        ExchargeLabel.text = TableManager.GetString("STR_MENU_07");
        RankLabel.text = TableManager.GetString("STR_MENU_08");
        DailyLabel.text = TableManager.GetString("STR_MENU_09");
        LikeLabel.text = TableManager.GetString("STR_MENU_10");
        ReviewLabel.text = TableManager.GetString("STR_MENU_11");
        BugLabel.text = TableManager.GetString("STR_MENU_12");
        SettingLabel.text = TableManager.GetString("STR_MENU_13");
        ExitLabel.text = TableManager.GetString("STR_MENU_14");
        SummonLabel.text = TableManager.GetString("STR_MENU_15");
    }

    void OnClicked(GameEnum.Menu menu)
    {
        if (ClickedEvent != null)
        {
            ClickedEvent(menu);
        }
    }

    public void OnClick_Cha()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.CHARACTER);
        GameProcess.GetGameDataManager().ConformCharacter();
        Hide();
    }

    public void OnClick_Inven()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.INVENTORY);
        Hide();
    }

    public void OnClick_Map()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.MAP);
        Hide();
    }

    public void OnClick_Shop()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.SHOP);
        Hide();
    }

    public void OnClick_Recharge()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.RECHARGE);
        Hide();
    }

    public void OnClick_Exchange()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.EXCHANGE);
        Hide();
    }

    public void OnClick_CashShop()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.CASHSHOP);
        Hide();
    }

    public void OnClick_Setup()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.SETUP);
        Hide();
    }

    public void OnClick_Rank()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.RANK);
        Hide();
    }

    public void OnClick_Daily()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.DAILY);
        Hide();
    }

    public void OnClick_Review()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.REVIEW);
        Hide();
    }

    public void OnClick_Bug()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.BUG);
        Hide();
    }

    public void OnClick_Exit()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.EXIT);
        Hide();
    }

    public void OnClick_Like()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.LIKE);
        Hide();
    }

    public void OnClick_Summon()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        OnClicked(GameEnum.Menu.SUMMON);
        Hide();
    }

    public void OnClick_Close()
    {
        if (gameObject.activeSelf)
        {
            GameProcess.PlaySound(SOUND_EFFECT.CLICK);
            Hide();
        }
    }
}
