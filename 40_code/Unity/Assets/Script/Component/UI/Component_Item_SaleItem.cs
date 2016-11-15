using UnityEngine;
using System.Collections;
using System;

using Kazoh.Table;


public class Component_Item_SaleItem : GameComponent {

    public Action<Component_Item_SaleItem> ClickedEvent;

    public UISprite IconSprite;
    public UISprite GradeSprite;
    public UILabel NameLabel;
    public UISprite MoneySprite;
    public UILabel PriceLabel;

    public TableData_SaleList Data { get; private set; }

    public void Init(TableData_SaleList _data)
    {
        Data = _data;
        if(Data != null)
        {
            TableData_Item item = TableManager.GetGameData(Data.ItemName) as TableData_Item;
            if (item != null)
            {
                IconSprite.spriteName = item.IconName;
                GradeSprite.spriteName = "grade_0" + item.Grade;
                if (Data.Amount > 1) NameLabel.text = string.Format("{0} x {1}", TableManager.GetString(item.Str), Data.Amount);
                else NameLabel.text = TableManager.GetString(item.Str);
                MoneySprite.spriteName = "coin_0" + (int)Data.Type;
                int price = Data.Type == TableEnum.MoneyType.Gold ? item.Price * Data.Amount : item.CoinPrice * Data.Amount;
                PriceLabel.text = string.Format("{0:###,###,##0}", price);
                Show();
            }
            else
            {
                Hide();
            }
        }
        else
        {
            Hide();
        }
    }

    void OnClick()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (ClickedEvent != null) ClickedEvent(this);
    }
}
