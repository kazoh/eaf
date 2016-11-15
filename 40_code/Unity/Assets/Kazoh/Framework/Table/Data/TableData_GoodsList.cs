using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TableData_GoodsList : TableData
{
    readonly public TableEnum.GoodsType Type;
    readonly public TableEnum.MoneyType Currency;
    readonly public int Price;
    readonly public float Price_USD;
    readonly public int Price_YEN;
    readonly public int Price_WON;
    readonly public int Num;
    readonly public string IconName;
    readonly public int Membership;
    readonly public int ProductId;
    readonly public int PurchaseLimit;

    public List<string> SummonList;
    
    public TableData_GoodsList() : base()
	{
        SummonList = new List<string>();
	}

    public TableData_GoodsList(IDictionary dict)
        : base(dict)
    {
        if (dict.Contains("TYPE")) Type = (TableEnum.GoodsType)Enum.Parse(typeof(TableEnum.GoodsType), Convert.ToString(dict["TYPE"]), true);
        if (dict.Contains("CURRENCY")) Currency = (TableEnum.MoneyType)Enum.Parse(typeof(TableEnum.MoneyType), Convert.ToString(dict["CURRENCY"]), true);
        if (dict.Contains("PRICE")) Price = Convert.ToInt32(dict["PRICE"]);
        if (dict.Contains("PRICE_USD")) Price_USD = Convert.ToInt32(dict["PRICE_USD"]);
        if (dict.Contains("PRICE_YEN")) Price_YEN = Convert.ToInt32(dict["PRICE_YEN"]);
        if (dict.Contains("PRICE_WON")) Price_WON = Convert.ToInt32(dict["PRICE_WON"]);
        if (dict.Contains("NUM")) Num = Convert.ToInt32(dict["NUM"]);
        if (dict.Contains("ICON")) IconName = Convert.ToString(dict["ICON"]);
        if (dict.Contains("VIP")) Membership = Convert.ToInt32(dict["VIP"]);
        if (dict.Contains("PRODUCT_ID")) ProductId = Convert.ToInt32(dict["PRODUCT_ID"]);
        if (dict.Contains("BUY_LIMIT")) PurchaseLimit = Convert.ToInt32(dict["BUY_LIMIT"]);

        SummonList = new List<string>();
        IList list = dict["CHA_ARRAY"] as IList;
        foreach (IDictionary dict2 in list)
        {
            if (dict2.Contains("CHARACTER"))
            {
                string chaName = Convert.ToString(dict2["CHARACTER"]);
                if(!string.IsNullOrEmpty(chaName)) SummonList.Add(chaName);
            }
        }
    }
}
