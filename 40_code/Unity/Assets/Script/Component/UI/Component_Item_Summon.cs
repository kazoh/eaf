using UnityEngine;
using System.Collections;
using System;

using Kazoh.Table;

public class Component_Item_Summon : GameComponent
{
    public Action<Component_Item_Summon> ClickedEvent;

    public UISprite IconSprite;
    public UILabel NameLabel;
    public UILabel DescLabel;
    public UISprite MoneySprite;
    public UILabel PriceLabel;
    public UILabel MembershipLabel;

    public TableData_GoodsList Data { get; private set; }

    public void Init(TableData_GoodsList _data)
    {
        if (_data == null)
        {
            Hide();
            return;
        }

        Data = _data;

        IconSprite.spriteName = _data.IconName;
        NameLabel.text = TableManager.GetString(_data.Str);
        DescLabel.text = TableManager.GetString(_data.Desc);
        MoneySprite.spriteName = "coin_0" + (int)_data.Currency;
        PriceLabel.text = string.Format("{0:###,###,##0}", _data.Price);

        Show();
    }

    public override void Show()
    {
        base.Show();
        int membership = GameProcess.GetGameDataManager().GetMembership();
        if (membership < Data.Membership) MembershipLabel.text = TableManager.GetString("STR_UI_MEMBERSHIP_COND_" + Data.Membership);
        else MembershipLabel.text = "";
    }

    void OnClick()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (ClickedEvent != null) ClickedEvent(this);
    }
}