using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TableData_DailyReward : TableData
{
    readonly public TableEnum.MissionType Type;
    readonly public int Reward;

    public TableData_DailyReward() : base()
	{
	}

	public TableData_DailyReward(int id) : base(id)
    {
	}

    public TableData_DailyReward(IDictionary dict)
        : base(dict)
    {
        if (dict.Contains("TYPE")) Type = (TableEnum.MissionType)Enum.Parse(typeof(TableEnum.MissionType), Convert.ToString(dict["TYPE"]), true);
        if (dict.Contains("REWARD")) Reward = Convert.ToInt32(dict["REWARD"]);
    }
}
