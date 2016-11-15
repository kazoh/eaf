using UnityEngine;
using System.Collections;
using System;

public class ConfigData {

    readonly public string Name;
    public TableEnum.ConfigType Type { get; protected set; }

	public ConfigData()
	{
		Name = "ver_999";
	}

    public ConfigData(int id)
	{
		Name = "ver_"+id;
	}

    public ConfigData(IDictionary dict)
    {
        Name = Convert.ToString(dict["NAME"]);
    }
}
