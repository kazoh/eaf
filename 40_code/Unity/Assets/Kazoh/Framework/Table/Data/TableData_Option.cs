using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TableData_Option : TableData
{
    readonly public TableEnum.OptionType Type;
    readonly public int Value;

    public TableData_Option() : base()
    {
    }

    public TableData_Option(IDictionary dict)
        : base(dict)
    {
        if (dict.Contains("TYPE")) Type = (TableEnum.OptionType)Enum.Parse(typeof(TableEnum.OptionType), Convert.ToString(dict["TYPE"]), true);
        if (dict.Contains("VALUE")) Value = Convert.ToInt32(dict["VALUE"]);
    }
}
