using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TableData_Mission : TableData
{
    readonly public TableEnum.MissionType Type;
    readonly public int Reward;
    readonly public int MapId;
    readonly public TableEnum.MapClearGrade MapClearGrade;

    public TableData_Mission() : base()
	{
	}

	public TableData_Mission(int id) : base(id)
    {
	}

    public TableData_Mission(IDictionary dict)
        : base(dict)
    {
        if (dict.Contains("TYPE")) Type = (TableEnum.MissionType)Enum.Parse(typeof(TableEnum.MissionType), Convert.ToString(dict["TYPE"]), true);
        if (dict.Contains("REWARD")) Reward = Convert.ToInt32(dict["REWARD"]);
        if (dict.Contains("MAPID")) MapId = Convert.ToInt32(dict["MAPID"]);
        if (dict.Contains("RESULTGRADE")) MapClearGrade = (TableEnum.MapClearGrade)Enum.Parse(typeof(TableEnum.MapClearGrade), Convert.ToString(dict["RESULTGRADE"]), true);
    }
}
