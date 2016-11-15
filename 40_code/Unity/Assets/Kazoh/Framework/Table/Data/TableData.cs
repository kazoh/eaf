using UnityEngine;
using System.Collections;
using System;

public class TableData {

	readonly public int Id;
    readonly public string Name;
    readonly public string Str;
    readonly public string Desc;

	public TableData()
	{
		Id = int.MinValue;
	}

	public TableData(int id)
	{
		Id = id;
	}

    public TableData(IDictionary dict)
    {
        Id = Convert.ToInt32(dict["ID"]);
        Name = Convert.ToString(dict["NAME"]);
        if (dict.Contains("STR")) Str = Convert.ToString(dict["STR"]);
        if (dict.Contains("DESC")) Desc = Convert.ToString(dict["DESC"]);
    }
}
