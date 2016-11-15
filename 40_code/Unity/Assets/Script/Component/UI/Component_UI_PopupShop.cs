using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_UI_PopupShop : GameComponent
{
    public Action<GameComponent> ClickedEvent;

    public UILabel BtnLabel;
    public UIScrollView ScrollView;
    public UIGrid ExchangeGrid;
    public UIGrid RechargeGrid;
    public UIGrid CashShopGrid;
    public UIGrid SummonGrid;

    public GameObject PrefGoodsItem;
    public GameObject PrefCashItem;
    public GameObject PrefSummonItem;

    private List<Component_Item_Goods> cashGoodsList;
    private List<Component_Item_Summon> summonList;

    public override void Init()
    {
        base.Init();

        /* 스크롤 뷰 패널 뎁스 설정 */
        UIPanel panel = transform.parent.GetComponent<UIPanel>();
        if (panel != null) ScrollView.panel.depth = panel.depth + 1;

        /* 버튼 텍스트 설정 */
        BtnLabel.text = TableManager.GetString("STR_UI_CLOSE");

        /* 상품 리스트 획득 */
        List<TableData_GoodsList> list = TableManager.GetGoodsList();

        /* 환전 리스트 설정 */
        foreach (TableData_GoodsList item in list)
        {
            if (item.Type != TableEnum.GoodsType.CashToCoin && item.Type != TableEnum.GoodsType.CashToGold) continue;
            GameObject go = NGUITools.AddChild(ExchangeGrid.gameObject, PrefGoodsItem);
            Component_Item_Goods component = go.GetComponent<Component_Item_Goods>();
            if (component != null)
            {
                component.Init(item);
                component.ClickedEvent += OnSelect;
            }
        }
        ExchangeGrid.Reposition();

        /* 충전 리스트 설정 */
        foreach (TableData_GoodsList item in list)
        {
            if (item.Type != TableEnum.GoodsType.CoinToKey) continue;
            GameObject go = NGUITools.AddChild(RechargeGrid.gameObject, PrefGoodsItem);
            Component_Item_Goods component = go.GetComponent<Component_Item_Goods>();
            if (component != null)
            {
                component.Init(item);
                component.ClickedEvent += OnSelect;
            }
        }
        RechargeGrid.Reposition();

        /* 캐쉬구매 리스트 설정 */
        cashGoodsList = new List<Component_Item_Goods>();
        foreach (TableData_GoodsList item in list)
        {
            if (item.Type != TableEnum.GoodsType.Cash) continue;
            GameObject go = NGUITools.AddChild(CashShopGrid.gameObject, PrefCashItem);
            Component_Item_Goods component = go.GetComponent<Component_Item_Goods>();
            if (component != null)
            {
                component.Init(item);
                component.ClickedEvent += OnSelect;
                cashGoodsList.Add(component);
            }
        }
        CashShopGrid.Reposition();

        /* 소환 리스트 설정 */
        summonList = new List<Component_Item_Summon>();
        foreach (TableData_GoodsList item in list)
        {
            if (item.Type != TableEnum.GoodsType.Summon) continue;
            GameObject go = NGUITools.AddChild(SummonGrid.gameObject, PrefSummonItem);
            Component_Item_Summon component = go.GetComponent<Component_Item_Summon>();
            if (component != null)
            {
                component.Init(item);
                component.ClickedEvent += OnSelect;
                summonList.Add(component);
            }
        }
        SummonGrid.Reposition();
    }

    public void Show(GameEnum.Menu _type)
    {
        if (_type == GameEnum.Menu.EXCHANGE)
        {
            ExchangeGrid.gameObject.SetActive(true);
            CashShopGrid.Reposition();
        }
        else ExchangeGrid.gameObject.SetActive(false);
        if (_type == GameEnum.Menu.RECHARGE) RechargeGrid.gameObject.SetActive(true);
        else RechargeGrid.gameObject.SetActive(false);
        if (_type == GameEnum.Menu.CASHSHOP)
        {
            CashShopGrid.gameObject.SetActive(true);
            for(int i=0; i < cashGoodsList.Count; ++i)
            {
                cashGoodsList[i].Show();
            }
            CashShopGrid.Reposition();
        }
        else CashShopGrid.gameObject.SetActive(false);
        if (_type == GameEnum.Menu.SUMMON)
        {
            SummonGrid.gameObject.SetActive(true);
            for (int i = 0; i < summonList.Count; ++i)
            {
                summonList[i].Show();
            }
        }
        else SummonGrid.gameObject.SetActive(false);

        Show();
        if (_type != GameEnum.Menu.RECHARGE) ScrollView.ResetPosition();
    }

    void OnSelect(GameComponent item)
    {
        if (ClickedEvent != null) ClickedEvent(item);
    }

    public void OnClick_Close()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        Hide();
    }
}
