using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TableData_Character : TableData
{
    readonly public int Grade;
    readonly public int Pow;
    readonly public int Dex;
    readonly public int Int;
    readonly public int Con;
    readonly public int Luc;
    readonly public int RndPow;
    readonly public int RndDex;
    readonly public int RndInt;
    readonly public int RndCon;
    readonly public int RndLuc;
    readonly public string SpriteName;
    readonly public string IconName;
    readonly public string BulletName;

    public List<string> OptionList;

    public string GradeStr
    {
        get
        {
            string gStr = "D";
            if (Grade == 2) gStr = "C";
            else if (Grade == 3) gStr = "B";
            else if (Grade == 4) gStr = "A";
            else if (Grade == 5) gStr = "S";
            else if (Grade == 6) gStr = "SS";
            return gStr;
        }
    }

    public TableData_Character() : base()
	{
        OptionList = new List<string>();
	}

    public TableData_Character(IDictionary dict)
        : base(dict)
    {
        if (dict.Contains("GRADE")) Grade = Convert.ToInt32(dict["GRADE"]);
        if (dict.Contains("POW")) Pow = Convert.ToInt32(dict["POW"]);
        if (dict.Contains("DEX")) Dex = Convert.ToInt32(dict["DEX"]);
        if (dict.Contains("INT")) Int = Convert.ToInt32(dict["INT"]);
        if (dict.Contains("CON")) Con = Convert.ToInt32(dict["CON"]);
        if (dict.Contains("LUC")) Luc = Convert.ToInt32(dict["LUC"]);
        if (dict.Contains("RND_POW")) RndPow = Convert.ToInt32(dict["RND_POW"]);
        if (dict.Contains("RND_DEX")) RndDex = Convert.ToInt32(dict["RND_DEX"]);
        if (dict.Contains("RND_INT")) RndInt = Convert.ToInt32(dict["RND_INT"]);
        if (dict.Contains("RND_CON")) RndCon = Convert.ToInt32(dict["RND_CON"]);
        if (dict.Contains("RND_LUC")) RndLuc = Convert.ToInt32(dict["RND_LUC"]);
        if (dict.Contains("MODEL")) SpriteName = Convert.ToString(dict["MODEL"]);
        if (dict.Contains("ICON")) IconName = Convert.ToString(dict["ICON"]);
        if (dict.Contains("BULLET")) BulletName = Convert.ToString(dict["BULLET"]);

        OptionList = new List<string>();
        IList list = dict["OPTION_ARRAY"] as IList;
        foreach (IDictionary dict2 in list)
        {
            if (dict2.Contains("OPTION"))
            {
                string optionName = Convert.ToString(dict2["OPTION"]);
                if (!string.IsNullOrEmpty(optionName)) OptionList.Add(optionName);
            }
        }
    }
}
