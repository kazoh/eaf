using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TableData_Map : TableData
{
    readonly public int Grade;
    readonly public TableEnum.MapType Type;
    readonly public float Time_GCrown;
    readonly public float Time_SCrown;
    readonly public int Reward;
    readonly public int Price;
    readonly public string PrefName;
    readonly public string ImageName;
    readonly public string IconName;
    readonly public string AddDrop_GCrown;
    readonly public string AddDrop_SCrown;
    readonly public string Bgm;
    readonly public int NeedSlotNum;
    readonly public int RegionId;

    public List<string> DropList;
    
    public TableData_Map() : base()
	{
        DropList = new List<string>();
	}

    public TableData_Map(IDictionary dict)
        : base(dict)
    {
        if (dict.Contains("GRADE")) Grade = Convert.ToInt32(dict["GRADE"]);
        if (dict.Contains("TYPE")) Type = (TableEnum.MapType)Enum.Parse(typeof(TableEnum.MapType), Convert.ToString(dict["TYPE"]), true);
        if (dict.Contains("COND_GOLD")) Time_GCrown = Convert.ToSingle(dict["COND_GOLD"]);
        if (dict.Contains("COND_SILVER")) Time_SCrown = Convert.ToSingle(dict["COND_SILVER"]);
        if (dict.Contains("REWARD")) Reward = Convert.ToInt32(dict["REWARD"]);
        if (dict.Contains("PRICE")) Price = Convert.ToInt32(dict["PRICE"]);
        if (dict.Contains("MODEL")) PrefName = Convert.ToString(dict["MODEL"]);
        if (dict.Contains("IMAGE")) ImageName = Convert.ToString(dict["IMAGE"]);
        if (dict.Contains("SOUND")) Bgm = Convert.ToString(dict["SOUND"]);
        if (dict.Contains("ICON")) IconName = Convert.ToString(dict["ICON"]);
        if (dict.Contains("ADD_DROP_1")) AddDrop_SCrown = Convert.ToString(dict["ADD_DROP_1"]);
        if (dict.Contains("ADD_DROP_2")) AddDrop_GCrown = Convert.ToString(dict["ADD_DROP_2"]);
        if (dict.Contains("NEED_SLOT_NUM")) NeedSlotNum = Convert.ToInt32(dict["NEED_SLOT_NUM"]);
        if (dict.Contains("REGION_ID")) RegionId = Convert.ToInt32(dict["REGION_ID"]);

        DropList = new List<string>();
        IList list = dict["DROP_ARRAY"] as IList;
        foreach (IDictionary dict2 in list)
        {
            if (dict2.Contains("DROP"))
            {
                string dropName = Convert.ToString(dict2["DROP"]);
                if(!string.IsNullOrEmpty(dropName)) DropList.Add(dropName);
            }
        }
    }
}
