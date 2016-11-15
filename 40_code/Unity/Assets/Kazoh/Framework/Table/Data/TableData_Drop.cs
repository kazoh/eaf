using UnityEngine;
using System.Collections;  
using System.Collections.Generic;
using System;

public class TableData_Drop : TableData
{
    public struct DropItem
    {
        public string ItemName;
        public int Prob;

        public DropItem(IDictionary dict)
        {
            ItemName = string.Empty;
            Prob = 0;

            if (dict != null)
            {
                if (dict.Contains("ITEM")) ItemName = Convert.ToString(dict["ITEM"]);
                if (dict.Contains("PROB")) Prob = Convert.ToInt32(dict["PROB"]);
            }
        } 
    }

    readonly public int MinGold;
    readonly public int MaxGold;
    readonly public int MinCoin;
    readonly public int MaxCoin;
    public List<DropItem> DropList;

    public TableData_Drop() : base()
	{
        MinGold = 0;
        MaxGold = 0;
        MinCoin = 0;
        MaxCoin = 0;
        DropList = new List<DropItem>();
	}

    public TableData_Drop(IDictionary dict)
        : base(dict)
    {
        if (dict.Contains("GOLD")) MinGold = Convert.ToInt32(dict["GOLD"]);
        if (dict.Contains("ADD_GOLD")) MaxGold = MinGold + Convert.ToInt32(dict["ADD_GOLD"]);
        if (dict.Contains("COIN")) MinCoin = Convert.ToInt32(dict["COIN"]);
        if (dict.Contains("ADD_COIN")) MaxCoin = MinCoin + Convert.ToInt32(dict["ADD_COIN"]);

        DropList = new List<DropItem>();
        IList list = dict["DROP_ARRAY"] as IList;
        foreach (IDictionary dict2 in list)
        {
            DropItem data = new DropItem(dict2);
            DropList.Add(data);
        }
    }

    public string Drop()
    {
        int dice = UnityEngine.Random.Range(0,10000);
        for(int i=0; i < DropList.Count; ++i)
        {
            if (dice > DropList[i].Prob)
            {
                dice -= DropList[i].Prob;
                continue;
            }

            return DropList[i].ItemName;
        }

        return string.Empty;
    }
}
