using UnityEngine;
using System.Collections;
using System;

using Kazoh.Table;

public class Component_Item_Goods : GameComponent
{

    public Action<Component_Item_Goods> ClickedEvent;

    public UISprite IconSprite;
    public UILabel NameLabel;
    public UISprite MoneySprite;
    public UILabel PriceLabel;
    public UILabel MembershipLabel;

    public TableData_GoodsList Data { get; private set; }

    public void Init(TableData_GoodsList _data)
    {
        if(_data == null)
        {
            Hide();
            return;
        }

        Data = _data;

        IconSprite.spriteName = _data.IconName;
        NameLabel.text = TableManager.GetString(_data.Str);
        if(_data.Type == TableEnum.GoodsType.Cash)
        {
            MoneySprite.spriteName = "none";

#if UNITY_EDITOR
            switch (GameProcess.Instance.Language)
            {
                case SystemLanguage.Korean: PriceLabel.text = string.Format("{0:###,###,##0}원", _data.Price_WON); break;
                case SystemLanguage.Japanese: PriceLabel.text = string.Format("{0:###,###,##0}円", _data.Price_YEN); break;
                case SystemLanguage.English: PriceLabel.text = string.Format("USD {0:###,##0.00}", _data.Price_USD); break;
                default: PriceLabel.text = string.Format("USD {0:###,##0.00}", _data.Price_USD); break;
            }
#elif UNITY_IPHONE
            PriceLabel.text = string.Format("USD {0:###,##0.00}", _data.Price_USD);
#else
            switch(Application.systemLanguage)
            {
                case SystemLanguage.Korean: PriceLabel.text = string.Format("{0:###,###,##0}원", _data.Price_WON); break;
                case SystemLanguage.Japanese: PriceLabel.text = string.Format("{0:###,###,##0}円", _data.Price_YEN); break;
                case SystemLanguage.English: PriceLabel.text = string.Format("USD {0:###,##0.00}", _data.Price_USD); break;
                default: PriceLabel.text = string.Format("USD {0:###,##0.00}", _data.Price_USD); break;                
            }
#endif

        }
        else
        {
            MoneySprite.spriteName = "coin_0" + (int)_data.Currency;
            PriceLabel.text = string.Format("{0:###,###,##0}", _data.Price);

        }
        Show();
    }

    public override void Show()
    {
        int membership = GameProcess.GetGameDataManager().GetMembership();
        if (membership < Data.Membership)
        {
            if (Data.Type == TableEnum.GoodsType.Cash) Hide();
            else
            {
                MembershipLabel.text = TableManager.GetString("STR_UI_MEMBERSHIP_COND_" + Data.Membership);
                base.Show();
            }
        }
        else
        {
            MembershipLabel.text = "";
            base.Show();
        }
    }

    void OnClick()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (ClickedEvent != null) ClickedEvent(this);
    }
}