using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TableData_Region : TableData
{
    readonly public string IconName;

    public TableData_Region() : base()
	{
	}

	public TableData_Region(int id) : base(id)
    {
	}

    public TableData_Region(IDictionary dict)
        : base(dict)
    {
        if (dict.Contains("ICON")) IconName = Convert.ToString(dict["ICON"]);
    }
}
