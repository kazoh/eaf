using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_UI_Shop : GameComponent
{
    public Action<Component_UI_Shop> ClickedEvent;

    public UILabel NameLabel;
    public UISprite IconSprite;
    public UISprite GradeSprite;
    public UISprite MoneySprite;
    public UILabel PriceLabel;
    public UILabel DescLabel;
    public UILabel AtkLabel;
    public UILabel AspdLabel;
    public UILabel DefLabel;
    public UILabel SpdLabel;
    public UILabel HpLabel;
    public UILabel AtkValueLabel;
    public UILabel AspdValueLabel;
    public UILabel DefValueLabel;
    public UILabel SpdValueLabel;
    public UILabel HpValueLabel;
    public UILabel BtnLabel;

    public UIGrid TabGrid;
    public GameObject PrefSaleTab;

    public UIScrollView ScrollView;
    public UIGrid ListGrid;
    public GameObject PrefSaleItem;

    private Component_Item_SaleTab curTab;
    public Component_Item_SaleTab SelectedTab
    {
        get { return curTab; }
        private set
        {
            if (curTab == value) return;
            if (curTab != null) curTab.IsSelected = false;
            curTab = value;
            curTab.IsSelected = true;
            UpdateSaleList();
            OnSelectList(saleItemList[0]);
        }
    }

    private Component_Item_SaleItem curItem;
    public Component_Item_SaleItem SelectedItem
    {
        get { return curItem; }
        private set
        {
            curItem = value;
            UpdateItemInfo();
        }
    }

    private List<TableData_SaleList> saleList;
    private List<Component_Item_SaleItem> saleItemList;

    public override void Init()
    {
        base.Init();

        /* 스크롤 뷰 패널 뎁스 설정 */
        UIPanel panel = transform.parent.GetComponent<UIPanel>();
        if (panel != null) ScrollView.panel.depth = panel.depth + 1;

        /* 상품 리스트 설정 */
        saleItemList = new List<Component_Item_SaleItem>();
        saleList = TableManager.GetSaleList();
        saleList.Sort(
                delegate (TableData_SaleList x, TableData_SaleList y)
                {
                    if(x.Type == y.Type)
                    {
                        if (x.Category == y.Category)
                        {
                            if (x.Category == TableEnum.SaleCatecory.Equip)
                            {
                                TableData_Item xItem = TableManager.GetGameData(x.ItemName) as TableData_Item;
                                TableData_Item yItem = TableManager.GetGameData(y.ItemName) as TableData_Item;
                                if (xItem.SlotIdx == yItem.SlotIdx)
                                {
                                    if (xItem.Grade == yItem.Grade) return xItem.Id.CompareTo(yItem.Id);
                                    return xItem.Grade.CompareTo(yItem.Grade);
                                }
                                return xItem.SlotIdx.CompareTo(yItem.SlotIdx);
                            }
                            return x.Id.CompareTo(y.Id);
                        }
                        return x.Category.CompareTo(y.Category);
                    }
                    return x.Type.CompareTo(y.Type);
                });
        foreach(TableData_SaleList item in saleList)
        {
            GameObject go = NGUITools.AddChild(ListGrid.gameObject, PrefSaleItem);
            Component_Item_SaleItem component = go.GetComponent<Component_Item_SaleItem>();
            if (component != null)
            {
                component.Init(item);
                component.ClickedEvent += OnSelectList;
                saleItemList.Add(component);
            }
        }


        /* 탭 설정 */
        List<Component_Item_SaleTab> tabList = new List<Component_Item_SaleTab>();
        foreach (int i in Enum.GetValues(typeof(TableEnum.SaleCatecory)))
        {
            GameObject go = NGUITools.AddChild(TabGrid.gameObject, PrefSaleTab);
            Component_Item_SaleTab tab = go.GetComponent<Component_Item_SaleTab>();
            if (tab != null)
            {
                tab.Init(i);
                tab.ClickedEvent += OnSelectTab;
                tab.Show();
                tabList.Add(tab);
            }
        }
        TabGrid.repositionNow = true;

        /* 초기 탭 선택 */
        if (tabList.Count > 0) OnSelectTab(tabList[0]);
    }

    void UpdateItemInfo()
    {
        if (SelectedItem != null && SelectedItem.Data != null)
        {
            TableData_Item item = TableManager.GetGameData(SelectedItem.Data.ItemName) as TableData_Item;
            if (item != null)
            {
                IconSprite.spriteName = item.IconName;
                GradeSprite.spriteName = "grade_0" + item.Grade;
                if (SelectedItem.Data.Amount > 1) NameLabel.text = string.Format("{0} x {1}", TableManager.GetString(item.Str), SelectedItem.Data.Amount);
                else NameLabel.text = TableManager.GetString(item.Str);
                MoneySprite.spriteName = "coin_0" + (int)SelectedItem.Data.Type;
                int price = SelectedItem.Data.Type == TableEnum.MoneyType.Gold ? item.Price * SelectedItem.Data.Amount : item.CoinPrice * SelectedItem.Data.Amount;
                PriceLabel.text = string.Format("{0:###,###,##0}", price);
                DescLabel.text = TableManager.GetString(item.Desc);
                AtkLabel.text = TableManager.GetString("STR_UI_ATK");
                AspdLabel.text = TableManager.GetString("STR_UI_ASPD");
                DefLabel.text = TableManager.GetString("STR_UI_DEF");
                SpdLabel.text = TableManager.GetString("STR_UI_SPD");
                HpLabel.text = TableManager.GetString("STR_UI_HP");
                BtnLabel.text = TableManager.GetString("STR_UI_BUY");
                AtkValueLabel.text = string.Format("{0}/{1}",item.SAtk, item.LAtk);

                if (item.SlotIdx == 1) AspdValueLabel.text = string.Format("{0:0.00}", item.ASpd * 0.001f);
                else AspdValueLabel.text = string.Format("{0}", item.ASpd);

                DefValueLabel.text = string.Format("{0}", item.Def);
                SpdValueLabel.text = string.Format("{0}", item.Spd);
                HpValueLabel.text = string.Format("{0}", item.Hp);
            }
        }
    }

    void UpdateSaleList()
    {
        List<TableData_SaleList> list = null;
        if (SelectedTab.Type != TableEnum.SaleCatecory.None)
        {
            list = saleList.FindAll(x => x.Category == SelectedTab.Type);
            list.Sort(
                delegate(TableData_SaleList x, TableData_SaleList y)
                {
                    int compare = x.Type.CompareTo(y.Type);
                    if(compare == 0 && x.Category == TableEnum.SaleCatecory.Equip)
                    {
                        TableData_Item xItem = TableManager.GetGameData(x.ItemName) as TableData_Item;
                        TableData_Item yItem = TableManager.GetGameData(y.ItemName) as TableData_Item;
                        if (xItem.SlotIdx == yItem.SlotIdx)
                        {
                            if (xItem.Grade == yItem.Grade) return xItem.Id.CompareTo(yItem.Id);
                            return xItem.Grade.CompareTo(yItem.Grade);
                        }
                        return xItem.SlotIdx.CompareTo(yItem.SlotIdx);
                    }
                    return compare;
                }
            );
        }
        else
        {
            list = saleList;
        }

        for (int i = 0; i < saleItemList.Count; ++i)
        {
            if (i < list.Count)
            {
                saleItemList[i].Init(list[i]);
            }
            else
            {
                saleItemList[i].Init(null);
            }
        }
        ListGrid.Reposition();
        ScrollView.ResetPosition();
    }

    void OnSelectTab(Component_Item_SaleTab tab)
    {
        SelectedTab = tab;
    }

    void OnSelectList(Component_Item_SaleItem item)
    {
        SelectedItem = item;
    }

    public void OnClick_Buy()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (ClickedEvent != null) ClickedEvent(this);
    }
}
