using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TableData_MapList : TableData
{
    public struct UnlockCond
    {
        readonly public TableEnum.UnlockType Cond;
        readonly public int Value;

        public UnlockCond(IDictionary dict)
        {
            Cond = TableEnum.UnlockType.None;
            Value = 0;

            if (dict != null)
            {
                if (dict.Contains("COND")) Cond = (TableEnum.UnlockType)Enum.Parse(typeof(TableEnum.UnlockType), Convert.ToString(dict["COND"]), true);
                if (dict.Contains("VALUE")) Value = Convert.ToInt32(dict["VALUE"]);
            }
        }
    }
    readonly public string MapName;
    public List<UnlockCond> UnlockCondList;

    public TableData_MapList() : base()
	{
        MapName = string.Empty;
        UnlockCondList = new List<UnlockCond>();
	}

    public TableData_MapList(IDictionary dict)
        : base(dict)
    {
        if (dict.Contains("MAP")) MapName = Convert.ToString(dict["MAP"]);

        UnlockCondList = new List<UnlockCond>();
        IList list = dict["COND_ARRAY"] as IList;
        foreach (IDictionary dict2 in list)
        {
            UnlockCond data = new UnlockCond(dict2);
            UnlockCondList.Add(data);
        }
    }
}
