using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TableData_SaleList : TableData
{
    readonly public TableEnum.SaleCatecory Category;
    readonly public TableEnum.MoneyType Type;
    readonly public string ItemName;
    readonly public int Amount;

    public TableData_SaleList() : base()
	{
	}

    public TableData_SaleList(IDictionary dict)
        : base(dict)
    {
        if (dict.Contains("CATEGORY")) Category = (TableEnum.SaleCatecory)Enum.Parse(typeof(TableEnum.SaleCatecory), Convert.ToString(dict["CATEGORY"]), true);
        if (dict.Contains("TYPE")) Type = (TableEnum.MoneyType)Enum.Parse(typeof(TableEnum.MoneyType), Convert.ToString(dict["TYPE"]), true);
        if (dict.Contains("ITEM")) ItemName = Convert.ToString(dict["ITEM"]);
        if (dict.Contains("NUM")) Amount = Convert.ToInt32(dict["NUM"]);
    }
}
