using UnityEngine;
using System.Collections;
using System;

public class StringData {

	readonly public string Name;
    readonly public string Kor;
    readonly public string Eng;
    readonly public string Jap;
    

	public StringData()
	{
	}

    public StringData(IDictionary dict)
    {
        Name = Convert.ToString(dict["NAME"]);
        if (dict.Contains("STR_KR")) Kor = Convert.ToString(dict["STR_KR"]);
        if (dict.Contains("STR_EN")) Eng = Convert.ToString(dict["STR_EN"]);
        if (dict.Contains("STR_JP")) Jap = Convert.ToString(dict["STR_JP"]);
    }
}
